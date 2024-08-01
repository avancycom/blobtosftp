# Configuration Settings

## Blob Storage Settings

These settings are used to configure the connection to a Blob storage service.

- **ConnectionString**: The connection string used to connect to the Blob storage service.
- **Container**: The name of the Blob storage container.
- **Directory**: The target directory within the Blob storage container, specified without a trailing '/'.

Example:
```json
{
  "BlobSettings:ConnectionString": "DefaultEndpointsProtocol=https;AccountName=accountName;AccountKey=accountKey;EndpointSuffix=core.windows.net",
  "BlobSettings:Container": "container-name",
  "BlobSettings:Directory": "target/directory"
}
```

## SFTP Server Settings

These settings are used to configure the connection to an SFTP server.

- **Host**: The hostname or IP address of the SFTP server.
- **Port**: The port number for the SFTP connection, typically 22.
- **Username**: The username used for authentication on the SFTP server.
- **Password**: The password used for authentication on the SFTP server.
- **Directory**: The target directory on the SFTP server, specified without a trailing '/'.
- **FileProtocol**: The file transfer protocol to be used, either 'sftp' or 'ftp'.

Example:
```json
{
  "SftpServerSettings:Host": "example.com",
  "SftpServerSettings:Port": 22,
  "SftpServerSettings:Username": "root",
  "SftpServerSettings:Password": "password123",
  "SftpServerSettings:Directory": "target/directory",
  "SftpServerSettings:FileProtocol": "sftp"
}
```