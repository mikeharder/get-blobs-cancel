using Azure.Storage.Blobs;

namespace GetBlobsCancel
{
    internal class Program
    {
        // Config
        private const int MinDelayMilliseconds = 1;
        private const int MaxDelayMilliseconds = 20;

        static void Main(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING") ??
                throw new InvalidOperationException("STORAGE_CONNECTION_STRING not set");

            var containerName = Environment.GetEnvironmentVariable("STORAGE_CONTAINER_NAME") ??
                throw new InvalidOperationException("STORAGE_CONTAINER_NAME not set");

            var containerClient = new BlobContainerClient(connectionString, containerName);

            var random = new Random();
            while (true)
            {
                var delay = random.Next(MinDelayMilliseconds, MaxDelayMilliseconds);
                using (var cts = new CancellationTokenSource(millisecondsDelay: delay))
                {
                    try
                    {
                        // Enumerate collection to ensure all BlobItems are downloaded
                        foreach (var _ in containerClient.GetBlobs(cancellationToken: cts.Token))
                        {
                        }
                        Console.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] completed");
                    }
                    catch (TaskCanceledException)
                    {
                        Console.WriteLine($"[{DateTime.Now:hh:mm:ss.fff}] cancelled");
                    }
                }
            }
        }
    }
}