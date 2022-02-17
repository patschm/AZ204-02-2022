using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

using Azure.Storage.Blobs;

namespace CloudStorage
{
    // Depricated StorageClient. Don't use
    //
    //class Program
    //{
    //    private static string storage = "psstoring";
    //    private static object key = "i9Jafh6m/c5JRTKwPRkHTlY/xJF1ryqe59R8/Q9mpuR0XPR4bCQsCK6XHV9Uffq249/kf6Z0PvoET1z9RYnALg==";
    //    private static string basePath = Assembly.GetExecutingAssembly().Location;

    //    static void Main(string[] args)
    //    {
    //        basePath = basePath.Substring(0, basePath.IndexOf("\\bin")+1);
    //        //WriteSimple();
    //        // ReadSimple();
    //        // WriteBlockBlob();
    //        //WritePageBlob();
    //        // ReadPageBlob();
    //        //MetaProp();
    //        Console.WriteLine("Done");
    //        Console.ReadLine();
    //    }

    //     private static BlobClient CreateAccount()
    //    {
    //        string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storage, key);
    //        BlobServiceClient account = new BlobServiceClient(connectionString);
    //        return account;
    //    }

    //    private static CloudBlobContainer GetContainer(string containerName)
    //    {
    //        CloudStorageAccount account = CreateAccount();
    //        CloudBlobClient client = account.CreateCloudBlobClient();
    //        CloudBlobContainer container = client.GetContainerReference(containerName);

    //        container.CreateIfNotExists();
    //        return container;
    //    }

    //    private static void WriteSimple()
    //    {
    //        CloudBlobContainer container = GetContainer("demo");
    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle0.jpg");
    //        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //        {
    //            blob.UploadFromStream(fs);
    //        }
    //    }
    //    private static void ReadSimple()
    //    {
    //        CloudBlobContainer container = GetContainer("demo");
    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle0.jpg");
    //        using (FileStream fs = File.OpenWrite(basePath + "chambord0.jpg"))
    //        {
    //            blob.DownloadToStream(fs);
    //        }
    //    }
    //    private static void WriteBlockBlob()
    //    {
    //        CloudBlobContainer container = GetContainer("demo");
    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle1.jpg");
            
    //        List<string> blockList = new List<string>();

    //        int blockID = 1;
    //        int blockSize = 1000000;
    //        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //        {
    //            using (BinaryReader rdr = new BinaryReader(fs))
    //            {
    //                byte[] buffer;
    //                do
    //                {
    //                    buffer = rdr.ReadBytes(blockSize);
    //                    using (MemoryStream mem = new MemoryStream(buffer))
    //                    {
    //                        // Block id's must have the same length
    //                        string sBlockId = Convert.ToBase64String(BitConverter.GetBytes(blockID));
    //                        blob.PutBlock(sBlockId, mem, null);
    //                        blockList.Add(sBlockId);
    //                        blockID++;
    //                    }
    //                }
    //                while (buffer.Length == blockSize);
    //            }
    //        }
    //         blob.PutBlockList(blockList);
    //    }
    //    private static void WritePageBlob()
    //    {
    //        CloudBlobContainer container = GetContainer("demo");
    //        CloudPageBlob blob = container.GetPageBlobReference("castle2.jpg");

    //        // must be a multitude of 512
    //        int byteCount = 1048576;
    //        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //        {
    //            long pagesNeeded = fs.Length / byteCount;
    //            // Reserve some space to write the pages
    //            blob.Create(byteCount * (pagesNeeded + 2));

    //            using (BinaryReader rdr = new BinaryReader(fs))
    //            {
    //                int offset = 0;
    //                byte[] buffer;
    //                do
    //                {
    //                    buffer = rdr.ReadBytes(byteCount);
    //                    Array.Resize(ref buffer, byteCount);
    //                    using (MemoryStream mem = new MemoryStream(buffer))
    //                    {
    //                        blob.WritePages(mem, offset);
    //                        offset += byteCount;
    //                    }
    //                }
    //                while (offset < fs.Length);
    //            }
    //        }
    //    }
    //    private static void ReadPageBlob()
    //    {
    //        CloudBlobContainer container = GetContainer("demo");
    //        CloudPageBlob blob = container.GetPageBlobReference("castle2.jpg");

    //        IEnumerable<PageRange> pageRanges = blob.GetPageRanges();
    //        using (FileStream fs = File.OpenWrite(basePath + "chambord2.jpg"))
    //        {
    //            using (Stream blobStream = blob.OpenRead())
    //            {
    //                foreach (PageRange range in pageRanges)
    //                {
    //                    int rangeSize = (int)(range.EndOffset + 1 - range.StartOffset);
    //                    blobStream.Seek(range.StartOffset, SeekOrigin.Begin);
    //                    byte[] buffer = new byte[rangeSize];
    //                    blobStream.Read(buffer, 0, rangeSize);
    //                    fs.Write(buffer, (int)range.StartOffset, rangeSize);
    //                }
    //            }
    //        }
    //    }

    //    private static void MetaProp()
    //    {
    //        CloudStorageAccount account = CreateAccount();
    //        CloudBlobClient client = account.CreateCloudBlobClient();
    //        CloudBlobContainer container = client.GetContainerReference("pub");
    //        container.CreateIfNotExists();

    //        BlobContainerPermissions permissions = new BlobContainerPermissions();
    //        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
    //        container.SetPermissions(permissions);

    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle0.jpg");

    //        if (!blob.Exists())
    //        {
    //            using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //            {
    //                blob.UploadFromStream(fs);
    //            }
    //        }
    //        // Blob must be in the store before setting properties           
    //        blob.Properties.CacheControl = "public, max-age=25036000";
    //        blob.Properties.ContentType = "image/jpg";
    //        //blob.Properties.CacheControl = DateTime.Fo
    //        blob.SetProperties();

    //        blob.Metadata.Add("crib", "MyCrib");
    //        blob.SetMetadata();

    //        Console.WriteLine("Blob Attributes");
    //        //blob.FetchAttributes();
    //        Console.WriteLine("Cache Control: {0}", blob.Properties.CacheControl);
    //        Console.WriteLine("ETag: {0}", blob.Properties.ETag);
    //        Console.WriteLine("crib: {0}", blob.Metadata["crib"]);
    //    }
    //    private static void Async()
    //    {
    //        CloudStorageAccount account = CreateAccount();
    //        CloudBlobClient client = account.CreateCloudBlobClient();
    //        CloudBlobContainer container = client.GetContainerReference("async");
    //        container.CreateIfNotExists();

    //        client.DefaultRequestOptions.SingleBlobUploadThresholdInBytes = 1048576;
    //        client.DefaultRequestOptions.ParallelOperationThreadCount = 10;

    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle0.jpg");
    //        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //        {
    //            blob.UploadFromStream(fs);
    //        }
    //    }
    //    private static void Misc()
    //    {
    //        CloudStorageAccount account = CreateAccount();
    //        CloudBlobClient client = account.CreateCloudBlobClient();
    //        CloudBlobContainer container = client.GetContainerReference("pub");
    //        container.CreateIfNotExists();
            
    //        BlobContainerPermissions permissions = new BlobContainerPermissions();
    //        permissions.PublicAccess = BlobContainerPublicAccessType.Container;
    //        container.SetPermissions(permissions);

    //        CloudBlockBlob blob = container.GetBlockBlobReference("castle0.jpg");
            
    //        BlobRequestOptions options = new BlobRequestOptions();
    //        options.ServerTimeout = TimeSpan.FromSeconds(60);
    //        options.RetryPolicy = new Microsoft.Azure.Storage.RetryPolicies.LinearRetry(TimeSpan.FromSeconds(60), 10);
            
    //        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
    //        {
    //            blob.UploadFromStream(fs, options: options);
    //        }

    //        blob.DeleteIfExists();

    //        container.DeleteIfExists();         
    //    }
    //}
}
