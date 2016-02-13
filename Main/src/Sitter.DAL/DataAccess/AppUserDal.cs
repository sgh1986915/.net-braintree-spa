using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.DataAccess
{
    public class AppUserDal
    {

        #region Insert

        public void InsertUser(AppUser appUser)
        {
            appUser.Id = new CounterDal().GetNextAppUserId();
            MongoCnn.GetAppUserCollection().Insert(appUser);
        }

        #endregion

        #region Get

        public IEnumerable<AppUser> GetAll()
        {
            return MongoCnn.GetAppUserCollection().FindAll();
        }

        public AppUser GetById(int id)
        {
            return MongoCnn.GetAppUserCollection().AsQueryable().FirstOrDefault(m => m.Id == id);
        }

        public AppUser GetUserByEmail(string email)
        {
            return MongoCnn.GetAppUserCollection().AsQueryable().FirstOrDefault(m => m.Email == email);
        }

        public AppUser GetByMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return null;

            AppUser usr = MongoCnn.GetAppUserCollection().AsQueryable().FirstOrDefault(m => m.MobilePhone == mobile);
            return usr;
        }

        public AppUser GetByMobileAndRole(string mobile, UserRole userRole)
        {
            if (string.IsNullOrEmpty(mobile))
                return null;

            AppUser usr = MongoCnn.GetAppUserCollection().AsQueryable().FirstOrDefault(m => m.MobilePhone == mobile && m.UserRole == userRole);
            return usr;
        }

        #endregion

        #region Update

        public void Update(AppUser appUser)
        {
            MongoCnn.GetAppUserCollection().Save(appUser);
        }

        #endregion
    }
}