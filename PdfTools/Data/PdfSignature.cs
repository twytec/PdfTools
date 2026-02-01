namespace PdfTools.Data
{
    public class PdfSignature
    {
        public string ImageData { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public string GetSrc()
        {
            return $"data:image/png;base64,{ImageData}";
        }
    }
}
