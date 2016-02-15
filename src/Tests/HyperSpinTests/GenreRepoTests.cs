using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hs.Hypermint.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Hs.Hypermint.Services.Tests
{
    [TestClass()]
    public class GenreRepoTests
    {
        [TestMethod()]
        public void PopulateGenresTest()
        {
            var genreRepo = new GenreRepo();

            genreRepo.GenreList = new List<string>();

            genreRepo.PopulateGenres(@"I:\Hyperspin\Databases\Atari 7800\Genre.xml");

            foreach (var item in genreRepo.GenreList)
            {
                Trace.WriteLine(item);
            }
        }
    }
}