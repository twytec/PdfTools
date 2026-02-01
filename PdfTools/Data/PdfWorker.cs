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
                Console.WriteLine("Job incoming");
                if (Helpers.Json.TryGetModel<JobData>(data, out var m))
                {
                    Console.WriteLine($"Pages: {m.Pages.Count} - Files: {m.PdfFiles.Count}");

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

        //private static string LoadPdf(string msg)
        //{
        //    try
        //    {
        //        Console.WriteLine("LoadPdf called");
        //        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

        //        if (Helpers.Json.TryGetModel<LoadPdfMessage>(msg, out var m))
        //        {
        //            int index = _pdfs.Count;

        //            var pdf = PdfDocument.Open(m.Data, SkiaRenderingParsingOptions.Instance);
        //            pdf.AddSkiaPageFactory();
        //            _pdfs.Add(pdf);

        //            Console.WriteLine($"PDF loaded in {sw.Elapsed.TotalSeconds}, pages: {pdf.NumberOfPages}");

        //            List<PdfImage> list = [];

        //            for (int i = 1; i <= pdf.NumberOfPages; i++)
        //            {
        //                sw.Restart();

        //                var p = pdf.GetPage(i);
        //                float scale = 1.0f;
        //                int width = (int)p.Width;
        //                int height = (int)p.Height;

        //                if (p.Width > m.MaxWidth)
        //                {
        //                    scale = (float)(m.MaxWidth / p.Width);
        //                    width = (int)Math.Ceiling(p.Width * scale);
        //                    height = (int)(p.Height * scale);
        //                }

        //                Console.WriteLine($"Get page {i} in {sw.Elapsed.TotalSeconds}");
        //                sw.Restart();

        //                using var bitmap = pdf.GetPageAsSKBitmap(i, 1);
        //                using var img = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Webp, 100);
        //                list.Add(new PdfImage(index, i, Convert.ToBase64String(img.Span), bitmap.Width, bitmap.Height));

        //                Console.WriteLine($"Create image {i} in {sw.Elapsed.TotalSeconds}");
        //            }

        //            Console.WriteLine("LoadPdf finish");

        //            return Helpers.Json.GetJson(list);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }

        //    throw new InvalidOperationException("Invalid LoadPdf message");
        //}
    }
}
