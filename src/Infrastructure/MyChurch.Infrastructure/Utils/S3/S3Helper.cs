using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyChurch.Infrastructure.Utils.S3
{
    public class S3Settings
    {
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }

    internal class S3Helper : IS3Helper
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3Settings _settings;
        private readonly ILogger<S3Helper> _logger;

        public S3Helper(IAmazonS3 s3Client, S3Settings settings, ILogger<S3Helper> logger)
        {
            _settings = settings;
            
            _logger = logger;

            var credentials = new BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey);
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_settings.Region)
            };

            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = fileName,
                InputStream = fileStream,
                ContentType = contentType
            };

            var response = await _s3Client.PutObjectAsync(putRequest, cancellationToken);
            _logger.LogInformation("File uploaded to S3 with key: {FileName}", fileName);

            return fileName;
        }

        public async Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(getRequest, cancellationToken);
            _logger.LogInformation("File downloaded from S3 with key: {FileName}", fileName);

            return response.ResponseStream;
        }

        public async Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = fileName
            };

            await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
            _logger.LogInformation("File deleted from S3 with key: {FileName}", fileName);
        }

        public async Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default)
        {
            var url = $"https://{_settings.BucketName}.s3.{_settings.Region}.amazonaws.com/{fileName}";
            _logger.LogInformation("Generated URL for S3 file: {FileName}", fileName);
            return await Task.FromResult(url);
        }
    }
}