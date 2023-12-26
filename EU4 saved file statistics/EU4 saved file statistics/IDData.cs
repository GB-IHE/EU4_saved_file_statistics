using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Stores data for a specific province, country etc.
    /// </summary>
    public struct IDData
    {
        // These three are not not printed
        public string id { get; set; }
        public int startLineInTheSaveFile { get; set; } // for the specific ID
        public int endLineInTheSaveFile { get; set; }

        /// <summary>
        /// List with all the things we want to print out for the specific provinde with header of the data and the value, like:
        /// {"Name", "Stockholm"} (where "Name" is the column name, key, and "Stockholm" is the value)
        /// </summary>
        public List<Tuple<string, string>> idStats { get; set; }
}
}
