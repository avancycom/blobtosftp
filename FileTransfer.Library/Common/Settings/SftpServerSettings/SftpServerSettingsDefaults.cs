using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FileTransfer.Library.Common.Settings.SftpServerSettings;

internal sealed class SftpServerSettingsDefaults : IConfigureOptions<SftpServerSettings>
{
    private readonly IConfiguration _configuration;
    public SftpServerSettingsDefaults(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(SftpServerSettings options)
    {
        var section = _configuration.GetSection(AppDefaults.SftpServerSettingsSection);

        if (section.Exists())
        {
            section.Bind(options);
        }
    }
}
