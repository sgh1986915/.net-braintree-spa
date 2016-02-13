using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.DataAccess
{
    public class UserPassDal
    {

        #region Insert

        public void InsertUserPass(AppUserPass appUserPass)
        {            
            MongoCnn.GetUserPassCollection().Insert(appUserPass);
        }

        #endregion

        #region Get

        public AppUserPass GetById(int id)
        {
            return MongoCnn.GetUserPassCollection().AsQueryable().FirstOrDefault(m => m.Id == id);
        }

        public AppUserPass GetByMobileOrEmail(string mobileOrEmail)
        {
            return MongoCnn.GetUserPassCollection().AsQueryable().FirstOrDefault(m => m.MobilePhone == mobileOrEmail || m.Email == mobileOrEmail);
        }

        #endregion

        #region Update

        public void ChangePass(int id, string newPass, string email,string mobilePhone)
        {
            var userPass = GetById(id);
            userPass.PasswordHash = StringHasher.GetHashString(newPass);
            userPass.Email = email;
            userPass.MobilePhone = mobilePhone;
            MongoCnn.GetUserPassCollection().Save(userPass);
        }

        public void Update(AppUserPass appUserPass)
        {
            MongoCnn.GetUserPassCollection().Save(appUserPass);
        }

        #endregion

        #region Delete

        public void DeleteById(int id)
        {
            IMongoQuery query = Query<AppUserPass>.EQ(m => m.Id, id);
            MongoCnn.GetUserPassCollection().Remove(query);
        }

        #endregion
    }
}