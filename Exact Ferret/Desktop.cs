using Exact_Ferret.Settings_Classes;
using Exact_Ferret.Utility_Classes;
using Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Foundation;
using Windows.Storage;
using Windows.System.UserProfile;

namespace Exact_Ferret
{
    class Desktop
    {
        public enum WallpaperStyle : int
        {
            Tile, Center, Stretch, Fit, Fill, NoChange
        }

        // Wallpaper changing codes
        private const int SET_DESKTOP_WALLPAPER_CODE = 20;
        private const int UPDATE_INI_FILE_CODE = 0x01;
        private const int SEND_WIN_INI_CHANGE_CODE = 0x02;
        private const string USER_32_DLL = "user32.dll";

        public const string CURRENT_PICTURE_POINTER_FILE_NAME = "ef.current";
        private const int WINDOWS_7_MINIMUM_IMAGE_DIMENSION = 100;
        private const double MIN_IMAGE_SCREEN_RATIO_FOR_STRETCH = 0.9;
        private const double MAX_IMAGE_SCREEN_RATIO_FOR_STRETCH = 1.1;
        private const double MIN_IMAGE_SCREEN_RATIO_FOR_FILL = 0.75;
        private const double MAX_IMAGE_SCREEN_RATIO_FOR_FILL = 1.25;
        private const double MIN_IMAGE_SCREEN_RATIO_FOR_FIT = 0.6;
        private const double MIN_IMAGE_SCREEN_RATIO_FOR_CENTER = 0.4;
        private const double MAX_IMAGE_RATIO_SCREEN_RATIO_DIFF = 0.3;
        private const string WINDOWS_7_LOCK_SCREEN_PATH = "\\\\localhost\\efw7\\backgroundDefault.jpg";
        private const string CONVERT_FILE_NAME = "convert.exe";
        private const int WINDOWS_7_MAX_LOCK_SCREEN_FILE_SIZE_BYTES = 250 * 1024;
        private const string DESKTOP_REG_KEY = "Control Panel\\Desktop";
        private const int RESIZE_INTERVAL_PERCENT = 80;

        [DllImport(USER_32_DLL, SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public static bool changePicture(string path, bool panic)
        {
            if (!panic && (path == null || path.Equals("")))
            {
                Log.error("Could not change the picture with a null or empty path.");
                return false;
            }

            if (!panic && !File.Exists(path))
            {
                Log.error("Could not change the picture because the path " + path + " doesn't exist.");
                return false;
            }

            if (!panic)
                updatePointerFile(path);

            // Check the validity of the image
            if (panic && !path.Equals(""))
            {
                try
                {
                    using (Image img = new Bitmap(path))
                    {
                        img.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Log.warning("The image at " + path + " is not a valid image file.");
                    path = "";
                }
            }

            bool changeSuccess = true;
            if (PropertiesManager.getChangeWallpaperEnabled())
            {
                Desktop.WallpaperStyle style;
                int wallpaperStyle = PropertiesManager.getWallpaperStyle();
                switch (wallpaperStyle)
                {
                    case 0:
                        if (panic)
                        {
                            // When panicking, don't fiddle with this option, just stretch
                            style = Desktop.WallpaperStyle.Stretch;
                            break;
                        }

                        // Automatically determine the best style

                        Log.trace("Determining the best wallpaper style.");

                        // Get the screen dimensions
                        int screenWidth, screenHeight;
                        try
                        {
                            screenWidth = Screen.AllScreens[0].Bounds.Width;
                            screenHeight = Screen.AllScreens[0].Bounds.Height;
                            Log.trace("Screen dimensions: " + screenWidth + "x" + screenHeight);
                        }
                        catch (Exception e)
                        {
                            Log.error("Could not determine the screen dimensions.");
                            style = WallpaperStyle.NoChange;
                            break;
                        }

                        // Get the image dimensions
                        int imageWidth, imageHeight;
                        try
                        {
                            using (Image img = new Bitmap(path))
                            {
                                imageWidth = img.Width;
                                imageHeight = img.Height;
                                img.Dispose();
                            }
                            Log.trace("Image dimensions: " + imageWidth + "x" + imageHeight);
                        }
                        catch (Exception e)
                        {
                            Log.warning("Could not determine the wallpaper style to use because the image at " + path +
                                    " could not be read.");
                            style = WallpaperStyle.NoChange;
                            break;
                        }

                        // Compare image dimensions to screen dimensions
                        double widthRatio = (double)imageWidth / screenWidth;
                        double heightRatio = (double)imageHeight / screenHeight;
                        double imageRatio = (double)imageWidth / imageHeight;
                        double screenRatio = (double)screenWidth / screenHeight;
                        Log.trace("Image:Screen ratio: " + widthRatio + "x" + heightRatio);

                        if (widthRatio >= MIN_IMAGE_SCREEN_RATIO_FOR_STRETCH && heightRatio >=
                                MIN_IMAGE_SCREEN_RATIO_FOR_STRETCH && widthRatio <=
                                MAX_IMAGE_SCREEN_RATIO_FOR_STRETCH && heightRatio <=
                                MAX_IMAGE_SCREEN_RATIO_FOR_STRETCH &&
                                1 - imageRatio / screenRatio < MAX_IMAGE_RATIO_SCREEN_RATIO_DIFF)
                            style = Desktop.WallpaperStyle.Stretch;
                        else if (widthRatio >= MIN_IMAGE_SCREEN_RATIO_FOR_FILL && heightRatio >=
                                MIN_IMAGE_SCREEN_RATIO_FOR_FILL && widthRatio <=
                                MAX_IMAGE_SCREEN_RATIO_FOR_FILL && heightRatio <=
                                MAX_IMAGE_SCREEN_RATIO_FOR_FILL)
                            style = Desktop.WallpaperStyle.Fill;
                        else if (widthRatio >= MIN_IMAGE_SCREEN_RATIO_FOR_FIT || heightRatio >=
                                MIN_IMAGE_SCREEN_RATIO_FOR_FIT)
                            style = Desktop.WallpaperStyle.Fit;
                        else if (widthRatio >= MIN_IMAGE_SCREEN_RATIO_FOR_CENTER || heightRatio >=
                            MIN_IMAGE_SCREEN_RATIO_FOR_CENTER)
                            style = Desktop.WallpaperStyle.Center;
                        else
                            style = Desktop.WallpaperStyle.Tile;

                        break;

                    case 1: style = Desktop.WallpaperStyle.Stretch; break;
                    case 2: style = Desktop.WallpaperStyle.Tile; break;
                    case 3: style = Desktop.WallpaperStyle.Center; break;
                    case 4: style = Desktop.WallpaperStyle.Fit; break;
                    case 5: style = Desktop.WallpaperStyle.Fill; break;
                    default: style = Desktop.WallpaperStyle.NoChange; break;
                }
                Log.trace("Setting wallpaper style to " + Enum.GetName(typeof(Desktop.WallpaperStyle), style));

                changeSuccess &= Desktop.changeWallpaper(path, style, panic);
            }
            
            if (PropertiesManager.getChangeLockScreenEnabled())
            {
                changeSuccess &= Desktop.changeLockScreen(path, panic);
            }

            return changeSuccess;
        }

        public static bool changePicture(string path)
        {
            return changePicture(path, false);
        }

        private static bool changeWallpaper(string path, WallpaperStyle style, bool panic)
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(DESKTOP_REG_KEY, true))
                {
                    switch (style)
                    {
                        case WallpaperStyle.Stretch:
                            key.SetValue(@"WallpaperStyle", "2");
                            key.SetValue(@"TileWallpaper", "0");
                            break;
                        case WallpaperStyle.Center:
                            key.SetValue(@"WallpaperStyle", "1");
                            key.SetValue(@"TileWallpaper", "0");
                            break;
                        case WallpaperStyle.Tile:
                            key.SetValue(@"WallpaperStyle", "1");
                            key.SetValue(@"TileWallpaper", "1");
                            break;
                        case WallpaperStyle.Fit:
                            key.SetValue(@"WallpaperStyle", "6");
                            key.SetValue(@"TileWallpaper", "0");
                            break;
                        case WallpaperStyle.Fill:
                            key.SetValue(@"WallpaperStyle", "10");
                            key.SetValue(@"TileWallpaper", "0");
                            break;
                        case WallpaperStyle.NoChange:
                            break;
                    }
                    key.Close();
                }
            }
            catch (Exception e)
            {
                Log.error("Could not set the wallpaper style.");
            }

            if (path.Equals(""))
            {
                Log.trace("Removing the desktop wallpaper.");
                path = "";    // To remove the wallpaper
            }

            // Set the wallpaper now
            int result = SystemParametersInfo(SET_DESKTOP_WALLPAPER_CODE, 0, path, 
                    UPDATE_INI_FILE_CODE | SEND_WIN_INI_CHANGE_CODE);

            // Check the result
            if (result == 0)    // 0 means there was an error
            {
                Log.error("Failed to set the desktop wallpaper from " + path + " (error code " + Marshal.GetLastWin32Error() + ")");
                if (panic && !path.Equals(""))  // Still have to get the wallpaper changed
                    changeWallpaper("", style, true);
                return false;
            }

            Log.trace("The desktop wallpaper was changed successfully.");

            return true;
        }

        private static bool changeLockScreen(string path, bool panic)
        {
            WindowsVersion windowsVersion = OSUtil.getWindowsVersion();

            if (windowsVersion == WindowsVersion.Windows7_Or_Server2008R2)
            {
                Log.trace("Setting the lock screen background for Windows 7.");


                if (!path.Equals(""))
                {
                    // Move (convert.exe) the image to the lock screen folder
                    Log.trace("Converting (moving) image to the lock screen folder.");

                    string folderOutput = runConvert("\"" + path + "\" \"" + WINDOWS_7_LOCK_SCREEN_PATH + "\"");

                    if (folderOutput != null || !File.Exists(WINDOWS_7_LOCK_SCREEN_PATH))
                    {
                        Log.error("An error occurred in setting the lock screen background: " + folderOutput);
                        return false;
                    }
                    else
                    {
                        Log.trace("The image was successfully moved to the lock screen folder.");
                    }

                    // Shrink the image until it is small enough for the lock screen
                    Log.trace("Adjusting the image size under " + WINDOWS_7_MAX_LOCK_SCREEN_FILE_SIZE_BYTES + " bytes.");
                    FileInfo lockScreenFileInfo = new FileInfo(WINDOWS_7_LOCK_SCREEN_PATH);
                    while (lockScreenFileInfo.Length > WINDOWS_7_MAX_LOCK_SCREEN_FILE_SIZE_BYTES)
                    {
                        bool validImage = true;
                        try
                        {
                            // Check the image dimensions
                            int width, height;
                            using (Bitmap image = new Bitmap(WINDOWS_7_LOCK_SCREEN_PATH))
                            {
                                width = image.Width;
                                height = image.Height;
                                image.Dispose();
                            }
                            if (width < WINDOWS_7_MINIMUM_IMAGE_DIMENSION || height < WINDOWS_7_MINIMUM_IMAGE_DIMENSION)
                            {
                                Log.warning("Could not shrink the image enough for the lock screen.");
                                validImage = false;
                            }
                        }
                        catch (Exception e)
                        {
                            Log.warning("The image file is invalid.");
                            validImage = false;
                        }

                        if (!validImage)
                        {
                            return false;
                        }

                        // Shrink the image
                        string shrinkOutput = runConvert("\"" + WINDOWS_7_LOCK_SCREEN_PATH + "\" -resize " +
                                RESIZE_INTERVAL_PERCENT + "% \"" + WINDOWS_7_LOCK_SCREEN_PATH + "\"");

                        if (shrinkOutput != null)
                        {
                            Log.error("An error occurred in setting the lock screen background: " + shrinkOutput);
                            return false;
                        }

                        lockScreenFileInfo.Refresh();
                    }
                }
                else
                {
                    // Remove the lock screen background
                    Log.trace("Removing the Windows 7 lock screen background.");
                    try
                    {
                        File.Delete(WINDOWS_7_LOCK_SCREEN_PATH);
                    }
                    catch
                    {
                        Log.error("Couldn't remove the Windows 7 lock screen background.");
                        return false;
                    }
                }

                Log.trace("The Windows 7 lock screen background was changed successfully.");
            }
            else if (windowsVersion >= WindowsVersion.Windows8_Or_Server2012)
            {
                Log.trace("Setting the lock screen background for Windows 8 or higher.");

                string windows8TempFilePath = Environment.ExpandEnvironmentVariables(
                        "%USERPROFILE%\\Downloads\\" + PictureFileManager.IMAGE_FILENAME_PREFIX + "temp.jpg");

                // If in panic mode and the file doesn't exist, remove the background on purpose
                if (panic && !File.Exists(path))
                {
                    path = "";
                }
                if (path.Equals(""))
                {
                    Log.trace("Removing the Windows 8 lock screen background.");
                    path = PropertiesManager.getInstallationDirectoryPath() + "\\blank.jpg";
                }

                // Delete the temp file from before, if it's still there
                if (File.Exists(windows8TempFilePath))
                    File.Delete(windows8TempFilePath);

                bool success = false;
                try
                {
                    SetBackgroundTask(path).GetAwaiter().GetResult();
                    success = true;
                }
                catch (Exception e)
                {
                    Log.error("An error occurred in setting the lock screen background: " + e.GetType() + " " + e.StackTrace);
                }

                Log.trace("Deleting the temporary image copy.");
                File.Delete(windows8TempFilePath);
                
                if (!success)
                    return false;

                Log.trace("The Windows 8 lock screen background was changed successfully.");
            }
            else
            {
                Log.error("Could not change the lock screen background because this version of Windows is not supported.");
                return false;
            }

            return true;
        }

        private async static Task SetBackgroundTask(string FilePath)
        {
            Log.trace("Creating a temporary image copy in the downloads folder.");

            string windows8TempFileName = PictureFileManager.IMAGE_FILENAME_PREFIX + "temp.jpg";
            StorageFile TempFile = await Windows.Storage.DownloadsFolder.CreateFileAsync(windows8TempFileName);

            Stream FileStream = File.OpenRead(FilePath);
            Stream TempStream = await TempFile.OpenStreamForWriteAsync();
            FileStream.CopyTo(TempStream);
            TempStream.Close();
            FileStream.Close();

            await LockScreen.SetImageFileAsync(TempFile);
        }

        private static string runConvert(string arguments)
        {
            Process convertProcess = new Process();
            convertProcess.StartInfo.FileName = PropertiesManager.getInstallationDirectory() + "\\" + CONVERT_FILE_NAME;
            convertProcess.StartInfo.Arguments = arguments;
            convertProcess.StartInfo.UseShellExecute = false;
            convertProcess.StartInfo.RedirectStandardError = true;
            convertProcess.StartInfo.CreateNoWindow = true;
            convertProcess.Start();

            string errorOutput = convertProcess.StandardError.ReadToEnd();
            convertProcess.WaitForExit();

            if (!errorOutput.Equals(""))
                return errorOutput;
            else
                return null;
        }

        private static void updatePointerFile(string newPath)
        {
            Log.trace("Updating " + CURRENT_PICTURE_POINTER_FILE_NAME + " file with current picture.");
            string pointerFilePath = PropertiesManager.getPictureFolderPath() + "\\" + CURRENT_PICTURE_POINTER_FILE_NAME;
            if (newPath != null)
                File.WriteAllText(pointerFilePath, newPath);
        }

        public static string getCurrentPicturePath()
        {
            string path;
            string metadataFilePath = PropertiesManager.getPictureFolderPath() + "\\" + CURRENT_PICTURE_POINTER_FILE_NAME;
            try
            {
                path = File.ReadAllText(metadataFilePath);
            }
            catch (Exception e)
            {
                path = null;
            }

            return path;
        }

        public static string getLatestPicturePath()
        {
            List<FileInfo> imageFiles = PictureFileManager.getPicturesListSortedByTimestamp();

            if (imageFiles != null && imageFiles.Count > 0)
            {
                return imageFiles[imageFiles.Count - 1].FullName;
            }
            else
            {
                return null;
            }
        }

        public static string getRandomPicturePath()
        {
            List<FileInfo> imageFiles = PictureFileManager.getPicturesList();

            if (imageFiles.Count > 0)
            {
                return imageFiles[new Random().Next(imageFiles.Count)].FullName;
            }
            else
            {
                return null;
            }
        }
    }
}
