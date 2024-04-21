using Azure.Storage.Blobs;
using FileTransfer.Library.Common.Settings.BlobSettings;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FileTransfer.Library.Application.Commands.BlobStorageCommands.DeleteBlobs;

internal sealed class DeleteBlobsCommandHandler : IRequestHandler<DeleteBlobsCommand>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IOptions<BlobSettings> _blobSettings;
    private readonly ILogger<DeleteBlobsCommandHandler> _logger;

    public DeleteBlobsCommandHandler(
        BlobServiceClient blobServiceClient,
        IOptions<BlobSettings> blobSettings,
        ILogger<DeleteBlobsCommandHandler> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _blobSettings = blobSettings ?? throw new ArgumentNullException(nameof(blobSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(DeleteBlobsCommand request, CancellationToken cancellationToken)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_blobSettings.Value.Container);
        if (!containerClient.Exists(cancellationToken))
        {
            _logger.LogWarning("[Delete Blob]: The Container '{container}' does not exist!", _blobSettings.Value.Container);
            return;
        }

        request.FileNames.ForEach(async fileName =>
        {
            var blobPath = $"{_blobSettings.Value.Directory}/{fileName}";
            var deleteResponse = await containerClient.DeleteBlobIfExistsAsync(blobPath, cancellationToken: cancellationToken);

            if (!deleteResponse.Value)
            {
                _logger.LogWarning("[Delete Blob]: Blob '{blobPath}' does not exist or could not be deleted.", blobPath);
            }
        });
    }
}