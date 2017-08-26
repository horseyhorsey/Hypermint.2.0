using System;
using System.Diagnostics;
using System.IO;

namespace Hs.Hypermint.VideoEdit.Helpers
{
    public static class VideoHelper
    {
        public static void TrimVideoRange(string ffmpeg, string inputFile, string outputFile, TimeSpan start, TimeSpan end)
        {
            var startStr = start.ToString(@"hh\:mm\:ss");
            var endStr = end.ToString(@"hh\:mm\:ss");

            int i = 0;
            var startInfo = new ProcessStartInfo(ffmpeg + "\\ffmpeg.exe");
            var fullPath = Path.GetDirectoryName(inputFile);
            var name = Path.GetFileName(inputFile);
            var ext = Path.GetExtension(name);
            var nameNoExt = Path.GetFileNameWithoutExtension(name);
            var outputNewFile = fullPath + "\\" + name;

            if (File.Exists(outputNewFile))
            {
                var incrementout = nameNoExt + $"_{i}{ext}";
                while (File.Exists(fullPath + "\\" + incrementout))
                {
                    i++;
                    incrementout = nameNoExt + $"_{i}{ext}";
                }

                outputNewFile = fullPath + "\\" + incrementout;
            }

            inputFile = "\"" + inputFile + "\"";
            outputNewFile = "\"" + outputNewFile + "\"";


            //startInfo.Arguments = $"-i {inputFile} -vcodec copy -acodec copy -ss {start.ToString()} -to {end.ToString()} {outputNewFile}";

            //Transcode
            startInfo.Arguments = $"-ss {start.ToString()} -i {inputFile} -vcodec copy -acodec copy -t {(end - start).ToString()} {outputNewFile}";

            //ffmpeg - ss 00:08:00 - i Video.mp4 - ss 00:01:00 - t 00:01:00 - c copy VideoClip.mp4
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            Process.Start(startInfo).WaitForExit();
        }
    }
}
