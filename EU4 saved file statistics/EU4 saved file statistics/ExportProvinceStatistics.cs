using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using CsvHelper;

//https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp 
//https://www.youtube.com/watch?v=fRaSeLYYrcQ&ab_channel=RobertsDevTalk
// https://riptutorial.com/csv-helper/learn/100000/getting-started

namespace EU4_saved_file_statistics
{
    internal class ExportProvinceStatistics : ExportStatistics
    {
        private readonly ProvinceStatistics provinceStatistics;
        private readonly string filePath;

        private readonly Boolean APPEND = true; // appends output to the output file
        private readonly string[] HEADERS = { "ID", "Name", "Owner", "Controler", "Religion", "Culture" };         // the column headers for this output

        public ExportProvinceStatistics(string outputFolder, string baseOutputFileName, Statistics statistics) : base(outputFolder, baseOutputFileName, statistics)
        {
            provinceStatistics = statistics.getProvinceStatistics();

            const string FILE_SUFFIX = "_province statistics.csv";
            filePath = outputFolder + @"\" + baseOutputFileName + FILE_SUFFIX;
            deleteOldOutputFiles(filePath);

            printHeaders(filePath, HEADERS);
            printStatistics(filePath);
        }

        // returns the province data that we want to print from the provinceData struct in Statistics statistics as an array for the specific id
        private string[] provinceDataOutput (in ProvinceData provinceData)
        {
            // get all the data that we want to print for the province and fill it into a string array
            string[] outputData = new string[10];

            // fill the array with data (note that we need the corresponding headers in HEADERS)
            outputData[0] = provinceData.id.ToString(); // key: id
            outputData[1] = provinceData.name;
            outputData[2] = provinceData.owner;
            outputData[3] = provinceData.controler;
            outputData[4] = provinceData.religion;
            outputData[5] = provinceData.culture;

            // return the array
            return outputData;
        }

        // printStatistics for each province
        private void printStatistics(string filePath)
        {
            // province statistics
            // get all province IDs from the dictonary in provinceStatistics and loop through them
            int[] ids = provinceStatistics.provinceIds();
            for (int i = 0; i < ids.Length; i++)
            {
                int id = ids[i];
                printProvinceStatistics(id);
            } // end for each id
        } // end void

        // print stats for specific province id
        private void printProvinceStatistics(int id)
        {
            // get the struct ProvinceData for the specific province (id) from the dictonary in provinceStatistics
            ProvinceData provinceData = provinceStatistics.provinceData(id);

            // get the output that we want for the specific province (id)
            string[] outputData = provinceDataOutput(provinceData);

            // print the data, column by column for all entries in outputData
            using (var textWriter = new StreamWriter(filePath, APPEND, System.Text.Encoding.GetEncoding("iso-8859-1")))
            {
                var outputFile = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                foreach (string columnData in outputData)
                {
                    outputFile.WriteField(columnData);
                } // end for each entry in outputData
                outputFile.NextRecord(); // new province
            } // end using the file
        } // end void
    } // end class
} // end namespace
