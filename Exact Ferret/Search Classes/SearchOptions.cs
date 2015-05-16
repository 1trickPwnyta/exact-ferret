using Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;

namespace Exact_Ferret
{
    class SearchOptions
    {
        public const string GOOGLE = "Google";
        public const string BING = "Bing";

        private List<SearchOption> options = new List<SearchOption>();
        private string searchEngine;
        private string dictionaryFile;

        public void addOption(SearchOption option)
        {
            options.Add(option);
        }

        public void removeOption(int optionType)
        {
            options.Remove(new SearchOption(optionType, null));
        }

        public string getSearchEngine()
        {
            return searchEngine;
        }

        public void setSearchEngine(string searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        public string getDictionaryFile()
        {
            return dictionaryFile;
        }

        public void setDictionaryFile(string dictionaryFile)
        {
            this.dictionaryFile = dictionaryFile;
        }

        public void execute(ISearchEngine engine)
        {
            if (options.Count > 0)
                Log.trace("Overriding search options with options from command line.");

            foreach (SearchOption option in options)
            {
                switch (option.getOptionType())
                {
                    case SearchOption.MINIMUM_WIDTH:
                        int minimumWidth = option.getValueAsInt();
                        Log.trace("Overriding minimum width with " + minimumWidth);
                        engine.setMinimumImageWidth(minimumWidth);
                        break;
                    case SearchOption.MINIMUM_HEIGHT:
                        int minimumHeight = option.getValueAsInt();
                        Log.trace("Overriding minimum height with " + minimumHeight);
                        engine.setMinimumImageHeight(minimumHeight);
                        break;
                    case SearchOption.MAXIMUM_WIDTH:
                        int maximumWidth = option.getValueAsInt();
                        Log.trace("Overriding maximum width with " + maximumWidth);
                        engine.setMaximumImageWidth(maximumWidth);
                        break;
                    case SearchOption.MAXIMUM_HEIGHT:
                        int maximumHeight = option.getValueAsInt();
                        Log.trace("Overriding maximum height with " + maximumHeight);
                        engine.setMaximumImageHeight(maximumHeight);
                        break;
                    case SearchOption.MINIMUM_RATIO:
                        double minimumRatio = option.getValueAsDouble();
                        Log.trace("Overriding minimum ratio with " + minimumRatio);
                        engine.setMinimumWidthHeightRatio(minimumRatio);
                        break;
                    case SearchOption.MAXIMUM_RATIO:
                        double maximumRatio = option.getValueAsDouble();
                        Log.trace("Overriding maximum ratio with " + maximumRatio);
                        engine.setMaximumWidthHeightRatio(maximumRatio);
                        break;
                    case SearchOption.SAFE_SEARCH:
                        bool safeSearch = option.getValueAsBool();
                        Log.trace("Overriding safe search with " + safeSearch);
                        engine.setSafeSearch(safeSearch);
                        break;
                    case SearchOption.SEARCH_DOMAIN:
                        string searchDomain = option.getValue();
                        Log.trace("Overriding search domain with " + searchDomain);
                        engine.setSiteSearchDomain(searchDomain);
                        break;
                    case SearchOption.COLORS_IN_IMAGE:
                        int colorsInImage = option.getValueAsInt();
                        Log.trace("Overriding colors in image with " + colorsInImage);
                        engine.setColorsInImage(colorsInImage);
                        break;
                    case SearchOption.TYPE_OF_IMAGE:
                        int typeOfImage = option.getValueAsInt();
                        Log.trace("Overriding type of image with " + typeOfImage);
                        engine.setTypeOfImage(typeOfImage);
                        break;
                }
            }
        }
    }

    class SearchOption
    {
        public const int MINIMUM_WIDTH = 1;
        public const int MINIMUM_HEIGHT = 2;
        public const int MINIMUM_RATIO = 3;
        public const int MAXIMUM_RATIO = 4;
        public const int SAFE_SEARCH = 5;
        public const int SEARCH_DOMAIN = 6;
        public const int COLORS_IN_IMAGE = 7;
        public const int TYPE_OF_IMAGE = 8;
        public const int MAXIMUM_WIDTH = 9;
        public const int MAXIMUM_HEIGHT = 10;
        public const int SEARCH_ENGINE = 11;
        public const int DICTIONARY = 12;

        private int type;
        private string value;

        public SearchOption(int type, object value)
        {
            this.type = type;
            this.value = value.ToString();
        }

        public int getOptionType()
        {
            return type;
        }

        public string getValue()
        {
            return value;
        }

        public int getValueAsInt()
        {
            return int.Parse(value);
        }

        public double getValueAsDouble()
        {
            return double.Parse(value);
        }

        public bool getValueAsBool()
        {
            return bool.Parse(value);
        }

        public override bool Equals(object other)
        {
            if (other.GetType() == typeof(SearchOption)) {
                return ((SearchOption)other).type == type;
            }
            return false;
        }
    }
}
