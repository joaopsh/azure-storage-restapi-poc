using System.Text;
using System.Threading;
using AzureUploader.Core;

namespace AzureUploader
{
    class Program
    {
        private static string _storageAccountName = "xxx";
        private static string _storageAccountKey = "xxx";

        static void Main(string[] args)
        {
            var storage = new AzureStorage(_storageAccountName, _storageAccountKey);

            storage.UploadAsync(containerName: "test", 
                blockName: "somefile.txt",
                payload: Encoding.UTF8.GetBytes("Você já parou para imaginar que nesse exato momento existe alguém com a máquina travada por causa do Java?"),
                cancellationToken: CancellationToken.None)
                .GetAwaiter().GetResult();
        }
    }
}
