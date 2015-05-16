using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exact_Ferret.Utility_Classes
{
    class CollectionUtil
    {
        public static List<string> convertSelectedObjectsCollectionToStringList(System.Windows.Forms.ListBox.SelectedObjectCollection c)
        {
            List<string> strings = new List<string>();
            foreach (object o in c)
            {
                string s = o.ToString();
                strings.Add(s);
            }
            return strings;
        }
    }
}
