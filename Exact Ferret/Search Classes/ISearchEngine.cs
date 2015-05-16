using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search
{
    interface ISearchEngine
    {
        List<string> searchForImages(string searchTerm);
        string getCaptionForImage(string imageUrl);
        bool isApiLimitExceeded();
        void setApiKey(string key);
        void allowImageFileType(string fileType);
        void blockImage(string imageUrl);
        void blockDomain(string domain);
        void blockImages(StringCollection imageUrls);
        void blockDomains(StringCollection domains);
        void setMinimumImageWidth(int width);
        void setMinimumImageHeight(int height);
        void setMinimumImageDimensions(int width, int height);
        void setMaximumImageWidth(int width);
        void setMaximumImageHeight(int height);
        void setMaximumImageDimensions(int width, int height);
        void setMinimumWidthHeightRatio(double minimum);
        void setMaximumWidthHeightRatio(double maximum);
        void setWidthHeightRatioRange(double minimum, double maximum);
        void setSafeSearch(bool safeSearch);
        void setColorsInImage(int colorsInImage);
        void setTypeOfImage(int typeOfImage);
        void setSiteSearchDomain(string siteSearchDomain);
        void setRequestTimeoutSeconds(int seconds);
    }
}
