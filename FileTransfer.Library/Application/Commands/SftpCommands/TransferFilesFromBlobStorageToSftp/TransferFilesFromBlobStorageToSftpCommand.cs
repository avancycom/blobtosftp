using MediatR;

namespace FileTransfer.Library.Application.Commands.SftpCommands.TransferFilesFromBlobStorageToSftp;

public sealed record TransferFilesFromBlobStorageToSftpCommand() : IRequest;
