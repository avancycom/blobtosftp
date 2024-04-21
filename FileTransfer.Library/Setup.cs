using Azure.Identity;
using FileTransfer.Library.Common;
using FileTransfer.Library.Common.Settings;
using FileTransfer.Library.Common.Settings.BlobSettings;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileTransfer.Library;

public static class Setup
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, HostBuilderContext context)
    {
        services.AddSettings();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.UseCredential(new DefaultAzureCredential());

            var blobSettings = context.Configuration.GetSection(AppDefaults.BlobSettingsSection).Get<BlobSettings>();
            clientBuilder.AddBlobServiceClient(blobSettings.ConnectionString);
        });

        return services;
    }
}
