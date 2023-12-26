using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/**
 * Stores the save file and make it usable for all the child classes (different statistics)
 * Stores all the statistic classes (child classes) that we create when we run createStatistics
 */

namespace EU4_saved_file_statistics
{
    public class Statistics
    {
        internal readonly SaveFile saveFile;
        private ProvinceStatistics provinceStats;

        public Statistics(SaveFile saveFile)
        {
            this.saveFile = saveFile;
        }

        // get the province statistics subclass only (used when we print out the province statstics)
        public ProvinceStatistics getProvinceStatistics() { return provinceStats; }

        // created all the statitics
        public void createStatistics()
        {
            // province statistics
            provinceStats = new ProvinceStatistics(saveFile);
        }
    }
}