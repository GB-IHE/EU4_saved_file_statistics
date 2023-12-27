using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/**
* @author Gunnar Brådvik
* E-mail: gunnar@bradvik.se
*
* @date - $time$ 
* 
* We have the followinig classes:
*   AnalyzeAndExport - Runs the other classes and child classes based on the user input
*       
*   SaveFile - loads the save file line by line, constructor filePath
*   
*   Statistics - a parent class that creates statistics for a specific filePath, constructor SaveFile
*       IDData is a struct where the data for each ID (province ID, country etc) are stored - used by each statistics class
*       ProvinceStatistics - a child class that creates statistics regarding the provinces in the save file (uses the province ID as key)
*          
*   ExportStatistics - a parent class that exports statistics to a specific folder for all the statistics we want to analyze constructed by outputFolder, outputFileName, and Statistics (outputFileName = base output file name, suffixes are added by the class)
*       ExportProvinceStatistics - a child class that exports the province statistics
*       
* Note that current output file will be deleted and new output files will be created in the same folder as the save files.
*/


// we have to make the user choce an output folder - now the files are saved in the same folder as the save file
// ask the user if he wants to overwrite the output file....
// generate sensible output file names

namespace EU4_saved_file_statistics
{
    public partial class frmMain : Form
    {
        private readonly string SAVE_FILE_EXTENSION = ".eu4";

        public frmMain()
        {
            InitializeComponent();

            lstFiles.DragDrop += lstFiles_DragDrop;
            lstFiles.DragEnter += lstFiles_DragEnter;

            // run debug if we are in dubug mode
            if (System.Diagnostics.Debugger.IsAttached)
                debug();
        }

        private void debug()
        {
            string currDirectory = Directory.GetCurrentDirectory();
            string saveFileDirectory = Path.GetFullPath(Path.Combine(currDirectory, @"..\..\..\..\", "Save files"));
            string saveFileName = @"\mp_Crimea1553_01_01 non compressed.eu4";
            string saveFilePath = saveFileDirectory + saveFileName;
            string baseOutputFileName = Path.GetFileNameWithoutExtension(saveFilePath);

            AnalyzeAndExport debugRun = new AnalyzeAndExport(saveFilePath, saveFileDirectory, baseOutputFileName);

            // done
            MessageBox.Show("File exported!", "Yippie");
        }

        private void btnExportStats_Click(object sender, EventArgs e)
        {
            if (lstFiles.Items.Count == 0)
            {
                Message("Error", "No file has been selected. Press ok to close this dialog and try again.");
                return;
            }

            // do the work for the save files one by one
            for (int i = 0; i < lstFiles.Items.Count; i++)
            {
                string filePathSaveFile = lstFiles.Items[i].ToString();
                if (!File.Exists(filePathSaveFile))
                {
                    Message("Error", "File " + filePathSaveFile + " does not exist. This file will not be loaded.");
                    continue;
                }

                string outputFolder = Path.GetDirectoryName(filePathSaveFile); // output folder will be the same as the input folder
                string baseOutputFileName = Path.GetFileNameWithoutExtension(filePathSaveFile); // output file name will be the same as the input file name + "_statistics.csv"
                
                try
                { 
                    AnalyzeAndExport debugRun = new AnalyzeAndExport(filePathSaveFile, outputFolder, baseOutputFileName);
                } 
                catch (Exception error)
                {
                    Message("Error", error.ToString());
                }
            }
            MessageBox.Show("Analyzed all files that could be analyzed!", "Yippie");
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
            if (e.KeyCode == Keys.Delete)
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
            Cursor.Current = Cursors.Default; //in some cases we have the loading cursor active...
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
            btnExportStats.Enabled = enable; // cannot export without anything analyzed to export
            Refresh();
        }
    }
}