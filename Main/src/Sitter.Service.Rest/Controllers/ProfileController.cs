using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using MySitterHub.Logic.Repository;
using MySitterHub.Service.Rest.Util;

namespace MySitterHub.Service.Rest.Controllers
{
    public class ProfileController : ApiController
    {
        private readonly AppUserRepository _appUserRepo = new AppUserRepository();
        private readonly ProfilePictureRepository _profilePictureRepository = new ProfilePictureRepository();

        public IHttpActionResult GetUserProfile()
        {
            int appUserId = HeaderHelper.GetAppUserId(this);
            return Ok(_appUserRepo.GetById(appUserId));
        }

#if OFF
        public HttpResponseMessage GetUserPicture()
        {
            int appUserId = HeaderHelper.GetAppUserId(this);
            var imgObj = _appUserRepo.GetUserPicture(appUserId);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);  // http://stackoverflow.com/questions/12467546/is-there-a-recommended-way-to-return-an-image-using-asp-net-web-api
            result.Content = new ByteArrayContent(imgObj.Bytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(imgObj.ContentType);
            return result;
        }
#endif

        public IHttpActionResult Post([FromBody] ProfileUpdateVM profile)
        {
#if OFF
    // http://stackoverflow.com/questions/10320232/how-to-accept-a-file-post-asp-net-mvc-4-webapi
            if (Request.Content.IsMimeMultipartContent())
            {
                var provider = new MultipartMemoryStreamProvider();
                Request.Content.ReadAsMultipartAsync(provider);
                foreach (HttpContent file in provider.Contents)
                {
                    string filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    byte[] buffer = file.ReadAsByteArrayAsync().Result;

                    string filePath = HttpContext.Current.Server.MapPath("~/" + filename);

                    using (var sw = new StreamWriter(filePath))
                    {
                        sw.Write(buffer);
                    }
                }
            }
#endif

            bool ret = _appUserRepo.UpdateProfile(profile);
            return Ok(ret);
        }

        public IHttpActionResult GetSecureUploadUrl()
        {
            int appUserId = HeaderHelper.GetAppUserId(this);
            string url = _profilePictureRepository.GetSecureAwsImageUploadUrl(appUserId);
            return Ok(url);
        }
    }
}