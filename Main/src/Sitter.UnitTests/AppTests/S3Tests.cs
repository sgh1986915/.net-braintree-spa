
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySitterHub.DAL.General;
using MySitterHub.Logic.Payment;

namespace MySitterHub.UnitTests.AppTests
{
    [TestClass]
    public class S3Tests
    {
        private S3Dal _s3Dal = new S3Dal();


        [TestMethod]
        public void GetSignedUploadUrl()
        {
#if OFF
            var key = new S3KeyBuilder("testcontent","txt");
            _s3Dal.PutString(key, "content for testing");

            var s3Obj = _s3Dal.GetS3ObjectAsString(key);

#endif


            FileInfo fi = new FileInfo(@"C:\zTempDocs\t\JFgray2.png");

            var s = _s3Dal.GeneratePreSignedURL(fi.Name);

            Console.WriteLine("url:" + s);

            UploadS3UsingWebService.UploadObject(s, fi.FullName);

        } 

    }
}