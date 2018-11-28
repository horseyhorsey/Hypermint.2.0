using System.Windows.Media;

namespace Hypermint.Base.Interfaces
{
    public interface IPdfService
    {
        int GetNumberOfPdfPages(string pdfFile);

        ImageSource GetPage(string ghostScriptPath, string pdfFile, int pageNumber);
    }
}
