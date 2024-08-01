using System.Text.Json.Serialization;

namespace FileTransfer.Library.Common.Models;

public class IpInfo
{
    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("region")]
    public string Region { get; set; }
}