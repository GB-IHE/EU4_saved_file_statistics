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
        /// Defines the start and end lines for the data section. Also defines the specific functions we want to use to gather the statistics.
        /// </summary>
        /// <param name="saveFile"></param>
        public CountryStatistics(SaveFile saveFile) : base(saveFile)
        {
            const string START_OF_THIS_SECTION_TEXT = "countries={";                   // without tabs, line 921682 in the example file
            const string START_OF_NEXT_SECTION_TEXT = "active_advisors={";               // Next section, line 3173769 in the example file

            List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics = new List<Func<string, string[]>>()
            {
                getGovernmentRank,
                getAverageUnrest,
                getAverageAutonomy,
                getEstimatedMonthlyIncome
            };

            createStatistics(START_OF_THIS_SECTION_TEXT, START_OF_NEXT_SECTION_TEXT, listOfMethodsUsedToGatherStatistics);
        }

        internal override void getAllIdsAndStartLinesFromSaveFile()
        {
            // on the form '	SWE={"
            const string PATTERN_OF_THE_ID = @"(.*?)=";  // take the line value and remove everything after "=" to get the id of the country (like SWE)
            const int LENGTH_OF_THE_COUNTRY_TAG = 3;

            // find the start line of country in the save file
            for (int i = startLineOfTheSectionInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, just go on to the next line
                    continue;

                // get rid of the tab space and the '=' and then check if this is a country tag, it must be of the lengt 3, upper case and non numeric
                Regex thingsArouondCountryTag = new Regex(@"\t(.*?)=");
                string possibleCountryTag = thingsArouondCountryTag.Match(line).Groups[1].Value;

                if (possibleCountryTag.Length == LENGTH_OF_THE_COUNTRY_TAG && IsAllUpper(possibleCountryTag) && !IsNumeric(possibleCountryTag))
                {
                    // ge the country ID and the add it together with the start line to the statistics data
                    Regex rx = new Regex(PATTERN_OF_THE_ID);
                    string id = rx.Match(line).Groups[1].Value;
                    addIDAndStartLineToTheStatisticsData(id, i);
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
