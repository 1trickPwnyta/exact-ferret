using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Logging;
using Http;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Exact_Ferret.Settings_Classes;

namespace Search
{
    class GoogleSearchEngine : ISearchEngine
    {
        private const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.146 Safari/537.36";
        //private const string SEARCH_URL = "https://www.google.com/search?q=";
        private const string SEARCH_URL = "https://www.googleapis.com/customsearch/v1?";
        private const string IMAGE_URL_REGEX = "https?://.*";
        private const string URL_ENCODING = "UTF-8";
        private const string SEARCH_ENGINE_ID = "005108198917574406650:nfvnah8p1nm";
        private const string SEARCH_FIELDS = "items(link,title,snippet,image/width,image/height)";

        // Aspect ratio thresholds
        private const double ASPECT_TALL_RANGE_MAX = 0.69;
        private const double ASPECT_SQUARE_RANGE_MIN = 0.7;
        private const double ASPECT_SQUARE_RANGE_MAX = 1.29;
        private const double ASPECT_WIDE_RANGE_MIN = 1.3;
        private const double ASPECT_WIDE_RANGE_MAX = 1.79;
        private const double ASPECT_PANORAMIC_RANGE_MIN = 1.8;

        // Search options
        private string apiKey = null;
        private List<string> imageFileTypes = new List<string>();
        private List<string> blockedImages = new List<string>();
        private List<string> blockedDomains = new List<string>();
        private int minimumImageWidth = 1024, minimumImageHeight = 768;
        private int maximumImageWidth = 999999, maximumImageHeight = 999999;
        private double minimumImageWHRatio = 1.2, maximumImageWHRatio = 1.8;
        private int colorsInImage = ColorsInImage.ANY_COLOR;
        private int typeOfImage = TypeOfImage.ANY_TYPE;
        private bool safeSearch = true;
        private string siteSearch = null;
        private int requestTimeoutSeconds = 10;

        // Errors
        private bool dailyLimitExceeded = false;

        // Metadata
        private JArray lastSearchResults;

        public List<string> searchForImages(string searchTerm)
        {
            if (searchTerm == null)
            {
                Log.warning("Can't search with a null search term. Returning an empty result set.");
                return new List<string>();
            }

            string query = buildSearchQuery(searchTerm);
            Log.trace("Using search query: " + query);

            // Remove any old HTTP headers
            HttpUtil.removeAllCustomHeaders();

            // Set HTTP options
            HttpUtil.setUserAgent(USER_AGENT);
            HttpUtil.setRequestTimeoutSeconds(requestTimeoutSeconds);

            string responseText = HttpUtil.get(query);
            Log.special("Received the following response from Google: \r\n" + responseText, "RESULTS");

            if (responseText != null)
            {
                // Daily limit has been reset
                dailyLimitExceeded = false;

                List<string> imageUrls = getImageUrls(responseText);

                if (imageUrls.Count == 0)
                {
                    Log.warning("No images were found for the query.");
                    return new List<string>();
                }
                Log.info("Found " + imageUrls.Count + " images that matched the search criteria.");

                return imageUrls;
            }
            else
            {
                HttpStatusCode error = HttpUtil.getLastErrorCode();

                if (error == HttpStatusCode.Forbidden || error == HttpStatusCode.BadRequest || error == HttpStatusCode.Unauthorized)
                {
                    string errorJson = HttpUtil.getLastErrorResponse();
                    JObject errorObject = (JObject)JsonConvert.DeserializeObject(errorJson);
                    JObject errorObject2 = (JObject)errorObject["error"];
                    JArray errorArray = (JArray)errorObject2["errors"];
                    JObject firstError = (JObject)errorArray[0];
                    string reason = firstError["reason"].ToString();

                    if (reason.Equals("dailyLimitExceeded") || error == HttpStatusCode.Unauthorized)
                    {
                        Log.warning("Your Google Custom Search API daily limit has been exceeded.");
                        dailyLimitExceeded = true;
                    }
                    else if (reason.Equals("keyInvalid"))
                    {
                        Log.error("Your Google Custom Search API key (" + apiKey + ") is invalid.");
                    }
                    else
                    {
                        Log.warning("Unknown error received from Google Custom Search API: " + reason);
                    }
                }

                return new List<string>();
            }
        }

        public bool isApiLimitExceeded()
        {
            return dailyLimitExceeded;
        }

        public void setApiKey(string key)
        {
            apiKey = key;
        }

        public void allowImageFileType(string fileType)
        {
            if (fileType.Equals("jpeg"))
                fileType = "jpg";
            if (!imageFileTypes.Contains(fileType.ToLower()))
            {
                imageFileTypes.Add(fileType.ToLower());
            }
        }

        public void blockImage(string imageUrl)
        {
            blockedImages.Add(imageUrl);
        }

        public void blockDomain(string domain)
        {
            blockedDomains.Add(domain);
        }

        public void blockImages(StringCollection imageUrls)
        {
            string[] temp = new string[imageUrls.Count];
            ArrayList.Adapter(imageUrls).CopyTo(temp);
            blockedImages.AddRange(temp);
        }

        public void blockDomains(StringCollection domains)
        {
            string[] temp = new string[domains.Count];
            ArrayList.Adapter(domains).CopyTo(temp);
            blockedDomains.AddRange(temp);
        }

        public void setMinimumImageWidth(int width)
        {
            minimumImageWidth = width;
        }

        public void setMinimumImageHeight(int height)
        {
            minimumImageHeight = height;
        }

        public void setMinimumImageDimensions(int width, int height)
        {
            minimumImageWidth = width;
            minimumImageHeight = height;
        }

        public void setMaximumImageWidth(int width)
        {
            maximumImageWidth = width;
        }

        public void setMaximumImageHeight(int height)
        {
            maximumImageHeight = height;
        }

        public void setMaximumImageDimensions(int width, int height)
        {
            maximumImageWidth = width;
            maximumImageHeight = height;
        }

        public void setMinimumWidthHeightRatio(double minimum)
        {
            minimumImageWHRatio = minimum;
        }

        public void setMaximumWidthHeightRatio(double maximum)
        {
            maximumImageWHRatio = maximum;
        }

        public void setWidthHeightRatioRange(double minimum, double maximum)
        {
            minimumImageWHRatio = minimum;
            maximumImageWHRatio = maximum;
        }

        public void setSafeSearch(bool safeSearch)
        {
            this.safeSearch = safeSearch;
        }

        public void setColorsInImage(int colorsInImage)
        {
            this.colorsInImage = colorsInImage;
        }

        public void setTypeOfImage(int typeOfImage)
        {
            this.typeOfImage = typeOfImage;
        }

        public void setSiteSearchDomain(string siteSearchDomain)
        {
            siteSearch = siteSearchDomain;
        }

        public void setRequestTimeoutSeconds(int seconds)
        {
            requestTimeoutSeconds = seconds;
        }

        private string buildSearchQuery(string searchTerm)
        {
            Log.trace("Building the search query.");
            string query = SEARCH_URL;

            // Append the API key
            Log.trace("Using Google Custom Search API key " + apiKey);
            query += "key=" + apiKey;

            // Append the search engine ID
            Log.trace("Using Custom Search Engine ID " + SEARCH_ENGINE_ID);
            query += "&cx=" + SEARCH_ENGINE_ID;

            // Append the search term
            query += "&q=" + WebUtility.UrlEncode(searchTerm);

            // Append the search type
            query += "&searchType=image";

            // Append the search fields
            query += "&fields=" + SEARCH_FIELDS;

            // Safe Search
            if (safeSearch)
            {
                query += "&safe=high";
            }
            else
            {
                query += "&safe=off";
            }

            // Site Search
            if (siteSearch != null && !siteSearch.Equals(""))
            {
                query += "&siteSearch=" + WebUtility.UrlEncode(siteSearch);
            }

            // Image size
            if (minimumImageWidth >= 1024 || minimumImageHeight >= 972)
            {
                query += "&imgSize=huge";
            }
            else if (minimumImageWidth >= 800 || minimumImageHeight >= 751)
            {
                query += "&imgSize=xxlarge";
            }
            else if (minimumImageWidth >= 480 || minimumImageHeight >= 470)
            {
                query += "&imgSize=xlarge";
            }
            else
            {
                if (maximumImageWidth <= 272 || maximumImageHeight <= 151)
                {
                    query += "&imgSize=icon";
                }
                if (maximumImageWidth <= 320 || maximumImageHeight <= 318)
                {
                    query += "&imgSize=small";
                }
                else
                {
                    query += "&imgSize=large";
                }
            }

            // Colors in image
            switch (colorsInImage)
            {
                case ColorsInImage.FULL_COLOR: query += "&imgColorType=color"; break;
                case ColorsInImage.BLACK_AND_WHITE: query += "&imgColorType=gray"; break;
                case ColorsInImage.YELLOW: query += "&imgDominantColor=yellow"; break;
                case ColorsInImage.GREEN: query += "&imgDominantColor=green"; break;
                case ColorsInImage.BLUE: query += "&imgDominantColor=blue"; break;
                case ColorsInImage.PURPLE: query += "&imgDominantColor=purple"; break;
                case ColorsInImage.TEAL: query += "&imgDominantColor=teal"; break;
                case ColorsInImage.PINK: query += "&imgDominantColor=pink"; break;
                case ColorsInImage.WHITE: query += "&imgDominantColor=white"; break;
                case ColorsInImage.GRAY: query += "&imgDominantColor=gray"; break;
                case ColorsInImage.BLACK: query += "&imgDominantColor=black"; break;
                case ColorsInImage.BROWN: query += "&imgDominantColor=brown"; break;
            }

            // Type of image
            switch (typeOfImage)
            {
                case TypeOfImage.FACE: case TypeOfImage.PORTRAIT: query += "&imgType=face"; break;
                case TypeOfImage.PHOTO: query += "&imgType=photo"; break;
                case TypeOfImage.CLIP_ART: query += "&imgType=clipart"; break;
                case TypeOfImage.LINE_DRAWING: query += "&imgType=lineart"; break;
            }

            // File type
            if (imageFileTypes.Count == 1)
            {
                string imageFileType = imageFileTypes[0];
                if (imageFileType.Equals("jpg") || imageFileType.Equals("png") ||
                    imageFileType.Equals("gif") || imageFileType.Equals("bmp"))
                {
                    query += "&fileType=" + imageFileType;
                }
            }

            // Set a random start index for a random page
            int startIndex = new Random().Next(92);
            query += "&start=" + startIndex;

            return query;
        }

        private List<string> getImageUrls(string json)
        {
            List<string> validUrls = new List<string>();

            // Deserialize the JSON string
            JObject googleImageResults = (JObject) JsonConvert.DeserializeObject(json);

            // Get the items array
            if (googleImageResults["items"] != null)
            {
                JArray items = (JArray)googleImageResults["items"];
                lastSearchResults = items;  // Remember the results in case metadata is required later

                // Go through each item and validate it, then add the URL to the list
                foreach (JObject item in items)
                {
                    string url = item["link"].ToString();

                    // Check syntax of URL
                    if (!Regex.IsMatch(url, IMAGE_URL_REGEX, RegexOptions.Multiline))
                        continue;

                    Log.trace("Parsed image URL: " + url);

                    // Check the file type
                    bool matchType = false;
                    if (imageFileTypes.Count == 0)
                    {
                        matchType = true;
                    }
                    else
                    {
                        foreach (string fileType in imageFileTypes)
                            if (url.ToLower().EndsWith("." + fileType) || (url.ToLower().EndsWith(".jpeg") && fileType.Equals("jpg")))
                            {
                                matchType = true;
                                break;
                            }
                    }
                    if (!matchType)
                    {
                        Log.trace("The image is not of an allowed file type.");
                        continue;
                    }

                    // Check if the image has been blocked
                    bool allowed = true;
                    string domain = new Uri(url).Host.ToLower();
                    while (domain.Contains("."))
                    {
                        if (blockedDomains.Contains(domain))
                        {
                            Log.trace("The domain " + domain + " is not allowed.");
                            allowed = false;
                        }
                        domain = domain.Substring(domain.IndexOf(".") + 1);
                    }
                    string urlNoProtocol = url.Contains("://") ? url.Substring(url.IndexOf("://") + 3).ToLower() : url.ToLower();
                    if (blockedImages.Contains(urlNoProtocol))
                    {
                        Log.trace("The image at URL " + urlNoProtocol + " is not allowed.");
                        allowed = false;
                    }
                    if (!allowed)
                    {
                        continue;
                    }

                    // Verify the image size and aspect ratio
                    try
                    {
                        JObject image = (JObject)item["image"];

                        int width = int.Parse(image["width"].ToString());
                        int height = int.Parse(image["height"].ToString());

                        Log.trace("Size of the image: " + width + "x" + height);

                        bool sizeMetMin = width >= minimumImageWidth &&
                                height >= minimumImageHeight;
                        if (sizeMetMin)
                            Log.trace("The image met or exceeded the minimum size.");
                        else
                            Log.trace("The image did not meet the minimum size.");

                        bool sizeMetMax = width <= maximumImageWidth &&
                            height <= maximumImageHeight;
                        if (sizeMetMax)
                            Log.trace("The image met or fell below the maximum size.");
                        else
                            Log.trace("The image did not meet the maximum size.");

                        if (!sizeMetMin || !sizeMetMax)
                            continue;

                        double imageRatio = (double)width / height;
                        Log.trace("Width:Height ratio of the image: " + imageRatio);

                        bool ratioMet = imageRatio >= minimumImageWHRatio &&
                                imageRatio <= maximumImageWHRatio;
                        if (ratioMet)
                            Log.trace("The image falls within the required ratio range");
                        else
                            Log.trace("The image falls outside the required ratio range");

                        if (!ratioMet)
                            continue;
                    }
                    catch (Exception e)
                    {
                        Log.warning("Failed to find the size of the image at " + url);
                        continue;
                    }

                    // Add the image URL to the list of valid image URLs
                    validUrls.Add(url);
                }
            }

            return validUrls;
        }

        public string getCaptionForImage(string imageUrl)
        {
            // Go through each item and validate it, then add the URL to the list
            foreach (JObject item in lastSearchResults)
            {
                string url = item["link"].ToString();
                if (url.ToLower().Equals(imageUrl.ToLower()))
                {
                    string caption = item["title"].ToString();
                    return caption;
                }
            }

            Log.warning("Could not get the caption for image at " + imageUrl);
            return null;
        }
    }
}
