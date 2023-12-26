﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Stores the save file and make it usable for all the child classes (different statistics).
    /// Stores all the statistic classes (child classes) that we create when we run createStatistics.
    /// </summary>
    public class Statistics
    {
        internal readonly SaveFile saveFile;
        internal int startLineOfTheSectionInTheSaveFile; // start and end line of the section in the save file (for instance province section) - used when we search for specific provinces, countries etc
        internal int endLineOfTheSectionInTheSaveFile;

        /// <summary>
        /// Data for all provinces, countries etc. <ID (string), data for the ID>
        /// </summary>
        internal Dictionary<string, IDData> statisticsData = new Dictionary<string, IDData>();

        public Statistics(SaveFile saveFile)
        {
            this.saveFile = saveFile;
        }

        /// <summary>
        /// Get the statistics for the akk  (used when we print out the province statstics)
        /// </summary>
        /// <returns>The province statistics class created</returns>
        public Dictionary<string, IDData> getStatisticsData() { return statisticsData; }

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
        /// Get the start line in the save file for the specific ID (for instance a province or a country).
        /// </summary>
        /// <param name="id">The id, 2 for province or SWE for country for instance</param>
        /// <param name="endLinePreviousID">The last line of the previous ID in the save file, for instance province 1.</param>
        /// <param name="START_LINE_CHAR">The start line char that identifies the ID, for instance "-" for provinces.</param>
        /// <param name="PATTERN">The pattern that the ID is withing, for instance "-4001=" </param>
        /// <returns></returns>
        internal int getIdStartLine(string id, int endLinePreviousID, in char START_LINE_CHAR, in string PATTERN)
        {
            // find the start line of id in the save file
            for (int i = endLinePreviousID; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, go on to the next line
                    continue;

                char lineFirstChar = line[0];
                if (lineFirstChar.Equals(START_LINE_CHAR)) // check if we have a tag for the ID
                {
                    Regex rx = new Regex(PATTERN);
                    string idString = rx.Match(line).Groups[1].Value;
                    string foundId = idString;

                    // if we have found the right id, then return the row number - else go on
                    if (foundId == id)
                        return i;
                }
            }
            return -1; // no end line found -> this will result in an error later on
        }

        /// <summary>
        /// Get the end line in the save file for the specific ID, such as a proivnce or a country.
        /// </summary>
        /// <param name="id">ID, such as province or country.</param>
        /// <param name="startLineInTheSaveFile">Start line of the ID in the save file.</param>
        /// <param name="END_LINE_TEXT">The text of the end line for this specific type of content, such as province or country.</param>
        /// <returns></returns>
        internal int getIdEndLine(string id, int startLineInTheSaveFile, in string END_LINE_TEXT)
        {
            // find the start line of provinces in the save file
            for (int i = startLineInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line == END_LINE_TEXT)
                    return i;
            }
            return -1; // no end line found -> this will result in an error later on
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
        /// 
        /// </summary>
        /// <param name="id">Start and line for the specific id (province)</param>
        /// <returns>Start line for the specific id (province)</returns>
        internal int startLineID(string id)
        {
            IDData _idData = statisticsData[id];
            return _idData.startLineInTheSaveFile;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>End line for the specific id (province)</returns>
        internal int endLineID(string id)
        {
            IDData _idData = statisticsData[id];
            return _idData.endLineInTheSaveFile;
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
        ///      Returns all the province IDs, country IDs etc.
        /// </summary>
        public string[] ids()
        {
            return statisticsData.Keys.ToArray();
        }

        /// <summary>
        ///      Returns data for a specific ID (province, country).
        /// </summary>
        public IDData dataForOneID(string id)
        {
            return statisticsData[id];
        }

        /// <summary>
        ///      Returns data for a all IDs (province, countries).
        /// </summary>
        public Dictionary<string, IDData> dataForAllIDs()
        {
            return statisticsData;
        }
    }
}