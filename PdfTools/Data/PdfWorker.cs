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
                            var page = pdf.GetPage(item.PageNumber);
                            var pb = builder.AddPage(page.Width, page.Height);
                            pb.CopyFrom(page);

                            if (item.Signatures.Count > 0)
                            {
                                AddSignatures(item, pb);
                            }
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

        private static void AddSignatures(ViewModels.PdfPageViewModel vm, PdfPageBuilder builder)
        {
            foreach (var item in vm.Signatures)
            {
                var bytes = Convert.FromBase64String(item.ImageData);
                double x = builder.PageSize.Width * item.X;
                double y = builder.PageSize.Height * item.Y;
                double width = builder.PageSize.Width * item.Width;
                double height = builder.PageSize.Height * item.Height;

                //from the coordinate system at the top left to the bottom left
                y = builder.PageSize.Height - y - height;

                UglyToad.PdfPig.Core.PdfPoint bottomLeft = new(x, y);
                UglyToad.PdfPig.Core.PdfPoint topRight = new(x + width, y + height);
                var rect = new UglyToad.PdfPig.Core.PdfRectangle(bottomLeft, topRight);
                builder.AddPng(bytes, rect);
            }
        }
    }
}
