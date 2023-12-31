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
        /// Defines the start and end lines for the data section. Also defines the specific functions we want to use to gather the statistics.
        /// </summary>
        /// <param name="saveFile"></param>
        public ProvinceStatistics(SaveFile saveFile) : base(saveFile)
        {
            const string START_OF_THIS_SECTION_TEXT = "provinces={";             // without tabs, line 147276 in the example file
            const string START_OF_NEXT_SECTION_TEXT = "countries={";               // next section, line 921682 in the example file

            List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics = new List<Func<string, string[]>>()
            {
                getProvinceName,
                getProvinceOwner,
                getProvinceContoler,
                getProvinceReligion,
                getProvinceCulture
            };

            createStatistics(START_OF_THIS_SECTION_TEXT, START_OF_NEXT_SECTION_TEXT, listOfMethodsUsedToGatherStatistics);
        }

        /// <summary>
        /// Fill the dictonary IDs with the keys (IDs) and a province data struct with the ID of each of the provinces.
        /// We also fill the start line for each ID, the line where we identified the tag.
        /// </summary>
        internal override void getAllIdsAndStartLinesFromSaveFile()
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
                    // get the province ID and the add it together with the start line to the statistics data
                    Regex rx = new Regex(PATTERN_OF_THE_ID);
                    string id = rx.Match(line).Groups[1].Value;
                    addIDAndStartLineToTheStatisticsData(id, i);
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