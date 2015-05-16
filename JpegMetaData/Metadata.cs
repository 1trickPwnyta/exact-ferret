using System.Collections.ObjectModel;
using System.Threading;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System;
using System.Linq;
using JpegMetaData;

// Required .NET 3.0 Assembly References: WindowsBase.dll, PresentationCore.dll

// Description:  Simple Class that allows you to read and write basic JPEG metadata.
// Author:       Marcel Wijnands
// Blog:         http://blog.shynet.nl/
// References:   Based on an article by Robert A. Wlodarczyk (http://blogs.msdn.com/rwlodarc/archive/2007/07/18/using-wpf-s-inplacebitmapmetadatawriter.aspx)
namespace FotoBoek.Code
{
    public class Metadata
    {
        private bool _saved;
        private string _filePath;

        private string _title;
        private string _subject;
        private int _rating;
        private List<string> _keywords;
        private string _comments;

        public string Title { get; set; }
        public string Subject { get; set; }
        public int Rating { get; set; }
        public List<string> Keywords { get; set; }
        public string Comments { get; set; }

        public Metadata(string path)
        {
            this.load(path);
        }

        public bool Save()
        {
            try
            {
                using (var jpegStream = new FileStream(_filePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    JpegBitmapDecoder jpgDecoder = new JpegBitmapDecoder(jpegStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    var jpegFrames = jpgDecoder.Frames[0];
                    var jpgInplace = jpegFrames.CreateInPlaceBitmapMetadataWriter();

                    jpgInplace.Title = this.Title;
                    jpgInplace.Subject = this.Subject;
                    jpgInplace.Rating = this.Rating;
                    jpgInplace.Keywords = new ReadOnlyCollection<string>(this.Keywords);
                    jpgInplace.Comment = this.Comments;

                    _saved = false;

                    if (jpgInplace.TrySave())
                    {
                        jpegStream.Close();
                        _saved = true;
                    }
                    else
                    {
                        jpegStream.Close();

                        var start = new ThreadStart(padAndSave);
                        var myThread = new Thread(start);

                        myThread.SetApartmentState(ApartmentState.STA);
                        myThread.Start();

                        while (myThread.IsAlive)
                        {

                        }
                    }
                }
            }
            catch
            {
                _saved = false;
            }

            return _saved;
        }

        private void load(string path)
        {
            using (var jpegStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var jpgDecoder = BitmapDecoder.Create(jpegStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                if (jpgDecoder.CodecInfo.FileExtensions.Contains("jpg"))
                {
                    var jpegFrames = jpgDecoder.Frames[0];
                    var myMetaData = (BitmapMetadata)jpegFrames.Metadata;

                    _filePath = path;

                    this.Title = myMetaData.Title ?? string.Empty;
                    this.Subject = myMetaData.Subject ?? string.Empty;
                    this.Rating = myMetaData.Rating;
                    this.Keywords = myMetaData.Keywords == null ? new List<string>() : myMetaData.Keywords.ToList();

                    this.Comments = myMetaData.Comment ?? string.Empty;
                }
                else
                {
                    throw new ArgumentException("File is not a JPEG.");
                }
            }
        }
        private void padAndSave()
        {
            try
            {
                var myJpgTmpFile = Path.GetTempFileName();

                using (var myJpegStream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
                {
                    JpegBitmapDecoder myJpgDecoder = new JpegBitmapDecoder(myJpegStream, BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile, BitmapCacheOption.None);
                    JpegBitmapEncoder myJpgEncoder = new JpegBitmapEncoder();
                    var myJpgPadding = (uint)4096;

                    if (myJpgDecoder.Frames[0] != null && myJpgDecoder.Frames[0].Metadata != null)
                    {
                        var myMetaData = (BitmapMetadata)myJpgDecoder.Frames[0].Metadata.Clone();

                        myMetaData.SetQuery("/app1/ifd/PaddingSchema:Padding", myJpgPadding);
                        myMetaData.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", myJpgPadding);
                        myMetaData.SetQuery("/xmp/PaddingSchema:Padding", myJpgPadding);

                        myMetaData.Title = this.Title;
                        myMetaData.Subject = this.Subject;
                        myMetaData.Rating = this.Rating;
                        myMetaData.Keywords = new ReadOnlyCollection<string>(this.Keywords);
                        myMetaData.Comment = this.Comments;

                        myJpgEncoder.Frames.Add(BitmapFrame.Create(myJpgDecoder.Frames[0], myJpgDecoder.Frames[0].Thumbnail, myMetaData, myJpgDecoder.Frames[0].ColorContexts));
                    }

                    using (var myTempStream = File.Open(myJpgTmpFile, FileMode.Create, FileAccess.ReadWrite))
                    {
                        myJpgEncoder.Save(myTempStream);
                    }
                }

                File.Copy(myJpgTmpFile, _filePath, true);

                try
                {
                    File.Delete(myJpgTmpFile);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                Program.success = false;
                Console.WriteLine(ex.ToString());
            }
        }
    }
}