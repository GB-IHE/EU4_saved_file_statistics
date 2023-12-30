using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Loads the save file, creates statistics for each type of statistics, and exports this to a statistics file. Also keeps track of the progress.
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

        // Progress in frmMain
        IProgress<int> progress;
        IProgress<string> progressText;
        private int currentProgress = 0;
        private readonly int MAX_PROGRESS = 100;
        private readonly int numberOfFiles;

        /// <summary>
        /// Analyze one save file and export the statistics from it.
        /// </summary>
        /// <param name="saveFilePath">Path of the save file to analyze.</param>
        /// <param name="outputDirectory">Path where to save the output files.</param>
        /// <param name="baseOutputFileName">Base name of the output files (suffixes are created later).</param>
        /// <param name="progress">Progressbar on the frmMain.</param>
        /// <param name="numberOfFiles">Number of files to analyze in total (used to weight the progresss).</param>
        /// <param name="progressText">Progress text to show to the user.</param>
        public AnalyzeAndExport(string saveFilePath, string outputDirectory, string baseOutputFileName, 
                                IProgress<int> progress, int numberOfFiles, IProgress<string> progressText) 
        {
            this.saveFilePath = saveFilePath;
            this.outputDirectory = outputDirectory;
            this.baseOutputFileName = baseOutputFileName;
            this.progress = progress;
            this.numberOfFiles = numberOfFiles;
            this.progressText = progressText;
        }

        private void openSaveFile()
        {
            progressText.Report("Opening save file for " + baseOutputFileName + "...");
            saveFile = new SaveFile(saveFilePath);
            //const int PROGRESS_TO_ADD = 10;
            //addAndReportProgress(PROGRESS_TO_ADD);
            progressText.Report("Save file for " + baseOutputFileName + "has been opend...");
        }

        /// <summary>
        /// Export statistics for each type of statistics.
        /// </summary>
        public void exportStatistics()
        {
            progressText.Report("Exporting statistics for " + baseOutputFileName + "...");
            const string OUTPUT_FILE_SUFFIX_PROVINCES = "_province statistics.csv";
            const string OUTPUT_FILE_SUFFIX_COUNTRIES = "_country statistics.csv";

            new ExportStatistics(outputDirectory, baseOutputFileName, provinceStats, OUTPUT_FILE_SUFFIX_PROVINCES);
            new ExportStatistics(outputDirectory, baseOutputFileName, countryStats, OUTPUT_FILE_SUFFIX_COUNTRIES);
            setProgress(MAX_PROGRESS);
        }

        public Task analyze()
        {
            progressText.Report("Work for " + baseOutputFileName + " has started...");
            openSaveFile();
            setProgress(10);

            var tasks = new List<Task>();
            
            // provinces
            var provinceAnalysis = Task.Run(() => provinceStats = new ProvinceStatistics(saveFile));
            tasks.Add(provinceAnalysis);
            progressText.Report("Analyzing province statistics for " + baseOutputFileName + "..."); ;

            // countries
            var countryAnalysis = Task.Run(() => countryStats = new CountryStatistics(saveFile));
            tasks.Add(countryAnalysis);
            progressText.Report("Analyzing country statistics for " + baseOutputFileName + "...");

            return Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Add progress with a specific value.
        /// </summary>
        /// <param name="progressToAdd"></param>
        private void addAndReportProgress(int progressToAdd)
        {
            progressToAdd /= numberOfFiles;
            currentProgress = currentProgress + progressToAdd > MAX_PROGRESS ? MAX_PROGRESS : currentProgress + progressToAdd;
            progress.Report(currentProgress);
        }

        /// <summary>
        /// Set the progress to a fixed value.
        /// </summary>
        /// <param name="totalProgress"></param>
        private void setProgress(int totalProgress)
        {
            totalProgress /= numberOfFiles;
            totalProgress = totalProgress > MAX_PROGRESS ? MAX_PROGRESS : totalProgress;
            progress.Report(totalProgress);
        }
    }
}
