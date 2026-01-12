namespace MyChurch.Infrastructure.Utils.S3
{
    public interface IS3Helper
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string fileName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);
        Task<string> GetFileUrlAsync(string fileName, CancellationToken cancellationToken = default);
        Task CheckConnectionAsync(CancellationToken cancellationToken = default);
    }
}
