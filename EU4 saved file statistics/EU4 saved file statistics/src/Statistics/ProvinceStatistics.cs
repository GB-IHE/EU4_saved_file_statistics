using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            // Get the start and end line in the saved file for the provinces
            const string START_LINE_TEXT = "provinces={";             // without tabs, line 147276 in the example file
            const string END_LINE_TEXT = "countries={";               // next section, line 921682 in the example file
            startLineOfTheSectionInTheSaveFile = getFirstLineOfDataSection(START_LINE_TEXT);
            endLineOfTheSectionInTheSaveFile = getLastLineOfDataSection(startLineOfTheSectionInTheSaveFile, END_LINE_TEXT) - 1;

            // Get all the province IDs from the save file
            getAllProvinceIdsAndStartLineFromSaveFile();
            findTheEndLineInTheSaveFileForAllIDs();

            // Get all the variables for each ID
            createProvinceStatisticsForAllIds();
        }

        /// <summary>
        /// Fill the dictonary IDs with the keys (IDs) and a province data struct with the ID of each of the provinces.
        /// ///     We also fill the start line for each ID, the line where we identified the tag.
        /// </summary>
        /// <param name="START_LINE_CHAR">Char that marks the start of the entry.</param>
        /// <param name="PATTERN_OF_THE_ID">Pattern of how the ID is formatted.</param>
        /// 
        internal void getAllProvinceIdsAndStartLineFromSaveFile()
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
                    IDData _provinceData = new IDData();
                    _provinceData.id = id;

                    // add start line
                    int startLineInTheSaveFile = i;
                    _provinceData.startLineInTheSaveFile = startLineInTheSaveFile;

                    // add the IDData object to the list
                    statisticsData.Add(id, _provinceData);
                } // end if
            } // end for
        } // end void

        /// <summary>
        ///     For each id (key), fill the struct ProvinceData with the statistics that we want for the specific province together with its start and end line in the save file.
        /// </summary>
        private void createProvinceStatisticsForAllIds()
        {
            var ids = statisticsData.Keys.ToArray();
            // fill the _provinceData with other stuff based on the start and end line already stored
            foreach (var id in ids)
            {
                // Get the province stats form data struct for the specific id
                IDData _provinceData = statisticsData[id];
                _provinceData.idStats = new List<Tuple<string, string>>();

                // Fill it with other statistics (that we acctally print later on)
                _provinceData.idStats.Add(new Tuple<string, string>("ID", id.ToString()));
                _provinceData.idStats.Add(new Tuple<string, string>("Owner", getProvinceOwner(id)));
                _provinceData.idStats.Add(new Tuple<string, string>("Name", getProvinceName(id)));
                _provinceData.idStats.Add(new Tuple<string, string>("Controler", getProvinceContoler(id)));
                _provinceData.idStats.Add(new Tuple<string, string>("Religion", getProvinceReligion(id)));
                _provinceData.idStats.Add(new Tuple<string, string>("Culture", getProvinceCulture(id)));

                // Update the struct in the dictonary Provinces with the statistics added
                statisticsData[id] = _provinceData;
            }
        }

        // Stats for a specific thing, like province owner or province controler
        private string getProvinceOwner(string id)
        {
            // On the form: '		owner="SWE"'
            const string START_LINE_TEXT = "		owner=";
            return tagsWithStartPattern(id, START_LINE_TEXT, true);
        }
        private string getProvinceContoler(string id)
        {
            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		controller=";
            return tagsWithStartPattern(id, START_LINE_TEXT, true);
        }
        private string getProvinceName(string id)
        {
            // On the form: '		name="Stockholm"'
            const string START_LINE_TEXT = "		name=";
            return tagsWithStartPattern(id, START_LINE_TEXT, true);
        }
        private string getProvinceReligion(string id)
        {
            // On the form: '		religion=protestant'
            const string START_LINE_TEXT = "		religion="; // 			religion=orthodox for hisotry later in the file
            return tagsWithStartPattern(id, START_LINE_TEXT, false);
        }
        private string getProvinceCulture(string id)
        {
            // On the form: '		controller=greek'
            const string START_LINE_TEXT = "		culture=";
            return tagsWithStartPattern(id, START_LINE_TEXT, false);
        }
    }
}