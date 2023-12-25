using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace EU4_saved_file_statistics
{
    public class ProvinceStatistics
    {
        private readonly SaveFile saveFile;

        private int startLineProvincesInTheSaveFile;
        private int endLineProvincesInTheSaveFile;

        private Dictionary<int, ProvinceData> Provinces = new Dictionary<int, ProvinceData>();

        // get the start and end line in the saved file for the provinces
        // get the specific start line and end line for all provinces
        public ProvinceStatistics(SaveFile saveFile)
        {
            this.saveFile = saveFile;
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
            for(int i = startLineProvincesInTheSaveFile; i < lenghtOfFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                {
                    endLineProvincesInTheSaveFile = i;
                    break;
                }
            }

        }

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



        // for each id (key), fill the struct ProvinceData with the statistics that we want
        private void createProvinceStatisticsForAllIds()
        {
            var ids = Provinces.Keys.ToArray();
            // fill it with start and end line for the specific province id (which we need to analyze the other sutff
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

            foreach (var id in ids)
            {
                ProvinceData _provinceData = Provinces[id];

                // fill it with other stuff
                string owner = getProvinceOwner(id);
                _provinceData.owner = owner;

                Provinces[id] = _provinceData;
            }
        }

        // filling the struct ProvinceData with data for a specific province (ID)
        private int getProvinceStartLine(int id)
        {
            // on the form -1={
            // -2={
            // we can get it by the pattern that the first char is not a space but a -
            const char START_LINE_CHAR = '-';

            // find the start line of provinces in the save file
            for (int i = startLineProvincesInTheSaveFile; i < endLineProvincesInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0)
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR))
                    return i;
            }
            return -1; // no end line found
        }

        private int getProvinceEndLine(int id)
        {
            const string END_LINE_TEXT = "	}";

            // find the start line of provinces in the save file
            for (int i = startLineProvincesInTheSaveFile; i < endLineProvincesInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                    return i;
            }
            return -1; // no end line found
        }

        private string getProvinceOwner(int id)
        {

            // here we need a patter, shoudl start with



            const string ID_LINE_TEXT = "		owner=";

            // get the province data for the specified ID and its start and end line in the data
            ProvinceData _provinceData = Provinces[id];
            int startLineInTheSaveFile = _provinceData.startLineInTheSaveFile;
            int endLineInTheSaveFile = _provinceData.endLineInTheSaveFile;


            // find the province owner of the province within the specified line ranges
            for (int i = startLineInTheSaveFile; i < endLineInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                Boolean ownerTag = line.StartsWith(ID_LINE_TEXT);
                if (ownerTag)
                {




                    line = line.Replace('"', '-'); // remove quotation marks
                // https://stackoverflow.com/questions/13024073/regex-c-sharp-extract-text-within-double-quotes
                // https://stackoverflow.com/questions/9133220/regex-matches-c-sharp-double-quotes
                    Regex rx = new Regex(@"-(.*?)-");




                    string owner = rx.Match(line).Groups[1].Value;
                    return owner; // everything between the qoutation marks in: owner="SWE"
                }
            }
            return "NONE";
        }

        // Accessible outside the class
        public string provinceOwner(string provinceName)
        {
            return "";
        }

        public string provinceOwner(int provinceId)
        {
            return "";
        }
    }
}
