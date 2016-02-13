//using System;
//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;

//namespace MySitterHub.Service.Rest.Controllers
//{
//    public class FileUploadFormatter : MediaTypeFormatter
//    {
//        public FileUploadFormatter()
//        {
//            SupportedMediaTypes
//                .Add(new MediaTypeHeaderValue("multipart/form-data"));
//        }

//        public override bool CanReadType(Type type)
//        {
//            return true;
//        }

//        public override bool CanWriteType(Type type)
//        {
//            return false;
//        }

//        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
//        {

//            //const string uploadDir = @"..\..\..\..\RawFileUploads";
//            //if (!Directory.Exists(uploadDir))
//            //    Directory.CreateDirectory(uploadDir);

//            //string diskName = Guid.NewGuid().ToString();
//            //string displayName = "file1.raw"; //todo: get from headers.
//            //using (var fileStream = File.Create(Path.Combine(uploadDir, diskName)))
//            //{
//            //    readStream.CopyTo(fileStream);
//            //}

//            //RawFileRecord r = new RawFileRecord() {DiskName = diskName, DisplayName = displayName, Folder = ""};
//            //_repo.InsertRawFile(r);
//            //TODO: return the new Id of the uploaded file.
            
//            return new Task<object>(Continue);
//        }

//        private object Continue()
//        {
//            return "c";
//        }


//    }
//}