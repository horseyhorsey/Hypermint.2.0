﻿using Hypermint.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Hs.Hypermint.GameLaunch
{
    public class GameLaunch : IGameLaunch
    {
        private string _rocketLaunchParams = "";

        public void RocketLaunchGame(string RlPath, string systemName, string RomName, string HsPath)
        {
            var hypermintExe = Application.ResourceAssembly.EscapedCodeBase.ToString();
            hypermintExe = hypermintExe.Replace(@"file:///", "");
            hypermintExe = hypermintExe.Replace(@"/", @"\");
            hypermintExe = "\"" + hypermintExe + "\"";
                                   
            if (Directory.Exists(RlPath))
            {
                try
                {
                    System.Diagnostics.Process.Start(RlPath +
                        "\\Rocketlauncher.exe", "-s " + "\"" + systemName + "\"" + " -r " + "\"" + RomName + "\""
                        + " -f " + HsPath + "\\HyperSpin.exe"
                        + " -p " + "HyperSpin");
                }
                catch(Exception e)
                {

                }
            }
        }
        
        //        <MenuItem Header="Pause" Click="RLModeClick"/>
        //        <MenuItem Header="MultiGame" Click="RLModeClick"/>
        //        <MenuItem Header="Fade" Click="RLModeClick"/>
        //        <MenuItem Header="Fade7z" Click="RLModeClick"/>
        ///

        public void RocketLaunchGameWithMode(string RlPath, 
            string systemName, string RomName, string mode)
        {
            if (Directory.Exists(RlPath))
            {
                try
                {
                    System.Diagnostics.Process.Start(RlPath + 
                        "\\Rocketlauncher.exe", 
                        "-s " + "\"" + systemName + "\"" + " -r " + "\"" + RomName + "\"" +
                        //" -f " + HsPath + "\\HyperSpin.exe" +
                        " -m " + mode + " -p hyperspin");
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
