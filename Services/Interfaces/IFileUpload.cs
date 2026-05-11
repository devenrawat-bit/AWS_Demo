using AWS_S3_Tutorial.Model;

namespace AWS_S3_Tutorial.Services.Interfaces
{
    public interface IFileUpload
    {
        /// <summary>
        /// This method will be responsible for uploading the file to the AWS S3 bucket.
        /// </summary>
        /// <param name="file"></param>
        /// <returns>This will return the file path of the uploaded file in the AWS S3 bucket</returns>
        Task<FileUploadResponse> UploadFileAsync(IFormFile file);
    }
}
