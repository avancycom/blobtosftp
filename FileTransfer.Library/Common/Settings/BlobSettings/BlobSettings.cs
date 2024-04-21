namespace FileTransfer.Library.Common.Settings.BlobSettings;

internal sealed class BlobSettings
{
    public string ConnectionString { get; init; } = null!;
    public string Container { get; init; } = null!;
    public string Directory { get; init; } = null!;
}