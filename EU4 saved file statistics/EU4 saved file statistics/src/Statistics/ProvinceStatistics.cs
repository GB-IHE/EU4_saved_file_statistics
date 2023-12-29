using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Statistics for provinces.
    /// </summary>
    public class ProvinceStatistics : Statistics
    {
        /// <summary>
        ///     1) Get the specific start line and end line for the province section in the save file.
        ///     2) Get the IDs for all the provinces in the save file together with the start and end line of stat specific province ID in the save file.
        ///     3) Add statistics for each of the provinces.
        /// </summary>
        public ProvinceStatistics(SaveFile saveFile) : base(saveFile)
        {
            // Get the start and end line in the saved file for the province section
            const string START_LINE_TEXT = "provinces={";             // without tabs, line 147276 in the example file
            const string END_LINE_TEXT = "countries={";               // next section, line 921682 in the example file
            startLineOfTheSectionInTheSaveFile = getFirstLineOfDataSection(START_LINE_TEXT);
            endLineOfTheSectionInTheSaveFile = getLastLineOfDataSection(startLineOfTheSectionInTheSaveFile, END_LINE_TEXT) - 1;

            // Get all the province IDs from the save file as well as start and end line for each ID
            getAllProvinceIdsAndStartLinesFromSaveFile();
            findAllEndLinesInTheSaveFileForAllIDs();

            // Get all the variables for each ID based on the methods that we use
            List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics = new List<Func<string, string[]>>()
            {
                getProvinceName,
                getProvinceOwner,
                getProvinceContoler,
                getProvinceReligion,
                getProvinceCulture
            };

            addStatisticsForAllIds(listOfMethodsUsedToGatherStatistics);
        }

        /// <summary>
        /// Fill the dictonary IDs with the keys (IDs) and a province data struct with the ID of each of the provinces.
        /// We also fill the start line for each ID, the line where we identified the tag.
        /// </summary>
        internal void getAllProvinceIdsAndStartLinesFromSaveFile()
        {
            const char START_LINE_CHAR = '-';             // on the form -1={ -2={ etc.
            const string PATTERN_OF_THE_ID = @"-(.*?)=";  // take the line value and remove "-" and everything after "=" to get the id of the province

            // find the start line of provinces in the save file
            for (int i = startLineOfTheSectionInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, just go on to the next line
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR))
                {
                    Regex rx = new Regex(PATTERN_OF_THE_ID);
                    string id = rx.Match(line).Groups[1].Value;

                    // add it to the data struct and the data lsit
                    IDData provinceData = new IDData();
                    provinceData.id = id;

                    // add start line
                    int startLineInTheSaveFile = i;
                    provinceData.startLineInTheSaveFile = startLineInTheSaveFile;

                    // add the IDData object to the list
                    statisticsData.Add(id, provinceData);
                } // end if
            } // end for
        } // end void

        // Stats for a specific thing, like province owner or province controler
        private string[] getProvinceOwner(string id)
        {
            const string HEADER = "Owner";

            // On the form: '		owner="SWE"'
            const string START_LINE_TEXT = "		owner=";
            const bool QUOTATION_AROUND_THE_DATA = true;
            string owner = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] {HEADER, owner};
        }
        private string[] getProvinceContoler(string id)
        {
            const string HEADER = "Controler";

            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		controller=";
            const bool QUOTATION_AROUND_THE_DATA = true;
            string controler = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, controler };
        }
        private string[] getProvinceName(string id)
        {
            const string HEADER = "Name";

            // On the form: '		name="Stockholm"'
            const string START_LINE_TEXT = "		name=";
            const bool QUOTATION_AROUND_THE_DATA = true;
            string name = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, name };
        }
        private string[] getProvinceReligion(string id)
        {
            const string HEADER = "Religion";

            // On the form: '		religion=protestant'
            const string START_LINE_TEXT = "		religion="; // 			religion=orthodox for hisotry later in the file
            const bool QUOTATION_AROUND_THE_DATA = false;
            string religion = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, religion };
        }
        private string[] getProvinceCulture(string id)
        {
            const string HEADER = "Culture";

            // On the form: '		controller=greek'
            const string START_LINE_TEXT = "		culture=";
            const bool QUOTATION_AROUND_THE_DATA = false;
            string culture = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, culture };
        }
    }
}