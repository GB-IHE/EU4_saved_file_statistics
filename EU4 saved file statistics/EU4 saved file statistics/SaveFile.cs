using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_saved_file_statistics
{
    public class SaveFile
    {
        private List<string> SaveFileData = new List<string>(); // the save file line by line

        // opens and stores the file
        public void openFile(string filePath)
        {
            try
            {
                // looping each line in the file https://stackoverflow.com/questions/2161895/reading-large-text-files-with-streams-in-c-sharp
                using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.GetEncoding("iso-8859-1")))
                {
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                        SaveFileData.Add(line);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error, could not open file");
            }
        }
        public void emptyFile() 
        {
            SaveFileData = null;
        } 
        public string getLineData(int line)
        {
            return SaveFileData[line - 1];
        }
        public int getLineCount()
        {
            return SaveFileData.Count();
        }
    }
}
