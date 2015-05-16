using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FotoBoek.Code;
using System.IO;

namespace JpegMetaData
{
    class Program
    {
        public static bool success = true;

        static void Main(string[] args)
        {
            try
            {
                string fileName = args[0];
                if (args.Length > 3)
                {
                    // Set the metadata

                    string imageUrl = args[1];
                    string searchTerm = args[2];
                    string caption = args[3];

                    Metadata allMetadata = new Metadata(fileName);
                    List<string> metadata = new List<string>(new[]{
                        "url", imageUrl, "term", searchTerm, "caption", caption
                    });
                    allMetadata.Keywords = metadata;
                    allMetadata.Save();

                    Console.WriteLine(Program.success);
                }
                else
                {
                    // Read and return the metadata

                    Metadata allMetadata = new Metadata(fileName);
                    List<string> metadata = allMetadata.Keywords;

                    for (int i = 0; i < metadata.Count; i++)
                    {
                        if (i % 2 == 1)
                        {
                            string value = metadata[i];
                            if (value == null)
                                return;
                            Console.WriteLine(value);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
