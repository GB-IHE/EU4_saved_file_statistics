using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_saved_file_statistics
{
    public partial class frmMain : Form
    {
        private SaveFile saveFile;
        private ProvinceStatistics provinceStatistics;
        private ExportProvinceStatistics exportStatistics;

        public frmMain()
        {
            InitializeComponent();


            // temp
            const string TEMP_PATH = @"C:\Users\GB\Documents\GitHub\EU4_saved_file_statistics\mp_Crimea1553_01_01 non compressed.eu4";
            saveFile = new SaveFile(TEMP_PATH);
            provinceStatistics = new ProvinceStatistics(saveFile);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // ask the user which file he want to load
            const string TEMP_PATH = @"C:\Users\GB\Documents\GitHub\EU4_saved_file_statistics\mp_Crimea1553_01_01 non compressed.eu4";
            saveFile = new SaveFile(TEMP_PATH);
            MessageBox.Show("File loaded!", "Yippie");
        }

        private void btnGetStats_Click(object sender, EventArgs e)
        {
            provinceStatistics = new ProvinceStatistics(saveFile);
        }

        private void btnExportStats_Click(object sender, EventArgs e)
        {
            // ask the user if he wants to overwrite the file....
            const string TEMP_PATH = @"C:\Users\GB\Documents\GitHub\EU4_saved_file_statistics\Output.csv";
            exportStatistics = new ExportProvinceStatistics(TEMP_PATH, provinceStatistics);
            MessageBox.Show("File exported!", "Yippie");
        }
    }
}
