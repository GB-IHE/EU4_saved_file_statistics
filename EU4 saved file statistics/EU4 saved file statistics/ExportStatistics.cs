using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    internal class ExportStatistics
    {
        internal readonly string outputFolder;
        internal readonly string baseOutputFileName; // base name, suffixes added later on, like: mp_Crimea1553_01_01 non compressed_provice statistics.csv
        internal readonly Statistics statistics;

        internal readonly Boolean APPEND = true;

        public ExportStatistics(string outputFolder, string baseOutputFileName, Statistics statistics)
        {
            this.outputFolder = outputFolder;
            this.baseOutputFileName = baseOutputFileName;
            this.statistics = statistics;
        }

        // for all the statistics that we want to print
        public void exportStatistics()
        {
            ExportProvinceStatistics provinceStats = new ExportProvinceStatistics(outputFolder, baseOutputFileName, statistics);
        }

        // prints the column headers in the output file (takes the filePath of the output file and the headers as input in form of a string array)
        internal void printHeaders(string filePath, in string[] HEADERS)
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

        internal void deleteOldOutputFiles (string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath); //delete the csv output file if it already exists - we are going to make a new one
        }

    }
}
