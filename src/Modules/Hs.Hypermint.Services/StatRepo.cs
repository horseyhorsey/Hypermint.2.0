using Hypermint.Base.Interfaces;
using System;
using Hs.RocketLauncher.Statistics;
using System.IO;
using Hs.Hypermint.Services.Helpers;

namespace Hs.Hypermint.Services
{
    public enum GeneralStats
    {
        General,
        TopTen_Time_Played,
        TopTen_Times_Played,
        Top_Ten_Average_Time_Played,
        Number_of_Times_Played,
        Last_Time_Played,
        Average_Time_Played,
        Total_Time_Played
    }

    public class StatRepo : IStatsRepo
    {
        public Stat GetSingleGameStats(string statsPath, string systemName, string romName)
        {
            IniFile ini = new IniFile();
            ini.Load(statsPath + "\\" + systemName + ".ini");

            var i = ini.GetSection(romName);
            if (i == null)
                return new Stat();

            var gameStat = new Stat();
            gameStat.TimesPlayed = Convert.ToInt32(ini.GetKeyValue(romName, "Number_of_Times_Played"));
            gameStat.LastTimePlayed = Convert.ToDateTime(ini.GetKeyValue(romName, "Last_Time_Played"));

            var avgTime = TimeSpan.Parse(ini.GetKeyValue(romName, "Average_Time_Played")).Days;
            gameStat.AvgTimePlayed = new TimeSpan(0, 0, avgTime);
            
            var totalTime = TimeSpan.Parse(ini.GetKeyValue(romName, "Total_Time_Played")).Days;
            gameStat.TotalTimePlayed = new TimeSpan(0, 0, totalTime);            

            return gameStat;
        }

        public Stats GetStatsForSystem(string rlStatsIni)
        {
            var statList = new Stats();

            string sysStatIniName = Path.GetFileNameWithoutExtension(rlStatsIni);

            if (sysStatIniName == "desktop" || sysStatIniName == "Global Statistics")
                return statList;

            IniFile ini = new IniFile();
            string[] genStats = { "General", "TopTen_Time_Played", "TopTen_Times_Played", "Top_Ten_Average_Time_Played" };
            ini.Load(rlStatsIni);
            int count = ini.Sections.Count;
            string time = "";

            foreach (IniFile.IniSection s in ini.Sections)
            {
                string section = s.Name.ToString();
                if (genStats[0] != section)
                    if (genStats[1] != section)
                        if (genStats[2] != section)
                            if (genStats[3] != section)
                            {
                                if (section == GeneralStats.General.ToString())
                                {

                                }
                                else
                                {
                                    var t = new TimeSpan();
                                    var gameStat = new Stat();
                                    gameStat.Rom = section;

                                    gameStat._systemName = sysStatIniName;
                                    try
                                    {
                                        gameStat.TimesPlayed = Convert.ToInt32(ini.GetKeyValue(section, "Number_of_Times_Played"));

                                    }
                                    catch (Exception)
                                    {

                                    }

                                    try
                                    {
                                        gameStat.LastTimePlayed = Convert.ToDateTime(ini.GetKeyValue(section, "Last_Time_Played"));
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    try
                                    {

                                        //AvgTimePlayed = ;
                                        var avgTime = TimeSpan.Parse(ini.GetKeyValue(section, "Average_Time_Played")).Days;
                                        gameStat.AvgTimePlayed = new TimeSpan(0, 0, avgTime);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    try
                                    {
                                        var TotalTime = TimeSpan.Parse(ini.GetKeyValue(section, "Total_Time_Played")).Days;

                                        gameStat.TotalTimePlayed = new TimeSpan(0, 0, TotalTime);
                                        gameStat.TotalOverallTime = gameStat.TotalOverallTime + gameStat.TotalTimePlayed;


                                    }
                                    catch (Exception)
                                    {


                                    }

                                    statList.Add(new Stat
                                    {
                                        AvgTimePlayed = gameStat.AvgTimePlayed,
                                        LastTimePlayed = gameStat.LastTimePlayed,
                                        TimesPlayed = gameStat.TimesPlayed,
                                        Rom = gameStat.Rom,
                                        TotalTimePlayed = gameStat.TotalTimePlayed,
                                        TotalOverallTime = gameStat.TotalOverallTime,
                                        _systemName = gameStat._systemName
                                    });
                                }
                            }
            }

            return statList;
        }
             
    }
}
