using Hs.Hypermint.WheelCreator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using Hs.Hypermint.WheelCreator.Models;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Hs.Hypermint.WheelCreator.Repo
{
    public class TextImage : ITextImageService
    {
        public async Task<MagickImage> GenerateCaptionAsync(WheelTextSetting setting)
        {
            var image = new MagickImage(MagickColors.Transparent, setting.Width, setting.Height);

            await Task.Run(() =>
            {
                var captionString = "caption:" + setting.PreviewText;

                image.Settings.FillColor =
                    new MagickColor(Converters.ColorConvert.ColorFromMediaColor(setting.TextColor));

                image.Settings.StrokeColor =
                    new MagickColor(Converters.ColorConvert.ColorFromMediaColor(setting.TextStrokeColor));

                image.Settings.StrokeWidth = setting.StrokeWidth;

                image.Settings.FontFamily = setting.FontName;

                image.Settings.TextGravity = (Gravity)Enum.Parse(typeof(Gravity), setting.Gravity);

                image.Read(captionString);

                if (setting.ArcAmount > 0)
                    image.Distort(DistortMethod.Arc, setting.ArcAmount);

                if (setting.ShadeOn)
                    image.Shade(setting.ShadeAzimuth, setting.ShadeElevation, false);

                var shadowColor = new MagickColor(Converters.ColorConvert.ColorFromMediaColor(setting.ShadowColor));

                image.Shadow(setting.ShadowX, setting.ShadowY, setting.ShadowSigma,
                    new Percentage(setting.ShadowPercentage), new MagickColor(shadowColor));

                image.RePage();

                if (setting.Trim)
                    image.Trim();

            });

            return image;

        }

        public string[] GetTextPresets()
        {
            var settingsPath = "preset\\wheel\\text\\";

            var textPresets = Directory.GetFiles(settingsPath, "*.xml");

            for (int i = 0; i < textPresets.Length; i++)
            {
                textPresets[i] = Path.GetFileName(textPresets[i]);
            }

            return textPresets;
        }

        public WheelTextSetting DeserializePreset(string presetFile)
        {
            if (!File.Exists(presetFile)) return null;

            using (TextReader sr = new StreamReader(presetFile))
            using (var reader = XmlReader.Create(sr))
            {
                var serializer = new XmlSerializer(typeof(WheelTextSetting));

                WheelTextSetting setting = (WheelTextSetting)serializer.Deserialize(reader);

                return setting;
            }
        }
    }
}
