namespace PdfTools.Data
{
    public class PdfFile
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[] File { get; set; } = [];
    }
}
