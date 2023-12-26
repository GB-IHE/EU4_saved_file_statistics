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
    public class ProvinceStatistics : Statistics
    {
        private int startLineProvincesInTheSaveFile;
        private int endLineProvincesInTheSaveFile;

        private readonly int FIRST_PROVINCE_ID = 1;

        private Dictionary<int, ProvinceData> Provinces = new Dictionary<int, ProvinceData>();

        // get the start and end line in the saved file for the provinces
        // get the specific start line and end line for all provinces
        public ProvinceStatistics(SaveFile saveFile) : base(saveFile)
        {
            getProvinceStartAndEndLinesInTheSaveFile();
            getAllIdsFromSaveFile();
            createProvinceStatisticsForAllIds();
        }
 
        private void getProvinceStartAndEndLinesInTheSaveFile()
        {
            const string START_LINE_TEXT = "provinces={";   // without tabs, line 147276 in the example file
            const string END_LINE_TEXT = "}";               // without tabs, line 921682 in the example file
            int lenghtOfFile = saveFile.getLineCount();

            // find the start line of provinces in the save file
            for (int i = 1; i <= lenghtOfFile; i++) // the lines in the file start at 1
            {
                string line = saveFile.getLineData(i);
                if (line == START_LINE_TEXT)
                {
                    startLineProvincesInTheSaveFile = i;
                    break;
                }
            }

            // find the end line of provinces in the save file
            for (int i = startLineProvincesInTheSaveFile; i < lenghtOfFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                {
                    endLineProvincesInTheSaveFile = i;
                    break;
                } // end if
            } // end loop
        } // end void

        // fill the dictonary Provinces with the keys, IDs and a province data struct with the ID
        private void getAllIdsFromSaveFile()
        {
            // on the form -1={
            // -2={
            // etc.
            const char START_LINE_CHAR = '-';

            // find the start line of provinces in the save file
            for (int i = startLineProvincesInTheSaveFile; i < endLineProvincesInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if emptu line, just go on
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR))
                {
                    // take the line value and remove "-" and everything after "=" to get the id of the province
                    Regex rx = new Regex(@"-(.*?)="); // https://stackoverflow.com/questions/49239218/get-string-between-character-using-regex-c-sharp
                    string idString = rx.Match(line).Groups[1].Value;
                    int id = Int32.Parse(idString);

                    // add it to the province data
                    ProvinceData _provinceData = new ProvinceData();
                    _provinceData.id = id;
                    Provinces.Add(id, _provinceData);
                } // end if
            } // end for
        } // end void

        // for each id (key), fill the struct ProvinceData with the statistics that we want for the province
        private void createProvinceStatisticsForAllIds()
        {
            var ids = Provinces.Keys.ToArray();
            // fill it with start and end line for the specific province id (which we need to analyze the other sutff)
            foreach (var id in ids)
            {
                ProvinceData _provinceData = Provinces[id];

                // start line
                int startLine = getProvinceStartLine(id);
                _provinceData.startLineInTheSaveFile = startLine;
                Provinces[id] = _provinceData;

                // end line
                int endLine = getProvinceEndLine(id);
                _provinceData.endLineInTheSaveFile = endLine;
                Provinces[id] = _provinceData;
            }

            // fill the _provinceData with other stuff based on the start and end line already stored
            foreach (var id in ids)
            {
                ProvinceData _provinceData = Provinces[id];

                // fill it with other statistics (that we acctally print later on)
                _provinceData.owner = getProvinceOwner(id);
                _provinceData.controler = getProvinceContoler(id);
                _provinceData.name = getProvinceName(id);
                _provinceData.religion = getProvinceReligion(id);
                _provinceData.culture = getProvinceCulture(id);

                // update the struct in the dictonary Provinces with the statistics added
                Provinces[id] = _provinceData;
            }
        }

        // filling the struct ProvinceData with data for a specific province (ID)

        // get the start line in the save file for the specific proivnce id
        private int getProvinceStartLine(int id)
        {
            // on the form -1={
            // -2={ etc
            // we can get it by the pattern that the first char is not a space but a -
            const char START_LINE_CHAR = '-';

            // for the first id we seach from the start line of the province section in the start file
            // for the id after the first one, we start the search from the line after the end of the last province
            int startLineInTheSaveFile = id == FIRST_PROVINCE_ID ? startLineProvincesInTheSaveFile : Provinces[id - 1].endLineInTheSaveFile + 1;

            // find the start line of provinces in the save file
            for (int i = startLineInTheSaveFile; i < endLineProvincesInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, go on to the next line
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR)) // check if we have a province tag
                {
                    Regex rx = new Regex(@"-(.*?)="); // https://stackoverflow.com/questions/49239218/get-string-between-character-using-regex-c-sharp
                    string idString = rx.Match(line).Groups[1].Value;
                    int foundId = Int32.Parse(idString);

                    // if we have found the right id, then return the row number - else go on
                    if (foundId == id)
                        return i;
                }
            }
            return -1; // no end line found -> this will result in an error later on
        }

        // get the end line in the save file for the specific proivnce id
        private int getProvinceEndLine(int id)
        {
            const string END_LINE_TEXT = "	}";

            // scan the save file from start line of the specific id to the end of the province data
            ProvinceData provinceData = Provinces[id];
            int startLineInTheSaveFile = provinceData.startLineInTheSaveFile;

            // find the start line of provinces in the save file
            for (int i = startLineInTheSaveFile; i < endLineProvincesInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                    return i;
            }
            return -1; // no end line found -> this will result in an error later on
        }

        // get stats
        private string getProvinceOwner(int id)
        {
            // On the form: '		owner="SWE"'
            const string START_LINE_TEXT = "		owner=";
            return tagsWithStartPattern(id, START_LINE_TEXT);
        }
        private string getProvinceContoler(int id)
        {
            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		controller=";
            return tagsWithStartPattern(id, START_LINE_TEXT);
        }
        private string getProvinceName(int id)
        {
            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		name=";
            return tagsWithStartPattern(id, START_LINE_TEXT);
        }
        private string getProvinceReligion(int id)
        {
            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		religion="; // 			religion=orthodox for hisotry later in the file
            return tagsWithStartPattern(id, START_LINE_TEXT);
        }
        private string getProvinceCulture(int id)
        {
            // On the form: '		controller="SWE"'
            const string START_LINE_TEXT = "		culture=";
            return tagsWithStartPattern(id, START_LINE_TEXT);
        }



        /**
         * returns the value for all tags (like name, owner, controler, religion etc) on the form like : '		controller="SWE"'
         * that is there is a start pattern text to match for the province id
         */
        private string tagsWithStartPattern(int id, in string START_LINE_TEXT)
        {
            // find the province controler of the province within the specified line ranges
            for (int i = startLineProvince(id); i < endLineProvince(id); i++)
            {
                string line = saveFile.getLineData(i);
                Boolean controlerTag = line.StartsWith(START_LINE_TEXT);
                if (controlerTag)
                    return getTextBetweenQuotationMarks(line); // everything between the qoutation marks in: controler="SWE"
            }
            return "NONE"; // no province controler found inside the province line range
        }







        // returns the start and end lines for the specific province id
        private int startLineProvince(int id)
        {
            ProvinceData _provinceData = Provinces[id];
            return _provinceData.startLineInTheSaveFile;
        }
        private int endLineProvince(int id)
        {
            ProvinceData _provinceData = Provinces[id];
            return _provinceData.endLineInTheSaveFile;
        }

        /*
           returns everything between the first quotation qutation marks
            https://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
            https://stackoverflow.com/questions/69377127/how-can-i-access-string-values-in-results-view-where-the-number-of-results-may-v
        */
        private string getTextBetweenQuotationMarks(string line)
        {
            // if there is nothing inside the getTextBetweenQuotationMarks........... add a bolean to tagsWithStartPattern, culture and religion do not have quations
            var stringArray = line.Split('"').Where((item, index) => index % 2 != 0);
            return stringArray.ToArray()[0];
        }



        // Accessible outside the class
        public int[] provinceIds()
        {
            return Provinces.Keys.ToArray();
        }

        public ProvinceData provinceData(int i)
        {
            return Provinces[i];
        }
    }
}
