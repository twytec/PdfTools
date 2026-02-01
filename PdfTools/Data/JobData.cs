using PdfTools.ViewModels;

namespace PdfTools.Data
{
    public class JobData
    {
        public Dictionary<int, byte[]> PdfFiles { get; set; } = [];
        public List<PdfPageViewModel> Pages { get; set; } = [];
    }
}
