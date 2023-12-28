using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Loads the save file, creates statistics for each type of statistics, and exports this to a statistics file.
    /// </summary>
    public class AnalyzeAndExport
    {
        // Input strings used within the class
        private readonly string saveFilePath;
        private readonly string outputDirectory;
        private readonly string baseOutputFileName;

        // Class objects created within the class
        private SaveFile saveFile;
        private ProvinceStatistics provinceStats;
        private CountryStatistics countryStats;

        /// <summary>
        /// Analyze the save file and export the statistics from it;
        /// </summary>
        /// <param name="saveFilePath">Path of the save file to analyze.</param>
        /// <param name="outputDirectory">Path where to save the output files.</param>
        /// <param name="baseOutputFileName">Base name of the output files (suffixes are created later).</param>
        public AnalyzeAndExport(string saveFilePath, string outputDirectory, string baseOutputFileName) 
        {
            this.saveFilePath = saveFilePath;
            this.outputDirectory = outputDirectory;
            this.baseOutputFileName = baseOutputFileName;

            openSaveFile();
            createStatistics();
            exportStatistics();
        }

        private void openSaveFile()
        {
            saveFile = new SaveFile(saveFilePath);
        }

        /// <summary>
        /// Creates statistics from the save file for each type of statistics.
        /// </summary>
        private void createStatistics()
        {
            provinceStats = new ProvinceStatistics(saveFile);
            countryStats = new CountryStatistics(saveFile);
        }

        /// <summary>
        /// Export statistics for each type of statistics.
        /// </summary>
        private void exportStatistics()
        {
            const string OUTPUT_FILE_SUFFIX_PROVINCES = "_province statistics.csv";
            const string OUTPUT_FILE_SUFFIX_COUNTRIES = "_country statistics.csv";

            ExportStatistics exportProvinceStats = new ExportStatistics(outputDirectory, baseOutputFileName, provinceStats, OUTPUT_FILE_SUFFIX_PROVINCES);
            ExportStatistics exportCountryStats = new ExportStatistics(outputDirectory, baseOutputFileName, countryStats, OUTPUT_FILE_SUFFIX_COUNTRIES);
        }
    }
}
