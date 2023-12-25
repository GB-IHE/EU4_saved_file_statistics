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
    internal class ExportProvinceStatistics
    {
        public ExportProvinceStatistics(string filePath, ProvinceStatistics provinceStatistics)
        {
            this.filePath = filePath;
            this.provinceStatistics = provinceStatistics;

            printHeaders(filePath);
            printStatistics(filePath);
        }

        private readonly string filePath;
        private readonly ProvinceStatistics provinceStatistics;
        private readonly Boolean APPEND = true;

        // the column headers
        private readonly string[] HEADERS = { "ID", "Owner" };

        // returns the province data as an array for the specific id to be printed
        private string[] provinceDataOutput (in ProvinceData provinceData)
        {
            // get all the data that we want to print for the province and fill it into a string array
            string[] outputData = new string[10];

            // fill the array with data
            int id = provinceData.id;
            outputData[0] = id.ToString();

            string owner = provinceData.owner;
            outputData[1] = owner;

            // return the array
            return outputData;
        }

        // prints the column headers in the file
        private void printHeaders(string filePath)
        {
            using (var textWriter = new StreamWriter(filePath, APPEND, System.Text.Encoding.GetEncoding("iso-8859-1")))
            {
                var outputFile = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
                foreach (string header in HEADERS)
                {
                    outputFile.WriteField(header);
                } // end for each entry in HEADERS
                outputFile.NextRecord(); // provinces after headerss
            } // end using the file
        } // end void

        // print stats
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

        // print stats for specific id
        private void printProvinceStatistics(int id)
        {
            // get the struct ProvinceData for the specific province (id) from the dictonary in provinceStatistics
            ProvinceData provinceData = provinceStatistics.provinceData(id);

            // get the output that we want for the specific province
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
