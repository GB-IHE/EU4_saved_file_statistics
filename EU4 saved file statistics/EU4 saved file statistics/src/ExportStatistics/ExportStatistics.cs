using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// https://stackoverflow.com/questions/18757097/writing-data-into-csv-file-in-c-sharp 
// https://www.youtube.com/watch?v=fRaSeLYYrcQ&ab_channel=RobertsDevTalk
// https://riptutorial.com/csv-helper/learn/100000/getting-started

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Parent class for all statistic classes.
    /// </summary>
    internal class ExportStatistics
    {
        internal readonly Dictionary<string, IDData> statisticsData;
        internal CsvWriter outputFile;

        internal readonly Boolean APPEND = true;

        /// <summary>
        /// Export statistics to a .csv file.
        /// </summary>
        /// <param name="outputFolder">The folder for the output file.</param>
        /// <param name="baseOutputFileName">The name of the output file, a suffix is added to it based on the type of statistics.</param>
        /// <param name="statistics">The analyzed statistics that is to be printed.</param>
        /// /// <param name="outputFileSuffix">The suffix to be added to the output file.</param>
        public ExportStatistics(string outputFolder, string baseOutputFileName, Statistics statistics, string outputFileSuffix)
        {
            // Create the file path and delete old output files
            string filePath = outputFolder + @"\" + baseOutputFileName + outputFileSuffix;
            deleteOldOutputFiles(filePath);

            // Get statistics for all province IDs
            statisticsData = statistics.getStatisticsData();

            // Open output file
            StreamWriter textWriter = new StreamWriter(filePath, APPEND, System.Text.Encoding.GetEncoding("iso-8859-1"));
            outputFile = new CsvWriter(textWriter, CultureInfo.InvariantCulture);

            // Print to the file and close it
            printHeaders();
            printStatisticsForEachID();
            textWriter.Close();
        }

        /// <summary>
        /// Delets the old ouput file, if ther is any.
        /// </summary>
        /// <param name="filePath">The path of the file to be deleted.</param>
        internal void deleteOldOutputFiles (string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath); //delete the csv output file if it already exists - we are going to make a new one
        }

        /// <summary>
        /// Prints the column headers in the output file based on the key value in the statstics for one id (same headers for all provinces, countries etc).
        /// </summary>
        internal void printHeaders()
        {
            IDData dataFirstID = statisticsData.ElementAt(0).Value;         // statstics for first tag, province, country etc (same column names for all provinces, countries)
            List<Tuple<string, string>> statisticsIDOne = dataFirstID.idStats;
            foreach (var ouputData in statisticsIDOne)
            {
                string columnName = ouputData.Item1; // key
                outputFile.WriteField(columnName);
            }
            outputFile.NextRecord(); // done with headers, go on with provinces/countries for the next line
        }

        /// <summary>
        /// Print statistics for each output in the list of tuple in province statstics for each ID, province, country etc. 
        /// </summary>
        internal void printStatisticsForEachID()
        {
            foreach (var datID in statisticsData.Values)
            {
                List<Tuple<string, string>> statisticsID = datID.idStats;
                foreach (var ouputData in statisticsID)
                {
                    string val = ouputData.Item2;
                    outputFile.WriteField(val);
                }
                outputFile.NextRecord(); // new province
            } // end province loop
        } // end void
    }
}