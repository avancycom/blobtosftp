using FileTransfer.Library.Application.Commands.SftpCommands.TransferFilesFromBlobStorageToSftp;
using MediatR;
using Microsoft.Azure.Functions.Worker;

namespace FileTransfer.FromBlobStorageToSFTP;

public class FromBlobStorageToSFTPFunction(ISender mediator)
{
    private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [Function("from-blob-storage-to-sftp")]
    public async Task TransferFilesFromBlobStorageToSFTP(
        [TimerTrigger("%Timer%")] TimerInfo myTimer)
    {
        await _mediator.Send(new TransferFilesFromBlobStorageToSftpCommand());
    }
}