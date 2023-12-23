using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    internal class CreateStatistics
    {

        // create a database with stuff here

        private ProvinceStatistics provinceStats;
        public void createStatistics()
        {
            // province statistics
            provinceStats = new ProvinceStatistics();
        }
    }
}
