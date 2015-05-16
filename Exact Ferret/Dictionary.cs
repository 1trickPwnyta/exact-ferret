using Exact_Ferret.Settings_Classes;
using Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Language
{
    class Dictionary
    {
        public static string getRandomWord()
        {
            return getRandomWord(null);
        }

        public static string getRandomWord(string dictionaryFile)
        {
            Log.trace("Getting a random word.");

            string[] allWords = null;
            string dictionaryFilePath = dictionaryFile == null? PropertiesManager.getDictionaryPath(): dictionaryFile;
            try
            {
                allWords = File.ReadAllLines(dictionaryFilePath);
            }
            catch (Exception e)
            {
                Log.warning("Could not open the dictionary file at " + dictionaryFilePath +
                        ". Using empty string as dictionary word.");
                return "";
            }

            if (allWords.Length > 0)
            {
                int randomIndex = new Random().Next(allWords.Length);
                string randomWord = allWords[randomIndex];
                Log.trace("Selected dictionary word: " + randomWord + " (dictionary index = " + randomIndex + ")");

                return randomWord;
            }
            else
            {
                Log.warning("The dictionary file has no words. Using empty string as dicionary word.");
                return "";
            }
        }
    }
}
