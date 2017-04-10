using Hypermint.Base.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hypermint.Base.Services
{
    /// <summary>
    /// Builds avi synth scripts.
    /// </summary>
    public class AviSynthScriptBuilder : ScriptBuilder
    {
        protected internal override string BuildScript(ScriptOptions options)
        {
            var i = 0;
            var videoFilesLength = options.VideoFiles.Length;

            options.vidname = new string[videoFilesLength];
            options.audioname = new string[videoFilesLength];
            options.audioDub = new string[videoFilesLength];
            options.trimName = new string[videoFilesLength];

            if (options.Overlay)
            {
                options.wheelName = new string[videoFilesLength];
                options.wheelNameAlpha = new string[videoFilesLength];
            }

            for (i = 0; i < videoFilesLength; i++)
            {
                // Image overlay video
                if (!options.Overlay)
                {
                    options.vidname[i] = "V" + i + " = ffvideosource(" + "\"" + options.VideoFiles[i] + "\"" + ")";
                    options.audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + options.VideoFiles[i] + "\"" + ")";
                    options.audioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + options.avisynthOption.StartFrame + "," + options.avisynthOption.EndFrame + ")";
                }
                else
                {
                    // Get the wheel file name
                    var name = Path.GetFileNameWithoutExtension(options.VideoFiles[i]);
                    var path = options.WheelPath + name;
                    var fullPath = "";

                    options.trimName = new string[options.VideoFiles.Length];

                    fullPath = path + ".png";
                    if (!File.Exists(fullPath))
                        fullPath = Path.GetFullPath(@"Resources/empty.png");


                    options.vidname[i] = "V" + i + " = ffvideosource(" + "\"" + options.VideoFiles[i] + "\"" + ")";
                    options.wheelName[i] = "W" + i + " = ImageSource(" + "\"" + fullPath + "\"" + ")";
                    options.wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" + fullPath + "\"" + ", pixel_type=" + "\"" + "RGB32" + "\""
                        + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")";
                    options.audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + options.VideoFiles[i] + "\"" + ")";

                    if (options.overlayResize)
                    {
                        options.wheelName[i] = "W" + i + " = ImageSource(" + "\"" + fullPath + "\""
                            + ")" + @".BilinearResize(" + options.avisynthOption.ResizeWidth + @"," + options.avisynthOption.ResizeHeight + ")";
                        options.wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\""
                            + fullPath + "\"" + ", pixel_type=" + "\""
                            + "RGB32" + "\"" + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")" +
                            @".BilinearResize(" + options.avisynthOption.ResizeWidth + @"," + options.avisynthOption.ResizeHeight + ")";
                    }


                    options.audioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + options.avisynthOption.StartFrame + ","
                        + options.avisynthOption.EndFrame + ").Overlay(" + "W" + i + "," + options.avisynthOption.WheelPosX + ","
                        + options.avisynthOption.WheelPosY + ", Wa" + i + ",opacity = 0.7)";
                    //.Overlay(img, 0, 440,imgalpha,opacity = 0.7)                    

                }

                i = 0;
                foreach (var item in options.VideoFiles)
                {
                    options.trimName[i] = SetTrimName(i, options.audioDub[i]);
                    i++;
                }

                var linesTotal = 0;
                if (!options.Overlay)
                    linesTotal = options.vidname.Length + options.trimName.Length + options.audioname.Length + options.audioDub.Length + 1;
                else
                    linesTotal = options.vidname.Length + options.trimName.Length +
                        options.audioname.Length + options.audioDub.Length + 1 + options.wheelName.Length + options.wheelNameAlpha.Length;

                var lineArray = new string[linesTotal];
                i = 0; var ii = 0;
                foreach (var videoName in options.vidname)
                {
                    lineArray[ii] = options.vidname[i];
                    ii++;

                    if (options.Overlay)
                    {
                        lineArray[ii] = options.wheelName[i];
                        ii++;
                        lineArray[ii] = options.wheelNameAlpha[i];
                        ii++;
                    }

                    lineArray[ii] = options.audioname[i];
                    ii++;
                    i++;
                }

                i = 0;

                var stringBuilder = new StringBuilder();
                foreach (var item in options.trimName)
                {
                    lineArray[ii] = options.trimName[i];
                    stringBuilder.Append("Trim" + i + ",");
                    i++;
                    ii++;
                }

                stringBuilder.Append(options.avisynthOption.DissolveAmount + ").FadeIn("
                    + options.avisynthOption.FadeIn + ").FadeOut(" + options.avisynthOption.FadeOut + ")");

                lineArray[ii] = "Dissolve(" + stringBuilder.ToString();

                var system = options.SystemName.Replace(' ', '_');

                if (!Directory.Exists(options.ExportScriptPath + system))
                    Directory.CreateDirectory(options.ExportScriptPath + system);

                var scriptFile = system + ".avs";

                i = 1;

                while (File.Exists(options.ExportScriptPath + system + "\\" + scriptFile))
                {
                    scriptFile = system + "(" + i + ")" + ".avs";
                    i++;
                }

                File.WriteAllLines(options.ExportScriptPath + system + "\\" + scriptFile, lineArray);

                return scriptFile.Replace(".avs", "");
            }

            return "";
        }

        protected internal override void CreateScriptOptions()
        {

        }
    }
}
