using FileTransfer.Library.Application.Commands.BlobStorageCommands.DeleteBlobs;
using FileTransfer.Library.Application.Commands.BlobStorageCommands.DownloadBlobs;
using FileTransfer.Library.Common.Settings.SftpServerSettings;
using FluentFTP;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Renci.SshNet;

namespace FileTransfer.Library.Application.Commands.SftpCommands.TransferFilesFromBlobStorageToSftp;

internal sealed class TransferFilesFromBlobStorageToSftpCommandHandler : IRequestHandler<TransferFilesFromBlobStorageToSftpCommand>
{
    private readonly ISender _mediator;
    private readonly IOptions<SftpServerSettings> _sftpServerSettings;
    private readonly ILogger<TransferFilesFromBlobStorageToSftpCommandHandler> _logger;

    public TransferFilesFromBlobStorageToSftpCommandHandler(
        ISender mediator,
        IOptions<SftpServerSettings> sftpServerSettings,
        ILogger<TransferFilesFromBlobStorageToSftpCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _sftpServerSettings = sftpServerSettings ?? throw new ArgumentNullException(nameof(sftpServerSettings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(TransferFilesFromBlobStorageToSftpCommand request, CancellationToken cancellationToken)
    {
        Dictionary<string, Stream> downloadedFiles = await _mediator.Send(new DownloadBlobsCommand(), cancellationToken);

        List<string> uploadedFiles = _sftpServerSettings.Value.FileProtocol switch
        {
            "sftp" => await SftpHandler(downloadedFiles),
            "ftp" => await FtpHandler(downloadedFiles),
            _ => []
        };

        await _mediator.Send(new DeleteBlobsCommand(uploadedFiles), cancellationToken);
    }

    private async Task<List<string>> SftpHandler(Dictionary<string, Stream> files)
    {
        try
        {
            var connection = new ConnectionInfo(
               _sftpServerSettings.Value.Host,
               _sftpServerSettings.Value.Port,
               _sftpServerSettings.Value.Username,
               new PasswordAuthenticationMethod(_sftpServerSettings.Value.Username, _sftpServerSettings.Value.Password));

            using SftpClient sftpClient = new(connection);

            sftpClient.Connect();
            if (!sftpClient.IsConnected)
            {
                _logger.LogError("Failed to establish SFTP connection to '{server}' server.", _sftpServerSettings.Value.Host);
                return [];
            }

            List<string> uploadedFiles = new();
            foreach ((string fileName, Stream stream) in files)
            {
                sftpClient.UploadFile(stream, $"{_sftpServerSettings.Value.Directory}/{fileName}");
                uploadedFiles.Add(fileName);
            }

            sftpClient.Disconnect();
            return uploadedFiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return [];
        }
    }

    private async Task<List<string>> FtpHandler(Dictionary<string, Stream> files)
    {
        try
        {
            using FtpClient ftpClient = new(
                _sftpServerSettings.Value.Host,
                _sftpServerSettings.Value.Username,
                _sftpServerSettings.Value.Password,
                _sftpServerSettings.Value.Port);

            ftpClient.Connect();
            if (!ftpClient.IsConnected)
            {
                _logger.LogError("Failed to establish FTP connection to '{server}' server.", _sftpServerSettings.Value.Host);
                return [];
            }

            List<string> uploadedFiles = new();
            foreach ((string fileName, Stream stream) in files)
            {
                ftpClient.UploadStream(stream, $"{_sftpServerSettings.Value.Directory}/{fileName}", createRemoteDir: true);
                uploadedFiles.Add(fileName);
            }

            ftpClient.Disconnect();
            return uploadedFiles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return [];
        }
    }
}
