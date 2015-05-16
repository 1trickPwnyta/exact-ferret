using Exact_Ferret.Settings_Classes;
using FotoBoek.Code;
using Http;
using Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Exact_Ferret
{
    class PictureFileManager
    {
        public const string IMAGE_FILENAME_PREFIX = "$ef$";
        public const string IMAGE_FILENAME_SEPARATOR = "$.$";
        private const string TEMP_FILE_NAME = "ef.temp.jpg";
        public const int FILENAME_LIMIT = 255;

        public static List<FileInfo> getPicturesList()
        {
            // List all the picture files in the picture folder
            string pictureFolderPath = PropertiesManager.getPictureFolderPath();

            DirectoryInfo imageFolder;
            try
            {
                imageFolder = new DirectoryInfo(pictureFolderPath);
                if (!imageFolder.Exists)
                    imageFolder.Create();
            }
            catch (Exception e)
            {
                Log.error("Could not open the image folder " + pictureFolderPath);
                return null;
            }

            FileInfo[] filesInFolder = imageFolder.GetFiles();
            List<FileInfo> imageFiles = new List<FileInfo>();

            // Filter out the non-Exact Ferret files
            foreach (FileInfo fileInFolder in filesInFolder)
            {
                string fileName = fileInFolder.Name;
                if (fileName.StartsWith(IMAGE_FILENAME_PREFIX))
                    imageFiles.Add(fileInFolder);
            }

            return imageFiles;
        }

        public static List<FileInfo> getPicturesListSortedByTimestamp()
        {
            List<FileInfo> imageFiles = getPicturesList();

            // Sort the picture files by date
            if (imageFiles != null)
                imageFiles.Sort(new FileSorterByDate());

            return imageFiles;
        }

        public static string downloadImage(string url, string searchTermUsed, string caption)
        {
            if (url == null || url == "")
            {
                Log.error("Cannot download an image from a null or empty URL.");
                return null;
            }

            Log.trace("Downloading an image.");

            string urlFileName = url.Substring(url.LastIndexOf('/') + 1);
            string imageFileName = urlFileName;

            Log.trace("Matching the referer header to the domain.");
            HttpUtil.setReferer(url);

            HttpUtil.setRequestTimeoutSeconds(PropertiesManager.getHttpRequestTimeoutSeconds());

            List<FileInfo> imageFiles = getPicturesListSortedByTimestamp();
            Log.trace("Found " + imageFiles.Count + " old images in the folder.");

            int maxOldImages = PropertiesManager.getMaxOldImageFiles();

            try
            {
                for (int i = 0; i < imageFiles.Count - maxOldImages; i++)
                {
                    FileInfo file = imageFiles[i];
                    Log.trace("Deleting old image file " + file.FullName);
                    deleteFile(file.FullName);
                }
            }
            catch (Exception e)
            {
                Log.warning("Could not delete old image files in the folder.");
            }

            // Get the path for the image plus filename prefix
            string imageFilePath = PropertiesManager.getPictureFolderPath() +
                    "\\" + IMAGE_FILENAME_PREFIX + DateTime.Now.Ticks;

            // Check if this path is too long
            if (imageFilePath.Length > FILENAME_LIMIT)
            {
                Log.error("Can't save the image because the picture folder path is too long.");
                return null;
            }

            // If the file name is too long, attempt to shorten it
            if (imageFilePath.Length + imageFileName.Length > FILENAME_LIMIT)
            {
                imageFileName = imageFileName.Substring(imageFileName.Length - (FILENAME_LIMIT - imageFilePath.Length));
                if (!imageFileName.Contains("."))
                {
                    Log.error("Can't save the image because the file extension is too long.");
                    return null;
                }
            }

            // Add the file name to the path
            imageFilePath += imageFileName;

            Log.trace("Saving the image to " + imageFilePath);

            if (!HttpUtil.downloadFile(url, imageFilePath))
            {
                Log.warning("Could not save the image file to " + imageFilePath);
                return null;
            }

            if (!setPictureMetadata(imageFilePath, url, searchTermUsed, caption))
                return null;

            return imageFilePath;
        }

        public static string getUrlFromPicturePath(string picturePath)
        {
            if (picturePath == null)
                return null;
            
            Hashtable metadata = getPictureMetadata(picturePath);
            if (metadata != null && metadata["url"] != null)
            {
                string url = metadata["url"].ToString();
                return url;
            }
            else
            {
                string name = new FileInfo(picturePath).Name;
                string url = null;
                if (name.Contains(IMAGE_FILENAME_SEPARATOR))
                {
                    url = name.Substring(IMAGE_FILENAME_PREFIX.Length,
                            name.IndexOf(IMAGE_FILENAME_SEPARATOR) - IMAGE_FILENAME_PREFIX.Length);
                    return Base64Util.Base64Decode(url);
                }
                else
                    return null;
            }
        }

        public static string getDomainFromPicturePath(string picturePath)
        {
            if (picturePath == null)
                return null;

            string url = getUrlFromPicturePath(picturePath);
            if (url != null) {
                string pictureDomain = new Uri(url).Host.ToLower();
                string[] domainParts = pictureDomain.Split('.');
                try {
                    pictureDomain = domainParts[domainParts.Length - 2] + "." + domainParts[domainParts.Length - 1];
                } catch {}
                return pictureDomain;
            }
            else
                return null;
        }

        public static string getSearchTermFromPicturePath(string picturePath)
        {
            if (picturePath == null)
                return null;

            Hashtable metadata = getPictureMetadata(picturePath);
            if (metadata != null && metadata["term"] != null)
            {
                string term = metadata["term"].ToString();
                return term;
            }
            else
                return null;
        }

        public static string getCaptionFromPicturePath(string picturePath)
        {
            if (picturePath == null)
                return null;

            Hashtable metadata = getPictureMetadata(picturePath);
            if (metadata != null && metadata["caption"] != null)
            {
                string caption = metadata["caption"].ToString();
                return caption;
            }
            else
                return null;
        }

        public static void deleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        private static Hashtable getPictureMetadata(string imageFileName)
        {
            string metadataExePath = PropertiesManager.getInstallationDirectoryPath() + "\\" + PropertiesManager.METADATA_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = metadataExePath;
            process.StartInfo.Arguments = "\"" + imageFileName + "\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();

            if (output.Trim() == "")
                return null;

            string[] lines = output.Split('\n');

            if (lines.Length < 3)
                return null;

            string imageUrl = Base64Util.Base64Decode(lines[0]);
            string searchTerm = Base64Util.Base64Decode(lines[1]);
            string caption = Base64Util.Base64Decode(lines[2]);

            if (imageUrl == null || searchTerm == null || caption == null)
                return null;

            Hashtable metadataTable = new Hashtable();
            metadataTable.Add("url", imageUrl);
            metadataTable.Add("term", searchTerm);
            metadataTable.Add("caption", caption);

            return metadataTable;
        }

        private static bool setPictureMetadata(string imageFileName, string imageUrl, string searchTerm, string caption)
        {
            // Need to open and re-save the image so any current metadata is erased
            string tempPath = PropertiesManager.getPictureFolderPath() + "\\" + TEMP_FILE_NAME;
            try
            {
                using (Bitmap image = new Bitmap(imageFileName))
                {
                    image.Save(tempPath);
                    image.Dispose();
                }
                using (Bitmap image = new Bitmap(tempPath))
                {
                    image.Save(imageFileName);
                    image.Dispose();
                }
                File.Delete(tempPath);
            }
            catch
            {
                Log.warning("The file is not a valid image.");
                File.Delete(tempPath);
                File.Delete(imageFileName);
                return false;
            }

            string metadataExePath = PropertiesManager.getInstallationDirectoryPath() + "\\" + PropertiesManager.METADATA_EXECUTABLE_NAME;
            Process process = new Process();
            process.StartInfo.FileName = metadataExePath;
            process.StartInfo.Arguments = "\"" + imageFileName + "\" \"" + Base64Util.Base64Encode(imageUrl) + "\" \"" + Base64Util.Base64Encode(searchTerm) + "\" \"" + Base64Util.Base64Encode(caption) + "\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();

            string output = process.StandardOutput.ReadToEnd();
            if (output.Trim().Equals("True"))
                return true;
            else
            {
                Log.warning("Could not set the metadata. The error was: " + output.Trim());
                File.Delete(imageFileName);
                return false;
            }
        }
    }
}
