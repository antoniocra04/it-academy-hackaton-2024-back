using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System;
using System.Net.Mime;
using System.Security.AccessControl;

namespace InterestClubWebAPI.Services
{
    public static class MinIOManager
    {
        public static string ServerIP { get; } = "109.71.242.219:9000";
        public static string ImageBucket { get; } = "images";

        static IMinioClient? Client
        {
            get
            {
                var accessKey = "admin";
                var secretKey = "1234567890";
                try
                {
                    var client = new MinioClient().WithEndpoint(ServerIP).WithCredentials(accessKey, secretKey).Build();
                    return client;
                }
                catch
                {
                    return null;
                }
            }
        }

        public async static Task<string> UploadFile(IFormFile file, string contextPath)
        {
            if (Client == null)
            {
                return string.Empty;
            }

            var beArgs = new BucketExistsArgs().WithBucket(ImageBucket);
            bool found = await Client.BucketExistsAsync(beArgs).ConfigureAwait(false);

            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(ImageBucket);

                await Client.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }

            //using (var ms = new MemoryStream())
            //{
            //    await file.CopyToAsync(ms);
            //    ms.Seek(0, SeekOrigin.Begin);
            //    var putObjectArgs = new PutObjectArgs()
            //    .WithBucket(ImageBucket)
            //    .WithObject(Path.Combine(ImageBucket, contextPath, file.FileName))
            //    .WithStreamData(ms)
            //    .WithContentType(file.ContentType);
            //}

            // Upload a file to bucket.
            var putObjectArgs = new PutObjectArgs()
                .WithBucket(ImageBucket)
                .WithObject(Path.Combine(ImageBucket, contextPath, file.FileName))
                .WithStreamData(file.OpenReadStream())
                .WithContentType(file.ContentType);

            await Client.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
            Console.WriteLine("Successfully uploaded " + file.FileName);

            return Path.Combine(ServerIP, ImageBucket, contextPath, file.FileName);
        }

        public async static void RemoveFile(string fileURL)
        {
            if (Client is null)
            {
                throw new Exception("Client did connected.");
            }

            var args = new RemoveObjectArgs()
                .WithBucket(ImageBucket)
                .WithObject(fileURL);

            await Client.RemoveObjectAsync(args).ConfigureAwait(false);
        }
    }
}
