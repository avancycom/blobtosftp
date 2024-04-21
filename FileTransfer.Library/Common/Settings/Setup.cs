using FileTransfer.Library.Common.Settings.BlobSettings;
using FileTransfer.Library.Common.Settings.SftpServerSettings;
using Microsoft.Extensions.DependencyInjection;

namespace FileTransfer.Library.Common.Settings;

internal static class Setup
{
    internal static IServiceCollection AddSettings(this IServiceCollection services) =>
        services
            .ConfigureOptions<SftpServerSettingsDefaults>()
            .ConfigureOptions<BlobSettingsDefaults>();
}