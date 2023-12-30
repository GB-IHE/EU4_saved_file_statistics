using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

/**
* @author Gunnar Brådvik
* E-mail: gunnar@bradvik.se
*
* @2023
* 
* We have the followinig classes:
*   AnalyzeAndExport - Runs the other classes and child classes based on the user input
*       
*   SaveFile - loads the save file line by line, constructor filePath
*   
*   Statistics - an abstract class that creates statistics for a specific filePath, constructor SaveFile
*       IDData is a struct where the data for each ID (province ID, country tag etc) are stored - used by each statistics class
*       
*       ProvinceStatistics - a child class that creates statistics regarding the provinces in the save file (uses the province ID as key/ID)
*       CountryStatistics - a child class that creates statistics regarding the countries in the save file (uses the country tag as key/ID)
*          
*   ExportStatistics - a class that exports statistics to a specific folder for all the statistics we want to analyze constructed by outputFolder, outputFileName, Statistics, and the file suffix)
*       
* Note that current output file will be deleted and new output files will be created in the same folder as the save files.
* If we want to analyze something new will need to:
*   1) Create a new child class of statisticts wher we find the start line of the class and add analyze methods for what we want to check
*   2) Add the class object of this new class in the AnalyzeAndExport createStatistics() method to the Dictonary stats
*/

// https://stackoverflow.com/questions/68652535/using-iprogress-when-reporting-progress-for-async-await-code-vs-progress-bar-con
// https://stackoverflow.com/questions/19768718/updating-progressbar-external-class
// https://stackoverflow.com/questions/22699048/why-does-task-waitall-not-block-or-cause-a-deadlock-here (this one is really good)

// we have to make the user choce an output folder - now the files are saved in the same folder as the save file
// generate sensible output file names

namespace EU4_saved_file_statistics
{
    public partial class frmMain : Form
    {
        private readonly string SAVE_FILE_EXTENSION = ".eu4";
        public Progress<int> progress;
        public IProgress<string> progressText;

        public frmMain()
        {
            InitializeComponent();

            // Enable drag and drop
            lstFiles.DragDrop += lstFiles_DragDrop;
            lstFiles.DragEnter += lstFiles_DragEnter;

            // Kepps track of the progress
            progress = new Progress<int>(value => prgTotalProgress.Value = value);
            progressText = new Progress<string>(s => lstFiles.Items.Add(DateTime.Now + ": " + s));

            // run debug if we are in dubug mode
            //if (System.Diagnostics.Debugger.IsAttached)
            //    debug();
        }

        private async void debug()
        {
            string currDirectory = Directory.GetCurrentDirectory();
            string outputFolder = Path.GetFullPath(Path.Combine(currDirectory, @"..\..\..\..\", "Save files"));
            string saveFileName = @"\mp_Crimea1553_01_01 non compressed.eu4";
            string filePathSaveFile = outputFolder + saveFileName;
            string baseOutputFileName = Path.GetFileNameWithoutExtension(filePathSaveFile);

            var analysis = new AnalyzeAndExport(filePathSaveFile, outputFolder, baseOutputFileName, 
                                                progress, 1, progressText);

            await analysis.runAnalyze();

            // done
            MessageBox.Show("Files exported!", "Yippie");
            resetProgress();
        }

        private async void btnExportStats_Click(object sender, EventArgs e)
        {
            if (lstFiles.Items.Count == 0)
                Message("Error", "No file has been selected. Press ok to close this dialog and try again.");
            else
            {
                string[] saveFiles = lstFiles.Items.OfType<string>().ToArray();
                await analyzeSaveFiles(saveFiles);
                Refresh();
                runMode(false); // runMode(True) is within analyzeSaveFiles since it does not work otherwise
            } 
        }

        private void btnBrowseFiles_Click(object sender, EventArgs e)
        {
            //https://www.c-sharpcorner.com/UploadFile/mahesh/openfiledialog-in-C-Sharp/
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = true,
                ReadOnlyChecked = true,
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Browse for an input file",
                Filter = SAVE_FILE_EXTENSION + " files (*." + SAVE_FILE_EXTENSION + ")|*" + SAVE_FILE_EXTENSION
            };

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                foreach (string file in openFileDialog1.FileNames) // Add each file to the lstbox
                    CheckandAddFiles(file);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            EnableRunOptions(false); //no files, no way to run the program
            lstFiles.Items.Clear();
        }

        // lst things
        private void lstFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && btnBrowseFiles.Enabled == true)
                if (lstFiles.SelectedItems.Count != 0)
                    while (lstFiles.SelectedIndex != -1)
                        lstFiles.Items.RemoveAt(lstFiles.SelectedIndex);

            if (lstFiles.Items.Count == 0) // can't run when empty
                EnableRunOptions(false);
        }
        private void lstFiles_DragEnter(object sender, DragEventArgs e)
        {
                e.Effect = DragDropEffects.Copy;
        }
        private void lstFiles_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files)
                CheckandAddFiles(file);
        }

        // non control voids
        private void CheckandAddFiles(string file)
        {
            if (Path.GetExtension(file) != SAVE_FILE_EXTENSION) // If we add a non EU4 file
                Message("Error adding an input with the wrong format", 
                        "The inputfile has to be an " + SAVE_FILE_EXTENSION + " file." +
                        "\n\nThe file " + file + " cannot be added to the list.");
            else if (lstFiles.Items.Cast<Object>().Any(x => x.ToString() == file)) // Checks if we already have the file in the list
                Message("Error adding the same input file twice", 
                        "There is already an input file in the file list with the path:\n" 
                        + file + "\n\nIt cannot be added twice by the program.");
            else
                lstFiles.Items.Add(file);

            // This is used in order to check if we have any files that we can run
            if (lstFiles.Items.Count == 0) // No files, no way to run the program
                EnableRunOptions(false);
            else
                EnableRunOptions(true);
        }
        private void Message(string caption, string message)
        {
            Cursor.Current = Cursors.Default; // in some cases we have the loading cursor active...
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);

        }

        /// <summary>
        /// Enabales and deenables the btn:s that can be used to run the program
        /// </summary>
        /// <param name="enable"></param>
        private void EnableRunOptions(bool enable)
        {
            btnClear.Enabled = enable;
            btnExportStats.Enabled = enable; // cannot export without anything analyzed to export
            Refresh();
        }
        private void resetProgress()
        {
            EnableRunOptions(false);
            lblListTitle.Text = "List of save files";
            lblStatus.Text = "Waiting for work";
            lstFiles.Items.Clear();
            prgTotalProgress.Value = 0;
            Refresh();
        }

        /// <summary>
        /// Does the work for each of the save files and returns when all of the job is done.
        /// </summary>
        /// <param name="saveFiles">String array of the save file paths.</param>
        /// <returns></returns>
        private async Task analyzeSaveFiles(string[] saveFiles)
        {
            runMode(true);
            // Checks what files that can be analysed
            List<AnalyzeAndExport> analysisToDo = new List<AnalyzeAndExport>();
            int numberOfFiles = saveFiles.Count();
            for (int i = 0; i < numberOfFiles; i++)
            {
                string filePathSaveFile = saveFiles[i];
                if (!File.Exists(filePathSaveFile))
                    Message("Error", "File " + filePathSaveFile + " does not exist. This file will not be analyzed.");
                else
                {
                    string outputFolder = Path.GetDirectoryName(filePathSaveFile); // output folder will be the same as the input folder
                    string baseOutputFileName = Path.GetFileNameWithoutExtension(filePathSaveFile); // output file name will be the same as the input file name + "_statistics.csv"
                    analysisToDo.Add(new AnalyzeAndExport(filePathSaveFile, outputFolder, baseOutputFileName,
                                                          progress, numberOfFiles, progressText));
                }
            }

            // Now that we have check that the file exists, analyze the files one by one
            var tasks = new List<Task>();
            int numberOfAnalysis = analysisToDo.Count();
            for (int i = 0; i < numberOfAnalysis; i++) 
            {
                try
                {
                    var analysis = analysisToDo[i];
                    tasks.Add(Task.Run(async () => await analysis.runAnalyze()));
                }
                catch (Exception error)
                {
                    Message("Error", error.ToString());
                }
                //progressText.Report("\n");
            }

            await Task.WhenAll(tasks.ToArray());
        } // end void

        private void runMode(Boolean run)
        {
            if (run) // Show the user that we are now analyzing files and supress run optoions
            {
                lstFiles.Items.Clear(); // we use lstFiles as our progress report now
                lblStatus.Text = "Working...";
                lblListTitle.Text = "Progress status of the analyzis of the file(s)";
                btnBrowseFiles.Enabled = false;
                EnableRunOptions(false);
            }
            else // Return the user to normal
            {
                progressText.Report("Done");
                Message("Yippie", "Analyzed all files that could be analyzed and exported them to the same folder as the save file is located in!");
                resetProgress();
                btnBrowseFiles.Enabled = true;
            }
        }
    }
}