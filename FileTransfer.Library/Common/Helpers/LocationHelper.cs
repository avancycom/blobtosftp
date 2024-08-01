using FileTransfer.Library.Common.Models;
using System.Net.Http.Json;

namespace FileTransfer.Library.Common.Helpers;

public static class LocationHelper
{
    public static async Task<IpInfo> GetLocationInfoAsync()
    {
        using HttpClient httpClient = new();
        var ipInfo = await httpClient.GetFromJsonAsync<IpInfo>("https://ipinfo.io/json");
        return ipInfo ?? new IpInfo();
    }
}