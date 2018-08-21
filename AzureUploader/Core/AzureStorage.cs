using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AzureUploader.Core.Helpers;

namespace AzureUploader.Core
{
    public class AzureStorage
    {
        private readonly string _storageAccountName;
        private readonly string _storageAccountKey;

        public AzureStorage(string storageAccountName, string storageAccountKey)
        {
            _storageAccountName = storageAccountName;
            _storageAccountKey = storageAccountKey;
        }

        public async Task<bool> UploadAsync(string containerName, string blockName, byte[] payload, CancellationToken cancellationToken)
        {
            string uri = $"https://{_storageAccountName}.blob.core.windows.net/{containerName}/{blockName}";

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = (payload == null) ? null : new ByteArrayContent(payload)
            })
            {
                var now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");
                httpRequestMessage.Headers.Add("x-ms-blob-type", "BlockBlob");

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                    _storageAccountName,
                    _storageAccountKey,
                    httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    // If successful (status code = 200), 
                    //   parse the XML response for the container names.
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return true;
                    }

                    return false;
                }
            }
        }
    }
}
