using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Writer;

namespace PdfTools.Data
{
    [SupportedOSPlatform("browser")]
    public partial class PdfWorker
    {
        [JSExport]
        internal static string JobToWorker(string data)
        {
            try
            {
                if (Helpers.Json.TryGetModel<JobData>(data, out var m))
                {
                    Dictionary<int, PdfDocument> files = [];
                    foreach (var file in m.PdfFiles)
                        files[file.Key] = PdfDocument.Open(file.Value);

                    var builder = new PdfDocumentBuilder();
                    foreach (var item in m.Pages)
                    {
                        if (files.TryGetValue(item.PdfId, out var pdf))
                        {
                            builder.AddPage(pdf, item.PageNumber);
                        }
                    }

                    foreach (var item in files)
                        item.Value.Dispose();

                    if (builder.Pages.Count > 0)
                    {
                        var bytes = builder.Build();
                        builder.Dispose();
                        return Convert.ToBase64String(bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return string.Empty;
        }
    }
}
