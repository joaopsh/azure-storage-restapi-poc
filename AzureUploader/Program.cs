using AzureUploader.Core;
using AzureUploader.Core.Helpers;
using AzureUploader.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AzureUploader
{
    class Program
    {
        private static string _storageAccountName = "affiliation";
        private static string _storageAccountKey = "ia2eOnsQer0FeDomEEyuBm+eoiWTavYUdsvJo0TGb9CdSJ+BNCwTU4aZ7GpT7jKm3cdsl3WnnHGV+L9HCU9OQw==";

        static void Main(string[] args)
        {
            var storage = new AzureStorage(_storageAccountName, _storageAccountKey);

            storage.UploadAsync(containerName: "test", 
                blockName: "filenamessss.zip",
                payload: File.OpenRead(@"C:\affiliation\filename.zip"),
                cancellationToken: CancellationToken.None)
                .GetAwaiter().GetResult();
        }
    }
}
