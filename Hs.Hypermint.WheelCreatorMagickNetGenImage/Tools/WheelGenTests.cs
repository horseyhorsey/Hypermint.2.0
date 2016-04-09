using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.WheelCreator.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hs.Hypermint.WheelCreator.Tools.Tests
{
    [TestClass()]
    public class WheelGenTests
    {
        [TestMethod()]
        public void GenerateImageTest()
        {

            WheelGen.GenerateImage();

            //Assert.Fail();
        }

        [TestMethod()]
        public void GenerateLabelTest()
        {
            WheelGen.GenerateLabel();
        }

        [TestMethod()]
        public void GenerateLogoTest()
        {
            WheelGen.GenerateLogo(
                System.Drawing.Color.CornflowerBlue,
                System.Drawing.Color.DarkRed,
                System.Drawing.Color.Firebrick,
                System.Drawing.Color.Black,
                System.Drawing.Color.Black,
                 System.Drawing.Color.Black,
                border:true,
                BackgroundShadowOn: true,
                caption: true);
        }

        [TestMethod()]
        public void GenerateGradientBackground()
        {
            WheelGen.GradientBackground();
        }

        [TestMethod()]
        public void TiledText()
        {
            WheelGen.TiledText();
        }

        [TestMethod()]
        public void NeonText()
        {
            WheelGen.NeonSign();
        }

        [TestMethod()]
        public void TiledFont()
        {
            WheelGen.TiledFont();
        }
    }
}