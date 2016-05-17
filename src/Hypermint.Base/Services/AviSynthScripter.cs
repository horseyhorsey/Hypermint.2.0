using Hypermint.Base.Models;
using System.IO;
using System.Text;

namespace Hypermint.Base.Services
{
    public class AviSynthScripter : IAviSynthScripter
    {
        private string[] vidname;
        private string[] audioname;
        private string[] wheelName;
        private string[] wheelNameAlpha;
        private string[] audioDub;
        private string[] trimName;

        public void CreateScript(string[] VideoFiles, AviSynthOption options,string systemName,
            bool overlay = false,bool overlayResize = false, string wheelPath = "", string exportScriptPath = @"exports\aviSynth")
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

                    trimName = new string[VideoFiles.Length];

                    vidname[i] = "V" + i + " = ffvideosource(" + "\"" + VideoFiles[i] + "\"" + ")";
                    wheelName[i] = "W" + i + " = ImageSource(" + "\"" + wheelPath + name + ".png" + "\"" + ")";
                    wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" + wheelPath + name + 
                        ".png" + "\"" + ", pixel_type=" + "\"" + "RGB32" + "\""
                        + ").ShowAlpha(pixel_type=" + "\"" + "RGB32" + "\"" + ")";
                    audioname[i] = "A" + i + " = ffaudiosource(" + "\"" + VideoFiles[i] + "\"" + ")";

                    if (overlayResize)
                    {
                        wheelName[i] = "W" + i + " = ImageSource(" + "\"" + wheelPath + name + ".png" + "\"" 
                            + ")" + @".BilinearResize(" + options.ResizeWidth + @"," + options.ResizeHeight + ")";
                        wheelNameAlpha[i] = "Wa" + i + " = ImageSource(" + "\"" 
                            + wheelPath + name + ".png" + "\"" + ", pixel_type=" + "\""
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

            if (!Directory.Exists(exportScriptPath))
                Directory.CreateDirectory(exportScriptPath);

            var scriptFile = systemName + ".avs";

            i = 1;

            while (File.Exists(exportScriptPath + scriptFile))
            {
                scriptFile = systemName + "(" + i + ")" + ".avs";
                i++;
            }

            File.WriteAllLines(exportScriptPath + scriptFile, lineArray);
        }

        private string SetTrimName(int index, string audioDub)
        {
            return "Trim" + index + " = " + audioDub;
        }
    }
    
}
