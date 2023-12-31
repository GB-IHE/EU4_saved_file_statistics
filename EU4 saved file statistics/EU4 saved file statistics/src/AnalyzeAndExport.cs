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
        private Dictionary<string, Statistics> stats = new Dictionary<string, Statistics>();

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

        /// <summary>
        /// Open the save file and analyze it.
        /// </summary>
        /// <returns>Returns first when all analyses have been run and these have been exported.</returns>
        public async Task runAnalyze()
        {
            const int PROGRESS_LOADING_SAVE_FILE = 10;
            const int PROGRESS_ANALYSIS = 80 + PROGRESS_LOADING_SAVE_FILE;

            // Tell the user that we are starting
            progressText.Report("Work for " + baseOutputFileName + " has been started...");

            // Load the save file
            openSaveFile();
            currentProgress += PROGRESS_LOADING_SAVE_FILE / numberOfFiles;
            addAndReportProgress(currentProgress);

            // Create statistics
            await createStatistic();
            currentProgress += PROGRESS_ANALYSIS / numberOfFiles;
            addAndReportProgress(currentProgress);

            // Export statistics after the analysis is done
            await exportStatistics();
            currentProgress += MAX_PROGRESS / numberOfFiles;
            addAndReportProgress(currentProgress);

            // Tell the user that we are done
            progressText.Report("Work for " + baseOutputFileName + " has been finnished!");
        }

        private void openSaveFile()
        {
            progressText.Report("Opening save file for " + baseOutputFileName + "...");
            saveFile = new SaveFile(saveFilePath);
        }

        /// <summary>
        /// Create statistics for each of the statitics child classes.
        /// </summary>
        /// <returns>Returns when all statistics are created.</returns>
        private Task createStatistic()
        {
            // all analysis to be done
            var tasks = new List<Task>();

            // statistics classes to be created for analysis and export, the key value is the file suffix to the file name
            progressText.Report("Creating statistics for " + baseOutputFileName + "..."); ;
            tasks.Add(Task.Run(() => stats.Add("_province statistics.csv", new ProvinceStatistics(saveFile))));
            tasks.Add(Task.Run(() => stats.Add("_country statistics.csv", new CountryStatistics(saveFile))));
            tasks.Add(Task.Run(() => stats.Add("_player statistics.csv", new PlayerStatistics(saveFile))));

            // all done
            return Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Export statistics for each type of statistics in stats sepeleratly.
        /// </summary>
        /// <returns>Retruns when all statistics has been exported.</returns>
        private Task exportStatistics()
        {
            // all analysis to be done
            var tasks = new List<Task>();

            progressText.Report("Exporting statistics for " + baseOutputFileName + "...");

            // loop each statistic class with its file suffix in the dictornary stats and create ouput for each of the objects
            foreach (var statistics in stats)
            {
                string fileSuffix = statistics.Key.ToString();
                Statistics statObj = statistics.Value;
                tasks.Add(Task.Run(() => new ExportStatistics(outputDirectory, baseOutputFileName, statObj, fileSuffix)));
            }

            // done
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
