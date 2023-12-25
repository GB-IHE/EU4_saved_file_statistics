using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    internal class ExportStatistics
    {
        private readonly string path;
        private readonly ProvinceStatistics provinceStatistics;

        ExportStatistics(string path, ProvinceStatistics provinceStatistics)
        {
            this.path = path;
            this.provinceStatistics = provinceStatistics;
        }

        public void createOutputFile()
        {
            // for each province ID, get the struct and print out all the stuff
        }
    }
}
