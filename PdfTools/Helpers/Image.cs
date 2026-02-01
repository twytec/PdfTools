using SkiaSharp;
using Svg.Skia;

namespace PdfTools.Helpers
{
    public static class Image
    {
        public static byte[]? SvgToPng(string data, int width)
        {
            using SKSvg svg = SKSvg.CreateFromSvg(data);
            if (svg.Picture is not null)
            {
                var scaleX = width / svg.Picture.CullRect.Width;
                int height = (int)Math.Ceiling(svg.Picture.CullRect.Height * scaleX);
                var scaleY = height / svg.Picture.CullRect.Height;

                using MemoryStream ms = new();
                svg.Save(ms, SKColor.Empty, SKEncodedImageFormat.Png, 100, scaleX, scaleY);

                return ms.ToArray();
            }

            return null;
        }
    }
}
