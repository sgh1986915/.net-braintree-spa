using MySitterHub.DAL.DataAccess;
using MySitterHub.DAL.General;

namespace MySitterHub.Logic.Repository
{
    public class ProfilePictureRepository
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly S3Dal _s3Dal = new S3Dal();
        private readonly UserPassDal _userPassDal = new UserPassDal();


        public string GetSecureAwsImageUploadUrl(int appUserId)
        {
            string pictureKey = string.Format("profilePic_{0}", appUserId);
            return _s3Dal.GeneratePreSignedURL(pictureKey);
        }
    }
}