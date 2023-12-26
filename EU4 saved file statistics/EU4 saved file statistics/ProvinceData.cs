using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    public struct ProvinceData
    {
        public int id { get; set; }
        public int startLineInTheSaveFile { get; set; }
        public int endLineInTheSaveFile { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public string controler { get; set; }
        public string religion { get; set; }
        public string culture { get; set; }
    }
}
