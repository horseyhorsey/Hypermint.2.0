using Hypermint.Base.Models;
using System.IO;
using System.Text;

namespace Hypermint.Base.Services
{
    public class AviSynthScripter : IAviSynthScripter
    {
        #region Fields
        private string[] vidname;
        private string[] audioname;
        private string[] wheelName;
        private string[] wheelNameAlpha;
        private string[] audioDub;
        private string[] trimName;
        #endregion

        /// <summary>
        /// Creates an Avi synth script
        /// </summary>
        /// <param name="VideoFiles">Array of videos</param>
        /// <param name="options">Avi synth options</param>
        /// <param name="systemName">system to create for</param>
        /// <param name="overlay">use wheel overlay?</param>
        /// <param name="overlayResize">resize overlay?</param>
        /// <param name="wheelPath">path to wheel images</param>
        /// <param name="exportScriptPath">where to export the avi synth script</param>
        /// <returns></returns>
        public string CreateScript(string[] VideoFiles, AviSynthOption options,string systemName,
            bool overlay = false,bool overlayResize = false, string wheelPath = "", string exportScriptPath = @"exports\videos\")
        {
            var i = 0;
            vidname = new string[VideoFiles.Length];
            audioname = new string[VideoFiles.Length];
            audioDub = new string[VideoFiles.Length];
            trimName = new string[VideoFiles.Length];

            if (overlay)
            {
                wheelName = new string[VideoFiles.Length];
                wheelNameAlpha = new string[VideoFiles.Length];
            }
                        
            for (i = 0; i < VideoFiles.Length; i++)
            {
                // Image overlay video
                if (!overlay)
                {
                    vidname[i] = "V" + i + " = ffvideosource(" + "\"" + VideoFiles[i] + "\"" + ")";
                    audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + VideoFiles[i] + "\"" + ")";
                    audioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + options.StartFrame + "," + options.EndFrame + ")";                    
                }
                else
                {
                    // Get the wheel file name
                    var name = Path.GetFileNameWithoutExtension(VideoFiles[i]);
                    var path = wheelPath + name;
                    var fullPath = "";

                    trimName = new string[VideoFiles.Length];

                    fullPath = path + ".png";
                    if (!File.Exists(fullPath))
                        fullPath = Path.GetFullPath(@"Resources/empty.png");


                    vidname[i] = "V" + i + " = ffvideosource(" + "\"" + VideoFiles[i] + "\"" + ")";
                    wheelName[i] = "W" + i + " = ImageSource(" + "\"" + fullPath + "\"" + ")";
                    wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" + fullPath + "\"" + ", pixel_type=" + "\"" + "RGB32" + "\""
                        + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")";
                    audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + VideoFiles[i] + "\"" + ")";

                    if (overlayResize)
                    {
                        wheelName[i] = "W" + i + " = ImageSource(" + "\"" + fullPath + "\"" 
                            + ")" + @".BilinearResize(" + options.ResizeWidth + @"," + options.ResizeHeight + ")";
                        wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" 
                            + fullPath + "\"" + ", pixel_type=" + "\""
                            + "RGB32" + "\"" + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")" +
                            @".BilinearResize(" + options.ResizeWidth + @"," + options.ResizeHeight + ")";
                    }


                    audioDub[i] = "AudioDub" + "(V" + i + ",A" + i + ").Trim(" + options.StartFrame + ","
                        + options.EndFrame + ").Overlay(" + "W" + i + "," + options.WheelPosX + ","
                        + options.WheelPosY + ", Wa" + i + ",opacity = 0.7)";
                    //.Overlay(img, 0, 440,imgalpha,opacity = 0.7)                    

                }                                                

            }

            i = 0;

            foreach (var item in VideoFiles)
            {
                trimName[i] = SetTrimName(i, audioDub[i]);
                i++;
            }

            var linesTotal = 0;
            if (!overlay)
                linesTotal = vidname.Length + trimName.Length + audioname.Length + audioDub.Length + 1;
            else
                linesTotal = vidname.Length + trimName.Length + 
                    audioname.Length + audioDub.Length + 1 + wheelName.Length + wheelNameAlpha.Length;

            var lineArray = new string[linesTotal];
            i=0; var ii = 0;
            foreach (var videoName in vidname)
            {
                lineArray[ii] = vidname[i];
                ii++;

                if (overlay)
                {
                    lineArray[ii] = wheelName[i];
                    ii++;
                    lineArray[ii] = wheelNameAlpha[i];
                    ii++;
                }

                lineArray[ii] = audioname[i];
                ii++;
                i++;
            }

            i = 0;

            var stringBuilder = new StringBuilder();
            foreach (var item in trimName)
            {
                lineArray[ii] = trimName[i];
                stringBuilder.Append("Trim" + i + ",");
                i++;
                ii++;
            }

            stringBuilder.Append(options.DissolveAmount + ").FadeIn(" 
                + options.FadeIn + ").FadeOut(" + options.FadeOut + ")");

            lineArray[ii] = "Dissolve(" + stringBuilder.ToString();

            var system = systemName.Replace(' ', '_');

            if (!Directory.Exists(exportScriptPath + system))
                Directory.CreateDirectory(exportScriptPath + system);
            
            var scriptFile = system + ".avs";
            
            i = 1;

            while (File.Exists(exportScriptPath + system + "\\" + scriptFile))
            {
                scriptFile = system + "(" + i + ")" + ".avs";                
                i++;
            }

            File.WriteAllLines(exportScriptPath + system + "\\" + scriptFile, lineArray);

            return scriptFile.Replace(".avs","");
        }

        /// <summary>
        /// Sets a trim name for avi synth
        /// </summary>
        /// <param name="index"></param>
        /// <param name="audioDub"></param>
        /// <returns></returns>
        private string SetTrimName(int index, string audioDub)
        {
            return "Trim" + index + " = " + audioDub;
        }
    }
    
}
