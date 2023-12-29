using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    /// <summary>
    /// Statistics for countries.
    /// </summary>
    public class CountryStatistics : Statistics
    {
        /// <summary>
        ///     1) Get the specific start line and end line for the countries section in the save file.
        ///     2) Get the IDs for all the countries in the save file together with the start and end line of stat specific countries ID in the save file.
        ///     3) Add statistics for each of the countries.
        /// </summary>
        public CountryStatistics(SaveFile saveFile) : base(saveFile)
        {
            // Get the start and end line in the saved file for the countries section
            const string START_LINE_TEXT = "countries={";                   // without tabs, line 921682 in the example file
            const string END_LINE_TEXT = "active_advisors={";               // Next section, line 3173769 in the example file
            startLineOfTheSectionInTheSaveFile = getFirstLineOfDataSection(START_LINE_TEXT);
            endLineOfTheSectionInTheSaveFile = getLastLineOfDataSection(startLineOfTheSectionInTheSaveFile, END_LINE_TEXT) - 1;

            // Get all the country IDs from the save file as well as start and end line as well as start and end line for each ID    
            getAllCountryIdsAndStartLinesFromSaveFile();
            findAllEndLinesInTheSaveFileForAllIDs();

            // Get all the variables for each ID based on the methods that we use
            List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics = new List<Func<string, string[]>>()
            {
                getGovernmentRank,
                getAverageUnrest,
                getAverageAutonomy,
                getEstimatedMonthlyIncome
            };

            addStatisticsForAllIds(listOfMethodsUsedToGatherStatistics);
        }

        private void getAllCountryIdsAndStartLinesFromSaveFile()
        {
            // on the form '	SWE={"
            const string PATTERN_OF_THE_ID = @"(.*?)=";  // take the line value and remove everything after "=" to get the id of the country (like SWE)

            // find the start line of country in the save file
            for (int i = startLineOfTheSectionInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, just go on to the next line
                    continue;

                Regex thingsArouondCountryTag = new Regex(@"\t(.*?)=");
                string possibleCountryTag = thingsArouondCountryTag.Match(line).Groups[1].Value;

                if (possibleCountryTag.Length == 3 && IsAllUpper(possibleCountryTag) && ! IsNumeric(possibleCountryTag))
                {
                    Regex rx = new Regex(PATTERN_OF_THE_ID);
                    string id = rx.Match(line).Groups[1].Value;

                    // add it to the data struct and the data lsit
                    IDData _idData = new IDData();
                    _idData.id = id;
                    _idData.startLineInTheSaveFile = i;
                    statisticsData.Add(id, _idData);
                } // end if
            } // end for
        } // end void

        // Stats for a specific thing, i.e. government rank
        private string[] getGovernmentRank(string id)
        {
            const string HEADER = "Government rank";

            const string START_LINE_TEXT = "		government_rank=";
            const bool QUOTATION_AROUND_THE_DATA = false;
            string governmentRank = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, governmentRank };
        }
        private string[] getAverageUnrest(string id)
        {
            const string HEADER = "Average Unrest";

            const string START_LINE_TEXT = "		average_unrest=";
            const bool QUOTATION_AROUND_THE_DATA = false;
            string averageUnrest = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, averageUnrest };
        }
        private string[] getAverageAutonomy(string id)
        {
            const string HEADER = "Average Autonomy";

            const string START_LINE_TEXT = "		average_autonomy=";
            const bool QUOTATION_AROUND_THE_DATA = false;
            string averageAutonomy = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, averageAutonomy };
        }
        private string[] getEstimatedMonthlyIncome(string id)
        {
            const string HEADER = "Estimated Monthly Income";

            const string START_LINE_TEXT = "		estimated_monthly_income=";
            const bool QUOTATION_AROUND_THE_DATA = false;
            string estimatedMonthlyIncome = tagsWithStartPattern(id, START_LINE_TEXT, QUOTATION_AROUND_THE_DATA);

            return new string[2] { HEADER, estimatedMonthlyIncome };
        }
    }
}
