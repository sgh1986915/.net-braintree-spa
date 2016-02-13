using System;
using System.IO;
using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.General
{
    public class S3Dal
    {
        private readonly IAmazonS3 _client;

        public S3Dal()
        {
            string id = AppConfigWebValues.Instance.AwsKeyId;
            string secret = AppConfigWebValues.Instance.AwsKeySecret;
            _client = new AmazonS3Client(id, secret, RegionEndpoint.USEast1);
        }

        public string GeneratePreSignedURL(string key)
        {
            string urlString = "";
            var request1 = new GetPreSignedUrlRequest
            {
                BucketName = AppConfigWebValues.Instance.S3BucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.Now.AddMinutes(5)
            };

            try
            {
                urlString = _client.GetPreSignedURL(request1);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                     ||
                     amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Check the provided AWS Credentials.");
                    Console.WriteLine("To sign up for service, go to http://aws.amazon.com/s3");
                }
                else
                {
                    Console.WriteLine(
                        "Error occurred. Message:'{0}' when listing objects",
                        amazonS3Exception.Message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return urlString;
        }

        public void PutString(S3KeyBuilder s3Url, string content)
        {
            var putRequest1 = new PutObjectRequest
            {
                BucketName = AppConfigWebValues.Instance.S3BucketName,
                Key = s3Url.FullKey,
                ContentBody = content
            };

            PutObjectResponse response1 = _client.PutObject(putRequest1);
            if (response1.HttpStatusCode != HttpStatusCode.OK)
                throw new Exception("Put not successful. Status code " + response1.HttpStatusCode);
        }

        public S3ObjectAsString GetS3ObjectAsString(S3KeyBuilder s3Key)
        {
            var request = new GetObjectRequest
            {
                BucketName = AppConfigWebValues.Instance.S3BucketName,
                Key = s3Key.FullKey
            };

            var obj = new S3ObjectAsString();
            try
            {
                using (GetObjectResponse response = _client.GetObject(request))
                using (Stream responseStream = response.ResponseStream)
                using (var reader = new StreamReader(responseStream))
                {
                    obj.Key = s3Key.FullKey;
                    obj.ContentBody = reader.ReadToEnd();
                }
            }
            catch (AmazonS3Exception s3x)
            {
                if (s3x.ErrorCode == S3Constants.NoSuchKey)
                    return null;

                throw;
            }

            return obj;
        }

        public S3ObjectAsImage GetS3ObjectAsImage(S3KeyBuilder s3Key)
        {
            var request = new GetObjectRequest
            {
                BucketName = AppConfigWebValues.Instance.S3BucketName,
                Key = s3Key.FullKey
            };

            var obj = new S3ObjectAsImage();
            try
            {
                var ms = new MemoryStream();
                using (GetObjectResponse response = _client.GetObject(request))
                using (Stream responseStream = response.ResponseStream)
                {
                    obj.ContentType = "image/png"; //Futuredev: adjust to correct content type.

                    var buffer = new byte[8000];
                    int bytesRead = -1;
                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                    obj.Bytes = ms.ToArray();
                }
            }
            catch (AmazonS3Exception s3x)
            {
                if (s3x.ErrorCode == S3Constants.NoSuchKey)
                    return null;

                throw;
            }

            return obj;
        }

        public void DeleteS3Object(S3KeyBuilder s3Key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = AppConfigWebValues.Instance.S3BucketName,
                Key = s3Key.FullKey
            };
            DeleteObjectResponse response = _client.DeleteObject(deleteObjectRequest);
            if (response.HttpStatusCode != HttpStatusCode.NoContent)
                throw new Exception("Delete not successful. Status code " + response.HttpStatusCode);
        }
    }

    public class S3KeyBuilder
    {
        public S3KeyBuilder(S3ObjType type, string userId, string ext)
        {
            S3ObjType = type;
            UserId = userId;
            Ext = ext;
        }
        public S3ObjType S3ObjType { get; set; }
        public string UserId { get; set; }
        public string Ext { get; set; }

        public string FullKey
        {
            get
            {
                string url = string.Format("{0}_{1}.{2}",S3ObjType, UserId, Ext);

                return url;
            }
        }
    }

    public enum S3ObjType
    {
        pic //profile pic
    }

    public static class S3Constants
    {
        public const string NoSuchKey = "NoSuchKey";
        public const string png = "png";
        public const string S3publicBucketUrl = "https://s3.amazonaws.com/mshpics-dev";
    }

    public class S3ObjectAsString
    {
        public string ContentBody { get; set; }
        public string Key { get; set; }
    }

    public class S3ObjectAsImage
    {
        public byte[] Bytes { get; set; }
        public string Key { get; set; }
        public string ContentType { get; set; }
    }

}