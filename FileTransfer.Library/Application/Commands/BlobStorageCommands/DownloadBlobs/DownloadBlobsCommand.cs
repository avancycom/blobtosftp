using MediatR;

namespace FileTransfer.Library.Application.Commands.BlobStorageCommands.DownloadBlobs;

internal sealed record DownloadBlobsCommand() : IRequest<Dictionary<string, Stream>>;