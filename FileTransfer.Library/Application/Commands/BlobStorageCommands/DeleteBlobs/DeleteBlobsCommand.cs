using MediatR;

namespace FileTransfer.Library.Application.Commands.BlobStorageCommands.DeleteBlobs;

internal sealed record DeleteBlobsCommand(List<string> FileNames) : IRequest;