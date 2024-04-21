using FileTransfer.Library.Application.Commands.BlobStorageCommands.DeleteBlobs;
using FileTransfer.Library.Application.Commands.BlobStorageCommands.DownloadBlobs;
using FileTransfer.Library.Common.Settings.SftpServerSettings;
using MediatR;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace FileTransfer.Library.Application.Commands.SftpCommands.TransferFilesFromBlobStorageToSftp;

internal sealed class TransferFilesFromBlobStorageToSftpCommandHandler : IRequestHandler<TransferFilesFromBlobStorageToSftpCommand>
{
    private readonly ISender _mediator;
    private readonly IOptions<SftpServerSettings> _sftpServerSettings;
    private readonly ConnectionInfo _connectionInfo;

    public TransferFilesFromBlobStorageToSftpCommandHandler(
        ISender mediator,
        IOptions<SftpServerSettings> sftpServerSettings)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _sftpServerSettings = sftpServerSettings ?? throw new ArgumentNullException(nameof(sftpServerSettings));

        _connectionInfo = new ConnectionInfo(
            _sftpServerSettings.Value.Host,
            _sftpServerSettings.Value.Port,
            _sftpServerSettings.Value.Username,
            new PasswordAuthenticationMethod(
            _sftpServerSettings.Value.Username,
            _sftpServerSettings.Value.Password));
    }

    public async Task Handle(TransferFilesFromBlobStorageToSftpCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, Stream> files = await _mediator.Send(new DownloadBlobsCommand(), cancellationToken);

        using SftpClient sftpClient = new(_connectionInfo);
        sftpClient.Connect();

        foreach ((string fileName, Stream stream) in files)
        {
            sftpClient.UploadFile(stream, $"{_sftpServerSettings.Value.Directory}/{fileName}");
        }

        sftpClient.Disconnect();

        await _mediator.Send(new DeleteBlobsCommand(files.Keys.ToList()), cancellationToken);
    }
}
