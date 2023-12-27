using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Expoert statistics for provinces
    /// </summary>
    internal class ExportProvinceStatistics : ExportStatistics
    {
        /// <summary>
        /// Creates a file with the province statistics.
        /// </summary>
        /// <param name="outputFolder">The folder for the output file.</param>
        /// <param name="baseOutputFileName">The name of the output file, a suffix is added based on the type of statistics.</param>
        /// <param name="statistics">The analyzed statistics that is to be printed.</param>
        public ExportProvinceStatistics(string outputFolder, string baseOutputFileName, Statistics statistics) : base(outputFolder, baseOutputFileName, statistics)
        {
            const string OUTPUT_FILE_SUFFIX = "_province statistics.csv";
            string filePath = outputFolder + @"\" + baseOutputFileName + OUTPUT_FILE_SUFFIX;
            deleteOldOutputFiles(filePath);

            StreamWriter textWriter = new StreamWriter(filePath, APPEND, System.Text.Encoding.GetEncoding("iso-8859-1"));
            CsvWriter outputFile = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
            Statistics provinceStatistics = statistics;

            Dictionary<string, IDData> proviceStatisticsData = provinceStatistics.getStatisticsData(); // statistics for all province IDs

            printHeaders(provinceStatistics, outputFile, proviceStatisticsData);
            printStatisticsForEachID(provinceStatistics, outputFile);
            textWriter.Close();
        } // end consturtor
    } // end class
} // end namespace