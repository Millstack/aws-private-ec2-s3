using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace millstack_ec2.Controllers.AWS_log
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string s3Bucket = "millstack-s3-log-bucket";

        public LogController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadLog([FromBody] string logContent)
        {
            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = s3Bucket,
                    Key = $"logs/{DateTime.UtcNow:yyyy-MM-dd-HHmm}.txt",
                    ContentBody = logContent
                };

                var response = await _s3Client.PutObjectAsync(request);
                return Ok(new { Message = "Log saved!", RequestId = response.ResponseMetadata.RequestId });
            }
            catch (AmazonS3Exception e)
            {
                // This captures "Access Denied" or "NoSuchBucket" errors
                return StatusCode((int)e.StatusCode, new { Error = e.Message, Detail = "Check IAM Permissions!" });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
