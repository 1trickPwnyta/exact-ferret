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
using Exact_Ferret;

namespace Search
{
    class BingSearchEngine : ISearchEngine
    {
        private const string SEARCH_URL = "https://api.datamarket.azure.com/Bing/Search/Image?$format=json";
        private const string IMAGE_URL_REGEX = "https?://.*";
        private const string URL_ENCODING = "UTF-8";

        // Aspect ratio thresholds
        private const double ASPECT_SQUARE_RANGE_MIN = 0.7;
        private const double ASPECT_WIDE_RANGE_MIN = 1.3;

        // Search options
        private string accessToken = null;
        private List<string> imageFileTypes = new List<string>();
        private List<string> blockedImages = new List<string>();
        private List<string> blockedDomains = new List<string>();
        private int minimumImageWidth = 1024, minimumImageHeight = 768;
        private int maximumImageWidth = 999999, maximumImageHeight = 999999;
        private double minimumImageWHRatio = 1.2, maximumImageWHRatio = 1.8;
        private int colorsInImage = ColorsInImage.ANY_COLOR;
        private int typeOfImage = TypeOfImage.ANY_TYPE;
        private bool safeSearch = true;
        private int requestTimeoutSeconds = 10;

        // Errors
        private bool monthlyLimitExceeded = false;

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
            Log.trace("Settings HTTP request timeout to " + requestTimeoutSeconds + " seconds.");
            HttpUtil.setRequestTimeoutSeconds(requestTimeoutSeconds);

            // Set the access token
            Log.trace("Setting the authorization header with the value of the access token.");
            HttpUtil.setHeader("Authorization", "Basic " + Base64Util.Base64Encode(accessToken + ":" + accessToken));

            string responseText = HttpUtil.get(query);
            Log.special("Received the following response from Bing: \r\n" + responseText, "RESULTS");

            if (responseText != null)
            {
                // Monthly limit has been reset
                monthlyLimitExceeded = false;

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

                if (error == HttpStatusCode.ServiceUnavailable)
                {
                    Log.warning("Your Bing Search API monthly limit has been exceeded.");
                    monthlyLimitExceeded = true;
                }
                else if (error == HttpStatusCode.Unauthorized)
                {
                    Log.error("Your Bing API Access Token (" + accessToken + ") is invalid.");
                }
                else if (error == HttpStatusCode.Forbidden || error == HttpStatusCode.BadRequest)
                {
                    Log.error("Unknown error received from Bing Search API.");
                }

                return new List<string>();
            }
        }

        public bool isApiLimitExceeded()
        {
            return monthlyLimitExceeded;
        }

        public void setApiKey(string key)
        {
            accessToken = key;
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
            // Not supported
        }

        public void setRequestTimeoutSeconds(int seconds)
        {
            requestTimeoutSeconds = seconds;
        }

        private string buildSearchQuery(string searchTerm)
        {
            Log.trace("Building the search query.");
            string query = SEARCH_URL;

            // Append the search term
            query += "&Query=" + WebUtility.UrlEncode("'" + searchTerm + "'");

            // Safe Search
            if (safeSearch)
            {
                query += "&Adult=" + WebUtility.UrlEncode("'Strict'");
            }
            else
            {
                query += "&Adult=" + WebUtility.UrlEncode("'Off'");
            }

            string imageFilters = "";

            // Always use Large because Bing always returns microscopic images
            imageFilters += "Size:Large+";

            // Image aspect ratio
            if (minimumImageWHRatio < ASPECT_SQUARE_RANGE_MIN && maximumImageWHRatio < ASPECT_SQUARE_RANGE_MIN)
            {
                imageFilters += "Aspect:Tall+";
            }
            else if (minimumImageWHRatio < ASPECT_WIDE_RANGE_MIN && maximumImageWHRatio < ASPECT_WIDE_RANGE_MIN)
            {
                imageFilters += "Aspect:Square+";
            }
            else if (minimumImageWHRatio >= ASPECT_WIDE_RANGE_MIN && maximumImageWHRatio >= ASPECT_WIDE_RANGE_MIN)
            {
                imageFilters += "Aspect:Wide+";
            }
            else if (minimumImageWHRatio < ASPECT_SQUARE_RANGE_MIN && maximumImageWHRatio < ASPECT_WIDE_RANGE_MIN && maximumImageWHRatio >= ASPECT_SQUARE_RANGE_MIN)
            {
                double prefAvg = (maximumImageWHRatio + minimumImageWHRatio) / 2;
                double tallAvg = ASPECT_SQUARE_RANGE_MIN / 2;
                double squareAvg = (ASPECT_WIDE_RANGE_MIN + ASPECT_SQUARE_RANGE_MIN) / 2;
                if (Math.Abs(prefAvg - tallAvg) < Math.Abs(prefAvg - squareAvg))
                {
                    imageFilters += "Aspect:Tall+";
                }
                else
                {
                    imageFilters += "Aspect:Square+";
                }
            }
            else if (minimumImageWHRatio < ASPECT_WIDE_RANGE_MIN && minimumImageWHRatio >= ASPECT_SQUARE_RANGE_MIN && maximumImageWHRatio >= ASPECT_WIDE_RANGE_MIN)
            {
                double prefAvg = (maximumImageWHRatio + minimumImageWHRatio) / 2;
                double squareAvg = (ASPECT_WIDE_RANGE_MIN + ASPECT_SQUARE_RANGE_MIN) / 2;
                double wideAvg = ASPECT_WIDE_RANGE_MIN + ASPECT_SQUARE_RANGE_MIN / 2;
                if (Math.Abs(prefAvg - wideAvg) < Math.Abs(prefAvg - squareAvg))
                {
                    imageFilters += "Aspect:Wide+";
                }
                else
                {
                    imageFilters += "Aspect:Square+";
                }
            }

            // Colors in image
            switch (colorsInImage)
            {
                case ColorsInImage.FULL_COLOR:
                case ColorsInImage.YELLOW:
                case ColorsInImage.GREEN:
                case ColorsInImage.BLUE:
                case ColorsInImage.PURPLE:
                case ColorsInImage.TEAL:
                case ColorsInImage.PINK:
                case ColorsInImage.WHITE:
                case ColorsInImage.GRAY:
                case ColorsInImage.BLACK:
                case ColorsInImage.BROWN: imageFilters += "Color:Color+"; break;
                case ColorsInImage.BLACK_AND_WHITE: imageFilters += "Color:Monochrome+"; break;
            }

            // Type of image
            switch (typeOfImage)
            {
                case TypeOfImage.FACE: imageFilters += "Face:Face+"; break;
                case TypeOfImage.PORTRAIT: imageFilters += "Face:Portrait+"; break;
                case TypeOfImage.PHOTO: imageFilters += "Style:Photo+"; break;
                case TypeOfImage.CLIP_ART: 
                case TypeOfImage.LINE_DRAWING: imageFilters += "Style:Graphics+"; break;
            }

            // Remove the trailing +
            if (imageFilters.EndsWith("+"))
                imageFilters = imageFilters.Substring(0, imageFilters.Length - 1);

            // Image filters
            query += "&ImageFilters=" + WebUtility.UrlEncode("'" + imageFilters + "'");

            // Set a random start index for a random page
            int startIndex = new Random().Next(951);
            query += "&$skip=" + startIndex;

            return query;
        }

        private List<string> getImageUrls(string json)
        {
            List<string> validUrls = new List<string>();

            // Deserialize the JSON string
            JObject bingImageResults = (JObject) JsonConvert.DeserializeObject(json);

            JObject d = (JObject) bingImageResults["d"];

            // Get the items array
            if (d["results"] != null)
            {
                JArray items = (JArray) d["results"];
                lastSearchResults = items;  // Remember for later

                // Go through each item and validate it, then add the URL to the list
                foreach (JObject item in items)
                {
                    string url = item["MediaUrl"].ToString();

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
                        int width = int.Parse(item["Width"].ToString());
                        int height = int.Parse(item["Height"].ToString());

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
                string url = item["MediaUrl"].ToString();
                if (url.ToLower().Equals(imageUrl.ToLower()))
                {
                    string caption = item["Title"].ToString();
                    return caption;
                }
            }

            Log.warning("Could not get the caption for image at " + imageUrl);
            return null;
        }
    }
}
