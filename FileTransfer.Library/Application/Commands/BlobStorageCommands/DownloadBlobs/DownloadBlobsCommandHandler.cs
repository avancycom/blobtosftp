using Azure.Storage.Blobs;
using FileTransfer.Library.Common.Settings.BlobSettings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileTransfer.Library.Application.Commands.BlobStorageCommands.DownloadBlobs;

internal sealed class DownloadBlobsCommandHandler : IRequestHandler<DownloadBlobsCommand, Dictionary<string, Stream>>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IOptions<BlobSettings> _blobSettings;
    private readonly ILogger<DownloadBlobsCommandHandler> _logger;

    public DownloadBlobsCommandHandler(
        BlobServiceClient blobServiceClient,
        IOptions<BlobSettings> blobSettings,
        ILogger<DownloadBlobsCommandHandler> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _blobSettings = blobSettings ?? throw new ArgumentNullException(nameof(blobSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Dictionary<string, Stream>> Handle(DownloadBlobsCommand request, CancellationToken cancellationToken)
    {
        var blobStreams = new Dictionary<string, Stream>();
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.Value.Container);

        if (!containerClient.Exists(cancellationToken))
        {
            _logger.LogWarning("[Download Blob]: The Container '{container}' does not exist!", _blobSettings.Value.Container);
            return blobStreams;
        }

        string? blobPrefix = !string.IsNullOrEmpty(_blobSettings.Value.Directory) ? _blobSettings.Value.Directory : null;
        var blobs = containerClient.GetBlobsAsync(prefix: blobPrefix, cancellationToken: cancellationToken);

        await foreach (var blobItem in blobs)
        {
            var blobClient = containerClient.GetBlobClient(blobItem.Name);
            var blobDownloadInfo = await blobClient.DownloadAsync(cancellationToken);

            var stream = new MemoryStream();

            await blobDownloadInfo.Value.Content.CopyToAsync(stream, cancellationToken);
            stream.Seek(0, SeekOrigin.Begin);

            var blobName = blobItem.Name.Split('/').Last();
            blobStreams.Add(blobName, stream);
        }

        return blobStreams;
    }
}