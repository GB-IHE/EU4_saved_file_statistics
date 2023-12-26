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
        readonly private string FIRST_PROVINCE_ID = "1";

        /// <summary>
        ///     1) Get the specific start line and end line for the province section in the save file.
        ///     2) Get the IDs for all the provinces in the save file together with the start and end line of stat specific province ID in the save file.
        ///     3) Add statistics for each of the provinces.
        /// </summary>
        public ProvinceStatistics(SaveFile saveFile) : base(saveFile)
        {
            // Get the start and end line in the saved file for the provinces
            const string START_LINE_TEXT = "provinces={";   // without tabs, line 147276 in the example file
            const string END_LINE_TEXT = "}";               // without tabs, line 921682 in the example file
            startLineOfTheSectionInTheSaveFile = getFirstLineOfDataSection(START_LINE_TEXT);
            endLineOfTheSectionInTheSaveFile = getLastLineOfDataSection(startLineOfTheSectionInTheSaveFile, END_LINE_TEXT);

            getAllIdsFromSaveFile();
            createProvinceStatisticsForAllIds();
        }

        /// <summary>
        ///     Fill the dictonary Provinces with the keys (IDs) and a province data struct with the ID of each of the provinces.
        /// </summary>
        private void getAllIdsFromSaveFile()
        {
            // on the form -1={
            // -2={
            // etc.
            const char START_LINE_CHAR = '-';

            // find the start line of provinces in the save file
            for (int i = startLineOfTheSectionInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, just go on to the next line
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR))
                {
                    // take the line value and remove "-" and everything after "=" to get the id of the province
                    Regex rx = new Regex(@"-(.*?)=");
                    string id = rx.Match(line).Groups[1].Value;

                    // add it to the province data
                    IDData _provinceData = new IDData();
                    _provinceData.id = id;
                    statisticsData.Add(id, _provinceData);
                } // end if
            } // end for
        } // end void

        /// <summary>
        ///     For each id (key), fill the struct ProvinceData with the statistics that we want for the specific province together with its start and end line in the save file.
        /// </summary>
        private void createProvinceStatisticsForAllIds()
        {
            // fill it with start and end line for the specific province id (which we need to analyze the other sutff)
            var ids = statisticsData.Keys.ToArray();
            for (int i = 0; i < ids.Count(); i++)
            {
                string id = ids[i];
                IDData _provinceData = statisticsData[id];

                // start line
                // for the first id we seach from the start line of the province section in the start file
                // for the id after the first one, we start the search from the line after the end of the last province
                int startLineInTheSaveFile = id == FIRST_PROVINCE_ID ? startLineOfTheSectionInTheSaveFile : statisticsData[ids[i - 1]].endLineInTheSaveFile + 1;              
                const char START_LINE_CHAR = '-';  // on the form -1={, -2={ etc.
                const string PATTERN = @"-(.*?)="; // we can get it by the pattern that the first char is not a space but a -
                int startLine = getIdStartLine(id, startLineInTheSaveFile, START_LINE_CHAR, PATTERN);
                _provinceData.startLineInTheSaveFile = startLine;
                statisticsData[id] = _provinceData;

                // end line
                // scan the save file from start line of the specific id to the end of the province data
                const string END_LINE_TEXT = "	}";
                int endLine = getIdEndLine(id, startLineInTheSaveFile, END_LINE_TEXT);
                _provinceData.endLineInTheSaveFile = endLine;
                statisticsData[id] = _provinceData;
            }

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
