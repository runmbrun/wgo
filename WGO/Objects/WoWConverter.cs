using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WGO
{
    static public class WoWConverter
    {
        /// <summary>
        /// Converts a number from Blizzard's web site into a class type
        /// </summary>
        /// <param name="i">the numeric value of a wow class</param>
        /// <returns>string value of the number passed in</returns>
        public static string ConvertClass(int i)
        {
            string converted = string.Empty;

            switch (i)
            {
                case 1:
                    converted = "Warrior";
                    break;
                case 2:
                    converted = "Paladin";
                    break;
                case 3:
                    converted = "Hunter";
                    break;
                case 4:
                    converted = "Rogue";
                    break;
                case 5:
                    converted = "Priest";
                    break;
                case 6:
                    converted = "Death Knight";
                    break;
                case 7:
                    converted = "Shaman";
                    break;
                case 8:
                    converted = "Mage";
                    break;
                case 9:
                    converted = "Warlock";
                    break;
                case 10:
                    converted = "Monk";
                    break;
                case 11:
                    converted = "Druid";
                    break;
                case 12:
                    converted = "Demon Hunter";
                    break;
                default:
                    converted = "error: " + i;
                    break;
            }

            return converted;
        }

        /// <summary>
        /// Converts a number from Blizzard's web site into a class type
        /// </summary>
        /// <param name="i">the numeric value of a wow class</param>
        /// <returns>string value of the number passed in</returns>
        public static int ConvertClass(string i)
        {
            int converted = -1;

            switch (i)
            {
                case "Warrior":
                    converted = 1;
                    break;
                case "Paladin":
                    converted = 2;
                    break;
                case "Hunter":
                    converted = 3;
                    break;
                case "Rogue":
                    converted = 4;
                    break;
                case "Priest":
                    converted = 5;
                    break;
                case "Death Knight":
                    converted = 6;
                    break;
                case "Shaman":
                    converted = 7;
                    break;
                case "Mage":
                    converted = 8;
                    break;
                case "Warlock":
                    converted = 9;
                    break;
                case "Monk":
                    converted = 10;
                    break;
                case "Druid":
                    converted = 11;
                    break;
                case "Demon Hunter":
                    converted = 12;
                    break;
            }

            return converted;
        }

        /// <summary>
        /// Converts a number from Blizzard's web site into a Race type
        /// </summary>
        /// <param name="i">The numeric value of a race passed in</param>
        /// <returns>string value of the number passed in</returns>
        public static string ConvertRace(int i)
        {
            string converted = string.Empty;

            switch (i)
            {
                case 1:
                    converted = "Human";
                    break;
                case 2:
                    converted = "Orc";
                    break;
                case 3:
                    converted = "Dwarf";
                    break;
                case 4:
                    converted = "Night Elf";
                    break;
                case 5:
                    converted = "Undead";
                    break;
                case 6:
                    converted = "Tauren";
                    break;
                case 7:
                    converted = "Gnome";
                    break;
                case 8:
                    converted = "Troll";
                    break;
                case 9:
                    converted = "Goblin";
                    break;
                case 10:
                    converted = "Blood Elf";
                    break;
                case 11:
                    converted = "Draenei";
                    break;
                case 22:
                    converted = "Worgen";
                    break;
                case 26:
                    converted = "Pandaren";
                    break;
                default:
                    converted = "error";
                    break;
            }

            return converted;
        }

        /// <summary>
        /// Converts a number from Blizzard's web site into a Race type
        /// </summary>
        /// <param name="i">The numeric value of a race passed in</param>
        /// <returns>string value of the number passed in</returns>
        public static string ConvertSlot(int i)
        {
            string converted = string.Empty;

            switch (i)
            {
                case 1:
                    converted = "Human";
                    break;
                
                default:
                    converted = "error";
                    break;
            }

            return converted;
        }
    }
}