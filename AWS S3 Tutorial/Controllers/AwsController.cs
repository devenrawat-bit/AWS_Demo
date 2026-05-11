using AWS_S3_Tutorial.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AWS_S3_Tutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AwsController : ControllerBase
    {
        private readonly IFileUpload _fileUpload;
        public AwsController(IFileUpload fileUpload)
        {
            _fileUpload = fileUpload;
        }

       [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var result = await _fileUpload.UploadFileAsync(file);

            if (result.Success)
            {
                return Ok(new { 
                    message = "File uploaded successfully", 
                    result.FileUrl,
                    result.FileName,
                    result.FileSize,
                    result.ContentType
                });
            }
            else
            {
                return StatusCode(500, new { 
                    message = "File upload failed", 
                    error = result.ErrorMessage 
                });
            }
        }
    }   
}
