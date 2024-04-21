using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FileTransfer.Library.Common.Settings.BlobSettings;

internal sealed class BlobSettingsDefaults : IConfigureOptions<BlobSettings>
{
    private readonly IConfiguration _configuration;
    public BlobSettingsDefaults(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(BlobSettings options)
    {
        var section = _configuration.GetSection(AppDefaults.BlobSettingsSection);
        if (section.Exists())
        {
            section.Bind(options);
        }
    }
}
