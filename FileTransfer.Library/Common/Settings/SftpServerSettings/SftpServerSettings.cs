namespace FileTransfer.Library.Common.Settings.SftpServerSettings;

public sealed class SftpServerSettings
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string Username { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string Directory { get; init; } = null!;
    public string FileProtocol { get; init; } = null!;
};