namespace AWS_S3_Tutorial.Entity
{
    public class FileData
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string UploadedAt { get; set; }
        public string FileExtension { get; set; }
    }
}
