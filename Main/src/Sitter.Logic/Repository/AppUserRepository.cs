using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySitterHub.DAL.DataAccess;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;

namespace MySitterHub.Logic.Repository
{
    public class AppUserRepository
    {
        private readonly AppUserDal _appUserDal = new AppUserDal();
        private readonly UserPassDal _userPassDal = new UserPassDal();

        public List<AppUserSM> GetAllAppUsers()
        {
            var allUsers = new List<AppUserSM>();
            foreach (AppUser item in _appUserDal.GetAll())
            {
                var sm = new AppUserSM();
                sm.Id = item.Id;
                sm.FullName = item.FullName();
                sm.MobilePhone = item.MobilePhone;
                allUsers.Add(sm);
            }
            return allUsers.OrderBy(m=> m.FullName).ToList();
        }

        public AppUser GetById(int id)
        {
            var profile = _appUserDal.GetById(id);
            if (profile.HasProfilePic)
            {
                profile.PhotoUrl = string.Format("{0}/{1}_{2}.png", S3Constants.S3publicBucketUrl, S3ObjType.pic, profile.Id);
            }

            return profile;
        }

        public bool UpdateProfile(ProfileUpdateVM profile)
        {
            if (profile.AppUser.Email != null)
            {
                profile.AppUser.Email = profile.AppUser.Email.ToLower();
            }
            //TODO: we need to update AppUserPass if user changes mobile phone or email.
            _appUserDal.Update(profile.AppUser); // Note, we are not changing parent properties, just parent.User properties
            if (profile.ChangePass != null)
            {
                _userPassDal.ChangePass(profile.AppUser.Id, profile.ChangePass.NewPass, profile.AppUser.Email, profile.AppUser.MobilePhone);
            }
            return true;
        }

        public S3ObjectAsImage GetUserPicture(int appUserId)
        {
            S3KeyBuilder builder = new S3KeyBuilder(S3ObjType.pic, appUserId.ToString(), S3Constants.png);
            return new S3Dal().GetS3ObjectAsImage(builder);
        }
    }

    public class ProfileUpdateVM
    {
        public AppUser AppUser { get; set; }
        public ChangePassVM ChangePass { get; set; }
    }

    public class ChangePassVM
    {
        public string OldPass { get; set; }
        public string NewPass { get; set; }
    }

    public class AppUserSM
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
    }
}