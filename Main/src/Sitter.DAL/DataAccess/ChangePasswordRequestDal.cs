using MySitterHub.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySitterHub.DAL.General;
using MongoDB.Driver.Builders;
using MySitterHub.Model.Misc;

namespace MySitterHub.DAL.DataAccess
{
    public class ChangePasswordRequestDal
    {
        #region Insert

        public void Insert(ChangePasswordRequest changePasswordRequest)
        {
            MongoCnn.GetChangePasswordRequestCollection().Save(changePasswordRequest);
        }

        #endregion

        #region Get

        public List<ChangePasswordRequest> GetAll()
        {
            return MongoCnn.GetChangePasswordRequestCollection().FindAll().ToList();
        }

        public ChangePasswordRequest Get(int id)
        {
            return MongoCnn.GetChangePasswordRequestCollection().FindOne(Query<ChangePasswordRequest>.EQ(m => m.Id, id));
        }

        public ChangePasswordRequest GetByHash(Guid hash)
        {
            return MongoCnn.GetChangePasswordRequestCollection().FindOne(Query<ChangePasswordRequest>.EQ(m => m.Hash, hash));
        }

        #endregion

        #region Update

        public void Update(ChangePasswordRequest changePasswordRequest)
        {
            MongoCnn.GetChangePasswordRequestCollection().Save(changePasswordRequest);
        }

        #endregion
    }
}
