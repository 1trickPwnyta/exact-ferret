using System;
using System.Collections.Generic;
using System.Threading;
using Logging;
using Language;
using Search;
using Http;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Exact_Ferret.Settings_Classes;
using System.Collections;

namespace Exact_Ferret
{
    class ExactFerret
    {
        private static bool showedApiLimitWarning = false;
        private static bool[] showedSingleApiLimitWarning = new bool[PropertiesManager.getSearchEngineOrder().Count];

        public static void startBackgroundProcess(bool delay)
        {
            Log.info("The Exact Ferret background process is starting.");
            
            Communication.startListening(0);
            
            while (true)
            {
                if (!delay)
                {
                    bool onBatteryPower = SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Online;
                    Log.trace("On battery: " + onBatteryPower);

                    if (!onBatteryPower || !PropertiesManager.getDontRunOnBatteryEnabled())
                    {
                        Log.trace("Checking today's schedule...");
                        bool inSchedule = PropertiesManager.isScheduledNow();

                        try
                        {
                            if (inSchedule)
                                runOnce();
                            else
                                Log.trace("Today's schedule prevents Exact Ferret from running right now.");
                        }
                        catch (Exception e)
                        {
                            Log.error("An unhandled exception occurred in the Exact Ferret run: " + e.Message + ": " + e.StackTrace);
                        }
                    }
                }
                else
                    delay = false;

                // Wait for an interval before running again

                TimeSpan runInterval = PropertiesManager.getRunIntervalTimeSpan();
                Communication.setRunCountdown((int) runInterval.TotalSeconds);

                Log.trace("Going to sleep for " + (int) runInterval.TotalMinutes + " minutes, " + runInterval.Seconds + 
                        " seconds.");
                Thread.Sleep((int) runInterval.TotalMilliseconds);
            }
        }

        public static void runOnce()
        {
            runOnce(null);
        }

        public static void runOnce(string term)
        {
            runOnce(term, null);
        }

        public static void runOnce(string term, SearchOptions searchOptions)
        {
            Log.trace("Starting a new run.");

            // Restore the original settings
            PropertiesManager.restore();

            // Check for scheduled settings files
            SettingsSchedule[] applicableSchedules = SettingsSchedule.getScheduledSettings();
            foreach (SettingsSchedule schedule in applicableSchedules)
                applySettingsFromSchedule(schedule);

            // Re-initialize logging (settings may have changed)
            Log.setLogLevel(PropertiesManager.getLogLevel());
            Log.setLogFileLocation(PropertiesManager.getLogFilePath());
            Log.setLogRolloverKBytes(PropertiesManager.getLogRolloverKilobytes());

            // Clean up old logs
            int logRetentionDays = PropertiesManager.getLogRetentionDays();
            Log.trace("Cleaning up log files older than " + logRetentionDays + " days.");
            Log.clearOldLogs(logRetentionDays);

            // Check for software updates
            if (PropertiesManager.getAutoUpdateEnabled())
            {
                UpdateManager.checkForUpdates();
            }

            // Trust all SSL certificates
            Log.trace("Will now trust any server certificate.");
            HttpUtil.trustAllCertificates();

            bool done = false;
            string imageFilePath = null;
            int downloadAttempts = 0;
            int maxAttempts = PropertiesManager.getDownloadAttemptLimit();
            List<string> imageUrls = null;
            ISearchEngine engine = null;

            while (!done)
            {
                downloadAttempts++;

                if (imageUrls == null || imageUrls.Count == 0)
                {
                    // Get the search term
                    if (term == null)
                    {
                        if (searchOptions == null || searchOptions.getDictionaryFile() == null)
                            term = Dictionary.getRandomWord();
                        else
                            term = Dictionary.getRandomWord(searchOptions.getDictionaryFile());
                    }

                    // Modify the search term
                    string searchModifier = PropertiesManager.getSearchTermModifier();
                    if (!searchModifier.Equals(""))
                    {
                        Log.trace("Appending search term modifier " + searchModifier);
                        term += " " + searchModifier;
                    }

                    // Get ready to search

                    Log.info("Searching for " + term);

                    // Determine which search engine to use
                    int enginesWithNoKey = 0;
                    bool otherError = false;
                    StringCollection searchEngineOrder = PropertiesManager.getSearchEngineOrder();

                    for (int i = 0; i < searchEngineOrder.Count; i++)
                    {
                        imageUrls = null;
                        string searchEngine = null;

                        if (searchOptions != null && searchOptions.getSearchEngine() != null)
                        {
                            switch (searchOptions.getSearchEngine())
                            {
                                case SearchOptions.GOOGLE:
                                    engine = new GoogleSearchEngine();
                                    searchEngine = SearchOptions.GOOGLE;
                                    break;
                                case SearchOptions.BING:
                                    engine = new BingSearchEngine();
                                    searchEngine = SearchOptions.BING;
                                    break;
                                default:
                                    Log.warning("Unsupported search engine detected: " + searchOptions.getSearchEngine());
                                    break;
                            }
                        }
                        else
                        {
                            searchEngine = searchEngineOrder[i];

                            switch (searchEngine)
                            {
                                case "Google":
                                    if (!PropertiesManager.getGoogleApiKey().Equals(""))
                                    {
                                        Log.trace("Found Google Custom Search API key. Using Google.");
                                        engine = new GoogleSearchEngine();
                                    }
                                    else
                                    {
                                        Log.trace("No Google Custom Search API key found. Skipping Google.");
                                        enginesWithNoKey++;
                                        continue;
                                    }
                                    break;
                                case "Bing":
                                    if (!PropertiesManager.getBingAccessToken().Equals(""))
                                    {
                                        Log.trace("Found Bing access token. Using Bing.");
                                        engine = new BingSearchEngine();
                                    }
                                    else
                                    {
                                        Log.trace("No Bing access token found. Skipping Bing.");
                                        enginesWithNoKey++;
                                        continue;
                                    }
                                    break;
                                default:
                                    Log.warning("Unsupported search engine detected: " + searchEngineOrder[i]);
                                    continue;
                            }
                        }

                        // This could happen if the search engine in the search options was invalid
                        if (engine == null)
                            break;

                        // Perform the search
                        setSearchOptions(engine, searchOptions);
                        imageUrls = engine.searchForImages(term);

                        // Ensure at least one image was returned
                        if (imageUrls == null || imageUrls.Count == 0)
                        {
                            if (engine.isApiLimitExceeded())
                            {
                                if (PropertiesManager.getSingleApiLimitWarningEnabled() && !showedSingleApiLimitWarning[i])
                                {
                                    new Thread(new ThreadStart(() =>
                                    MessageBox.Show("Your " + searchEngine + " API limit has been exceeded.", "Exact Ferret",
                                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    )).Start();
                                    showedSingleApiLimitWarning[i] = true;
                                }
                            }
                            else
                            {
                                // Something else went wrong - start over
                                otherError = true;
                                break;
                            }
                        }
                        else
                        {
                            // Success! Move on
                            showedSingleApiLimitWarning[i] = false;
                            break;
                        }

                        if (searchOptions != null && searchOptions.getSearchEngine() != null)
                        {
                            break;
                        }
                    }

                    // This could happen if the search engine in the search options was invalid
                    if (engine == null)
                        break;

                    if (otherError)
                    {
                        if (downloadAttempts >= maxAttempts)
                        {
                            Log.error("The download attempt limit has been reached. Giving up for now.");
                            break;
                        }
                        else
                        {
                            term = null;
                            continue;
                        }
                    }

                    if (imageUrls == null || imageUrls.Count == 0)
                    {
                        if (enginesWithNoKey == searchEngineOrder.Count)
                            Log.error("No search API keys found.");
                        else if (searchOptions == null || searchOptions.getSearchEngine() == null)
                        {
                            Log.warning("All search API limits have been exhausted.");
                            if (PropertiesManager.getApiLimitWarningEnabled() && !showedApiLimitWarning)
                            {
                                new Thread(new ThreadStart(() =>
                                MessageBox.Show("All of your search API limits have been exhausted.", "Exact Ferret",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                )).Start();
                                showedApiLimitWarning = true;
                            }
                        }

                        if (PropertiesManager.getCycleAtApiLimitEnabled())
                            cycleOldImage();
                        break;
                    }
                }

                // API daily limit has been reset
                showedApiLimitWarning = false;

                // Select a random image
                int randomIndex = new Random().Next(imageUrls.Count);
                string imageUrl = imageUrls[randomIndex];
                string caption = engine.getCaptionForImage(imageUrl);
                Log.info("Selected image at " + imageUrl);

                // Remove the selected image from the list so it isn't selected again in case of failure
                imageUrls.Remove(imageUrl);

                // Download the image
                imageFilePath = PictureFileManager.downloadImage(imageUrl, term, caption);

                // Ensure the download completed successfully
                if (imageFilePath == null)
                {
                    if (downloadAttempts >= maxAttempts)
                    {
                        Log.error("The download attempt limit has been reached. Giving up for now.");
                        break;
                    }
                    else
                    {
                        if (imageUrls == null || imageUrls.Count == 0)
                            term = null;
                        continue;
                    }
                }

                // Verify we have a valid image and it meets the required size

                bool isValid = true;
                int width = 0, height = 1;
                try
                {
                    using (Image img = new Bitmap(imageFilePath))
                    {
                        width = img.Width;
                        height = img.Height;
                        img.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Log.warning("The file is not a valid image.");
                    isValid = false;
                }

                if (isValid) {
                    double ratio = (double) width / height;

                    int minWidth = PropertiesManager.getMinimumImageWidth();
                    int minHeight = PropertiesManager.getMinimumImageHeight();
                    int maxWidth = PropertiesManager.getMaximumImageWidth();
                    int maxHeight = PropertiesManager.getMaximumImageHeight();
                    double minRatio = PropertiesManager.getMinimumImageWidthHeightRatio();
                    double maxRatio = PropertiesManager.getMaximumImageWidthHeightRatio();

                    if (width < minWidth || width > maxWidth || height < minHeight || height > maxHeight ||
                            ratio < minRatio || ratio > maxRatio)
                    {
                        Log.warning("The downloaded image does not meet the advertised size. Actual size: " + width + "x" + height);
                        isValid = false;
                    }
                }

                // Added in version 3.3.0
                if (!isValid)
                {
                    if (downloadAttempts >= maxAttempts)
                    {
                        Log.error("The download attempt limit has been reached. Giving up for now.");
                        break;
                    }
                    else
                    {
                        if (imageUrls == null || imageUrls.Count == 0)
                            term = null;
                        continue;
                    }
                }

                // Change the picture
                bool changeSuccess = Desktop.changePicture(imageFilePath);

                if (changeSuccess)
                    Log.trace("The run was successful. Bye bye for now!");
                else
                {
                    if (downloadAttempts >= maxAttempts)
                    {
                        Log.error("The download attempt limit has been reached. Giving up for now.");
                        break;
                    }
                    else
                    {
                        if (imageUrls == null || imageUrls.Count == 0)
                            term = null;
                        continue;
                    }
                }

                done = true;
            }
        }

        private static void setSearchOptions(ISearchEngine engine, SearchOptions searchOptions)
        {
            Log.trace("Loading search options.");

            // Allow only JPG images
            engine.allowImageFileType("jpg");

            string apiKey;
            if (engine.GetType() == typeof(GoogleSearchEngine))
                apiKey = PropertiesManager.getGoogleApiKey();
            else if (engine.GetType() == typeof(BingSearchEngine))
                apiKey = PropertiesManager.getBingAccessToken();
            else
                apiKey = "?";
            engine.setApiKey(apiKey);

            int requestTimeoutSeconds = PropertiesManager.getHttpRequestTimeoutSeconds();
            Log.trace("HTTP request timeout: " + requestTimeoutSeconds);
            engine.setRequestTimeoutSeconds(requestTimeoutSeconds);

            StringCollection blockedImages = PropertiesManager.getBlockedImagesStringCollection();
            Log.trace("Blocked images: " + blockedImages.Count + " images");
            engine.blockImages(blockedImages);

            StringCollection blockedDomains = PropertiesManager.getBlockedDomainsStringCollection();
            Log.trace("Blocked domains: " + blockedDomains.Count + " domains");
            engine.blockDomains(blockedDomains);

            if (searchOptions == null)
                searchOptions = new SearchOptions();

            int minimumWidth = PropertiesManager.getMinimumImageWidth();
            Log.trace("Minimum width: " + minimumWidth);
            int minimumHeight = PropertiesManager.getMinimumImageHeight();
            Log.trace("Minimum height: " + minimumHeight);
            engine.setMinimumImageDimensions(minimumWidth, minimumHeight);

            int maximumWidth = PropertiesManager.getMaximumImageWidth();
            Log.trace("Maximum width: " + maximumWidth);
            int maximumHeight = PropertiesManager.getMaximumImageHeight();
            Log.trace("Maximum height: " + maximumHeight);
            engine.setMaximumImageDimensions(maximumWidth, maximumHeight);

            double minimumRatio = PropertiesManager.getMinimumImageWidthHeightRatio();
            Log.trace("Minimum width:height ratio: " + minimumRatio);
            double maximumRatio = PropertiesManager.getMaximumImageWidthHeightRatio();
            Log.trace("Maximum width:height ratio: " + maximumRatio);
            engine.setWidthHeightRatioRange(minimumRatio, maximumRatio);

            bool safeSearch = PropertiesManager.getSafeSearchEnabled();
            Log.trace("Safe Search: " + safeSearch);
            engine.setSafeSearch(safeSearch);

            string searchDomain = PropertiesManager.getSearchDomain();
            Log.trace("Search domain: " + searchDomain);
            engine.setSiteSearchDomain(searchDomain);

            int colorsInImage = PropertiesManager.getColorsInImage();
            Log.trace("Colors in image: " + colorsInImage);
            engine.setColorsInImage(colorsInImage);

            int typeOfImage = PropertiesManager.getTypeOfImage();
            Log.trace("Type of image: " + typeOfImage);
            engine.setTypeOfImage(typeOfImage);

            // Override with options specified at command line
            searchOptions.execute(engine);
        }

        private static void cycleOldImage()
        {
            // Get an image path
            string imageFilePath = Desktop.getRandomPicturePath();

            // Change the picture
            bool changeSuccess = Desktop.changePicture(imageFilePath);

            if (changeSuccess)
                Log.trace("The run was successful. Bye bye for now!");
        }

        private static void applySettingsFromSchedule(SettingsSchedule schedule)
        {
            string fileName = schedule.file.info.FullName;
            Log.trace("Applying settings from file " + fileName);
            bool success = PropertiesManager.importSettings(fileName);
            if (!success)
                Log.error("Failed to import settings from file " + fileName);
        }

        public static void panic()
        {
            string panicImage = PropertiesManager.getPanicImage();
            if (!panicImage.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && !panicImage.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                panicImage = "";
            Desktop.changePicture(panicImage, true);
        }
    }
}
