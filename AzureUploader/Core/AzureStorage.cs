using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureUploader.Core.Helpers;
using AzureUploader.Core.Models;

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

        public async Task UploadAsync(string containerName, string blockName, Stream payload, CancellationToken cancellationToken)
        {
            var createdBlockIds = new List<string>();
            byte[] buffer = new byte[2 * 1024 * 1024];

            while (await payload.ReadAsync(buffer, 0, buffer.Length) > 0)
            {
                var blockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
                string uri = $"https://{_storageAccountName}.blob.core.windows.net/{containerName}/{blockName}?comp=block&blockid={blockId}";

                using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
                {
                    Content = (payload == null) ? null : new ByteArrayContent(buffer)
                })
                {
                    var now = DateTime.UtcNow;
                    httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                    httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");

                    // Add the authorization header.
                    httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                        _storageAccountName,
                        _storageAccountKey,
                        httpRequestMessage);

                    // Send the request.
                    using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                    {
                        if (httpResponseMessage.StatusCode == HttpStatusCode.Created)
                        {
                            createdBlockIds.Add(blockId);
                        }
                    }
                }
            }

            await CommitBlocks(containerName, blockName, createdBlockIds, cancellationToken);
        }

        private async Task CommitBlocks(string containerName, string blockName, List<string> blocks, CancellationToken cancellationToken)
        {
            string uri = $"https://{_storageAccountName}.blob.core.windows.net/{containerName}/{blockName}?comp=blocklist";

            var requestContent = XmlSerializerHelper.Serialize(new BlockList()
            {
                Latest = blocks
            });

            using (var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri)
            {
                Content = new StringContent(requestContent)
            })
            {
                var now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2018-03-28");

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
                    _storageAccountName,
                    _storageAccountKey,
                    httpRequestMessage);

                // Send the request.
                using (HttpResponseMessage httpResponseMessage = await new HttpClient().SendAsync(httpRequestMessage, cancellationToken))
                {
                    if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                    {

                    }
                }
            }
        }
    }
}
