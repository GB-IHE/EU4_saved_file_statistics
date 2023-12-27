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
        // Class objects created within the class
        private SaveFile saveFile;
        private ProvinceStatistics provinceStats;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFilePath">Path of the save file to analyze.</param>
        /// <param name="outputDirectory">Path where to save the output files.</param>
        /// <param name="baseOutputFileName">Base name of the output files (suffixes are created later).</param>
        public AnalyzeAndExport(string saveFilePath, string outputDirectory, string baseOutputFileName) 
        {
            openSaveFile(saveFilePath);
            createStatistics();
            exportStatistics(outputDirectory, baseOutputFileName);
        }
        private void openSaveFile(string saveFilePath)
        {
            saveFile = new SaveFile(saveFilePath);
        }

        /// <summary>
        /// Creates statistics from the save file for each type of statistics.
        /// </summary>
        private void createStatistics()
        {
            provinceStats = new ProvinceStatistics(saveFile);
        }

        /// <summary>
        /// Export statistics for each type of statistics.
        /// </summary>
        private void exportStatistics(string outputDirectory, string baseOutputFileName)
        {
            ExportProvinceStatistics exportProvinceStats = new ExportProvinceStatistics(outputDirectory, baseOutputFileName, provinceStats);
        }
    }
}
