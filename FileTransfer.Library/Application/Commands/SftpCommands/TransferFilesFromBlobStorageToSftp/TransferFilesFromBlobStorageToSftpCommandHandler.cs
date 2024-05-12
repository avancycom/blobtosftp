using FileTransfer.Library.Application.Commands.BlobStorageCommands.DeleteBlobs;
using FileTransfer.Library.Application.Commands.BlobStorageCommands.DownloadBlobs;
using FileTransfer.Library.Common.Settings.SftpServerSettings;
using FluentFTP;
using MediatR;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace FileTransfer.Library.Application.Commands.SftpCommands.TransferFilesFromBlobStorageToSftp;

internal sealed class TransferFilesFromBlobStorageToSftpCommandHandler : IRequestHandler<TransferFilesFromBlobStorageToSftpCommand>
{
    private readonly ISender _mediator;
    private readonly IOptions<SftpServerSettings> _sftpServerSettings;

    public TransferFilesFromBlobStorageToSftpCommandHandler(
        ISender mediator,
        IOptions<SftpServerSettings> sftpServerSettings)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _sftpServerSettings = sftpServerSettings ?? throw new ArgumentNullException(nameof(sftpServerSettings));
    }

    public async Task Handle(TransferFilesFromBlobStorageToSftpCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, Stream> files = await _mediator.Send(new DownloadBlobsCommand(), cancellationToken);

        switch (_sftpServerSettings.Value.FileProtocol)
        {
            case "sftp":
                await SftpHandler(files);
                break;
            case "ftp":
                await FtpHandler(files);
                break;
        }

        await _mediator.Send(new DeleteBlobsCommand(files.Keys.ToList()), cancellationToken);
    }

    private async Task SftpHandler(Dictionary<string, Stream> files)
    {
        var connection = new ConnectionInfo(
           _sftpServerSettings.Value.Host,
           _sftpServerSettings.Value.Port,
           _sftpServerSettings.Value.Username,
           new PasswordAuthenticationMethod(_sftpServerSettings.Value.Username, _sftpServerSettings.Value.Password));

        using SftpClient sftpClient = new(connection);
        sftpClient.Connect();

        foreach ((string fileName, Stream stream) in files)
        {
            sftpClient.UploadFile(stream, $"{_sftpServerSettings.Value.Directory}/{fileName}");
        }

        sftpClient.Disconnect();

    }

    private async Task FtpHandler(Dictionary<string, Stream> files)
    {
        using FtpClient ftpClient = new(
            _sftpServerSettings.Value.Host,
            _sftpServerSettings.Value.Username,
            _sftpServerSettings.Value.Password,
            _sftpServerSettings.Value.Port);

        ftpClient.Connect();

        foreach ((string fileName, Stream stream) in files)
        {
            ftpClient.UploadStream(stream, $"{_sftpServerSettings.Value.Directory}/{fileName}", createRemoteDir: true);
        }

        ftpClient.Disconnect();
    }
}
