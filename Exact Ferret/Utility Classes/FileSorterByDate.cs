using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exact_Ferret
{
    class FileSorterByDate : IComparer<FileInfo>
    {
        public int Compare(FileInfo a, FileInfo b)
        {
            if (a.LastWriteTime.ToFileTime() > b.LastWriteTime.ToFileTime()) return 1;
            else if (a.LastWriteTime.ToFileTime() < b.LastWriteTime.ToFileTime()) return -1;
            else return 0;
        }
    }
}
