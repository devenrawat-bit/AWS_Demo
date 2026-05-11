using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using AWS_S3_Tutorial.Data;
using AWS_S3_Tutorial.Entity;
using AWS_S3_Tutorial.Model;
using AWS_S3_Tutorial.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AWS_S3_Tutorial.Services.Implementation
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _environment;
        private readonly AwsSettings _awsSettings;
        private readonly IAmazonS3 _s3Client;
        private readonly AppDbContext _context;

        public FileUpload(IWebHostEnvironment environment, IOptions<AwsSettings> awsOptions, AppDbContext context)
        {
            _environment = environment;
            _awsSettings = awsOptions.Value;
            _context = context;

            var credentials = new BasicAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsSettings.Region)
            };
            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<FileUploadResponse> UploadFileAsync(IFormFile file)
        {
            try
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                using var stream = file.OpenReadStream();

                var request = new PutObjectRequest
                {
                    BucketName = _awsSettings.BucketName,
                    Key = fileName,
                    InputStream = stream,
                    ContentType = file.ContentType
                };

                var response = await _s3Client.PutObjectAsync(request);
                //here above the authentication is done automatically as above _s3Client has the credentials.

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    // Generate a Pre-signed URL (Valid for 1 hour)
                    // This works even if the bucket is private!
                    var expiryEvent = DateTime.Now.AddHours(1);
                    var fileUrl = _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest
                    {
                        BucketName = _awsSettings.BucketName,
                        Key = fileName,
                        Expires = expiryEvent
                    });

                    // Save to Database
                    var fileData = new FileData
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FilePath = fileName, // Storing the Key (FileName) instead of full URL for robustness
                        FileExtension = Path.GetExtension(file.FileName),
                        UploadedAt = DateTime.UtcNow.ToString("O")
                    };

                    _context.FileDatas.Add(fileData);
                    await _context.SaveChangesAsync();

                    return new FileUploadResponse
                    {
                        Success = true,
                        FileName = fileName,
                        FileUrl = fileUrl,
                        FileSize = file.Length,
                        ContentType = file.ContentType
                    };
                }

                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = "S3 returned a non-OK status code."
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
