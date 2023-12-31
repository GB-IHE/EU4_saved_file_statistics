using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EU4_saved_file_statistics
{
    public class PlayerStatistics : Statistics
    {
        /// <summary>
        /// Defines the start and end lines for the data section, all palyers in the save file and their countries. Also defines the specific functions we want to use to gather the statistics.
        /// </summary>
        /// <param name="saveFile"></param>
        public PlayerStatistics(SaveFile saveFile) : base(saveFile)
        {
            const string START_OF_THIS_SECTION_TEXT = "players_countries={";             // without tabs, line 147276 in the example file
            const string START_OF_NEXT_SECTION_TEXT = "gameplaysettings={";                // next section, line 921682 in the example file

            List<Func<string, string[]>> listOfMethodsUsedToGatherStatistics = new List<Func<string, string[]>>()
            {
                getPlayersCountries
            };

            createStatistics(START_OF_THIS_SECTION_TEXT, START_OF_NEXT_SECTION_TEXT, listOfMethodsUsedToGatherStatistics);
        }

        internal override void getAllIdsAndStartLinesFromSaveFile()
        {
            /*
             * On the form:
             *  "BadThad"
             *  "PRO"
             *  "Chrilsk"
	         *  "TUN"
             */

            const char START_LINE_CHAR = '"'; 
            // find the start line of players in the save file
            for (int i = startLineOfTheSectionInTheSaveFile; i < endLineOfTheSectionInTheSaveFile; i++)
            {
                string line = saveFile.getLineData(i);
                if (line.Length == 0) // if empty line, just go on to the next line
                    continue;

                char lineFirstChar = line.Trim()[0]; // remove white spaces in the string
                if (lineFirstChar.Equals(START_LINE_CHAR))
                {
                    // get the province ID and the add it together with the start line to the statistics data
                    string id = getTextBetweenQuotationMarks(line);
                    addIDAndStartLineToTheStatisticsData(id, i);
                    i++; // the next line will be the country tag (see the form above) so skip one line so that we get the next player for the next iteration
                } // end if
            } // end for
        }

        // Stats for a specific thing, i.e. government rank
        private string[] getPlayersCountries(string id)
        {
            const string HEADER = "Country";

            string line = saveFile.getLineData(startLineID(id) + 1); // The country is always specified after the player name, the id, in the save file
            string country = getTextBetweenQuotationMarks(line);

            return new string[2] { HEADER, country };
        }

    }
}
