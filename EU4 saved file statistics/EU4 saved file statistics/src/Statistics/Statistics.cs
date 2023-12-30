using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Common functions and variables for the child classes with specific statistics
    /// </summary>
    public abstract class Statistics
    {
        internal readonly SaveFile saveFile;
        internal int startLineOfTheSectionInTheSaveFile; // start and end line of the section in the save file (for instance province section) - used when we search for specific provinces, countries etc
        internal int endLineOfTheSectionInTheSaveFile;
        /// <summary>
        /// Data for all provinces, countries etc. <ID (string), data for the specific ID>
        /// </summary>
        internal Dictionary<string, IDData> statisticsData = new Dictionary<string, IDData>();

        public Statistics(SaveFile saveFile)
        {
            this.saveFile = saveFile;
        }

        /// <summary>
        /// 1) Get the specific start line and end line for the countries section in the save file.
        /// 2) Get the IDs for all the countries in the save file together with the start and end line of stat specific countries ID in the save file.
        /// 3) Add statistics for each of the countries.
        /// </summary>
        /// <param name="START_LINE_TEXT">The text that marks the start of the section.</param>
        /// <param name="END_LINE_TEXT">The text that marks the end of the section.</param>
        /// <param name="listOfMethodsUsedToGatherStatistics">The methods used to create the statistics.</param>
        internal void createStatistics(in string START_LINE_TEXT, in string END_LINE_TEXT, List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics)
        {
            // Get the start and end line in the saved file for the province section
            startLineOfTheSectionInTheSaveFile = getFirstLineOfDataSection(START_LINE_TEXT);
            endLineOfTheSectionInTheSaveFile = getLastLineOfDataSection(startLineOfTheSectionInTheSaveFile, END_LINE_TEXT) - 1;

            // Get all the province IDs from the save file as well as start and end line for each ID
            getAllIdsAndStartLinesFromSaveFile();
            findAllEndLinesInTheSaveFileForAllIDs();

            // Get all the variables for each ID based on the methods that we use
            addStatisticsForAllIds(listOfMethodsUsedToGatherStatistics);
        }

        /// <summary>
        ///  Get the specific start line for the section in the save file.
        /// </summary>
        /// <param name="START_LINE_TEXT">The text in the save file that marks the start of the data section.</param>
        /// <returns></returns>
        internal int getFirstLineOfDataSection(in string START_LINE_TEXT)
        {
            int lenghtOfFile = saveFile.getLineCount();

            // find the start line of provinces in the save file
            for (int i = 1; i <= lenghtOfFile; i++) // the lines in the file start at 1
            {
                string line = saveFile.getLineData(i);
                if (line == START_LINE_TEXT)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Get the specific end line for the section in the save file.
        /// </summary>
        /// <param name="startLineOfDataSection">We start the seach at the start line of the data section and not at the start of the file.</param>
        /// <param name="END_LINE_TEXT">The text in the save file that marks the end of the data section.</param>
        /// <returns></returns>
        internal int getLastLineOfDataSection(int startLineOfDataSection, in string END_LINE_TEXT)
        {
            int lenghtOfFile = saveFile.getLineCount();

            // find the end line of provinces in the save file
            for (int i = startLineOfDataSection; i < lenghtOfFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                    return i;
            } // end loop
            return -1;
        }

        /// <summary>
        /// We have different mehtods for finding the IDs and start lines for each statistical category in the child classes.
        /// </summary>
        internal abstract void getAllIdsAndStartLinesFromSaveFile();

        /// <summary>
        ///  Creates a new ID data struct for the new ID found in the data and adds the IDData object to the list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startLineOfTheID"></param>
        internal void addIDAndStartLineToTheStatisticsData(string id, int startLineOfTheID)
        {
            IDData idData = new IDData();
            idData.id = id;
            idData.startLineInTheSaveFile = startLineOfTheID;
            statisticsData.Add(id, idData);
        }

        /// <summary>
        /// Fill the IDData with end line for the specific province id (which we need to analyze the other sutff)
        /// </summary>
        internal void findAllEndLinesInTheSaveFileForAllIDs()
        {
            var ids = statisticsData.Keys.ToArray();
            for (int i = 0; i < ids.Count(); i++)
            {
                string id = ids[i];
                IDData idData = statisticsData[id];

                int endLineInTheSaveFile;
                if (i == ids.Count() - 1) // sedond last ID
                    endLineInTheSaveFile = endLineOfTheSectionInTheSaveFile; // the line of the end of the section in the save file - we have no more IDs
                else
                    endLineInTheSaveFile = statisticsData[ids[i + 1]].startLineInTheSaveFile - 1; // the line before the start of the next province

                idData.endLineInTheSaveFile = endLineInTheSaveFile;
                statisticsData[id] = idData;
            }
        }

        /// <summary>
        /// For each id (key), call the function addIDData to fill the struct IDData with the statistics that we want for the specific province/country. 
        /// </summary>
        /// <param name="listOfMethodsUsedToGatherStatistic">The methods used to get the ID Data for the specific case, like provinces or countries.</param>
        internal void addStatisticsForAllIds(List<Func<string, string[]>> listOfMethodsUsedToGatherStatistic)
        {
            var ids = statisticsData.Keys.ToArray();

            // Update the struct in the dictonary statisticsData with statistics that will be added for the IDs one by one
            var tasks = new List<Task>();
            foreach (string id in ids)
            {
                var task = Task.Run(() => addStatisticsForOneID(id, listOfMethodsUsedToGatherStatistic));
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// Adds statistics for a specific ID that is to be printed later on.
        /// </summary>
        /// <param name="id">ID of the province.</param>
        /// <param name="listOfMethodsUsedToGatherStatistics">The methods used to get the ID Data for the specific case, like provinces or countries.</param>
        /// <returns>A filled IDData with statistics.</returns>
        internal void addStatisticsForOneID(string id, List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics)
        {
            IDData idData = statisticsData[id];             // Get the ID stats form data struct for the specific id

            // Create a new list of tuples in the IDData data struct with the ouput to be analyzed and later printed
            idData.idStats = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("ID", id) // We already have the ID in the struct, add it to the list to be printed
            };

            foreach (Func<string, string[]> method in listOfMethodsUsedToGatherStatistics)
            {
                string[] headerAndValue = method(id);
                string header = headerAndValue[0]; // key value
                string value = headerAndValue[1];
                idData.idStats.Add(new Tuple<string, string>(header, value));
            }

            statisticsData[id] = idData;
        }

        /// <summary>
        /// Returns the value for all tags (like name, owner, controler, religion etc) on the form like : '		controller="SWE"'.
        /// That is there is a start pattern text to match for the province id, country etc.
        /// If we do not have quotation marks around the text, we have an equal sign - return that instead.
        /// </summary>
        /// <param name="id">ID of the province, country that we are looking for</param>
        /// <param name="START_LINE_TEXT">The text makring the type of statistics, like "country"</param>
        /// <param name="quotationMarks">Quotation marks around the tag, like controler="SWE"</param>
        /// <returns>The value, like province owner or religion</returns>
        internal string tagsWithStartPattern(string id, in string START_LINE_TEXT, Boolean quotationMarks)
        {
            // find the province controler of the province within the specified line ranges
            for (int i = startLineID(id); i < endLineID(id); i++)
            {
                string line = saveFile.getLineData(i);
                Boolean controlerTag = line.StartsWith(START_LINE_TEXT);
                if (controlerTag && quotationMarks)
                    return getTextBetweenQuotationMarks(line); // everything between the qoutation marks in text like: controler="SWE"
                else if (controlerTag && !quotationMarks)
                    return getTextAfterChar(line, '=');        // everything after "="
            }
            return "NONE"; // nothing found inside the tag (province, country) line range
        }

        /// <summary>
        /// Checks if a string only contains uppercase chars.
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        internal bool IsAllUpper(string inputString)
        {
            foreach (char c in inputString)
                if (char.IsLower(c))
                    return false;
            return true;
        }

        public bool IsNumeric(string str)
        {
            return str.Any(char.IsDigit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Start line for the specific id (province, country etc.)</returns>
        internal int startLineID(string id)
        {
            IDData idData = statisticsData[id];
            return idData.startLineInTheSaveFile;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>End line for the specific id (province, country etc.)</returns>
        internal int endLineID(string id)
        {
            IDData idData = statisticsData[id];
            return idData.endLineInTheSaveFile;
        }

        /// <summary>
        /// Returns everything between the first quotation marks in the line.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <returns></returns>
        internal string getTextBetweenQuotationMarks(string line)
        {
            var stringArray = line.Split('"').Where((item, index) => index % 2 != 0);
            return stringArray.ToArray()[0];
        }

        /// <summary>
        /// Returns everything from a line after a sign (su.
        /// </summary>
        /// <param name="line">Text line.</param>
        /// <param name="character">The character that we should get text after.</param>
        /// <returns></returns>
        internal string getTextAfterChar(string line, char character)
        {
            return line.Split(character).Last();
        }

        /// <summary>
        /// Get the statistics for the all IDs (used when we print out the province statstics)
        /// </summary>
        /// <returns>The province statistics class created</returns>
        public Dictionary<string, IDData> getStatisticsData()
        {
            return statisticsData;
        }
    }
}