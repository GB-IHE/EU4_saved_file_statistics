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
        private SaveFile saveFile = new SaveFile();
        private ProvinceStatistics provinceStatistics;
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            const string TEMP_PATH = @"C:\Users\GB\Documents\GitHub\EU4_saved_file_statistics\mp_Crimea1553_01_01 non compressed.eu4";
            saveFile.openFile(TEMP_PATH);
            MessageBox.Show("File loaded", "Yippie");
        }

        private void btnGetStats_Click(object sender, EventArgs e)
        {
            provinceStatistics = new ProvinceStatistics(saveFile);
        }
    }
}
