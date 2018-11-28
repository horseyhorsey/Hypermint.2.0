namespace Hs.Hypermint.ViewModel.Tests.Fixtures
{
    /// <summary>
    /// Fixture needs no attributes, just initialize what you need to test across classes marked with attribute
    /// </summary>
    public  class RealFixture
    {
        public IExample example;

        public RealFixture()
        {
            example = new Example();
        }
    }

    public interface IExample
    {

    }

    public class Example : IExample
    {

    }
}