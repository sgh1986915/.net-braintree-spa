using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver.Linq;
using MySitterHub.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MySitterHub.DAL.General;
using MySitterHub.Model.Common;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.DataAccess
{
    public class SitterDal
    {
        readonly AppUserDal _appUserDal = new AppUserDal();

        #region Insert

        public void Insert(Sitter sitter)
        {
            MongoCnn.GetSitterCollection().Insert(sitter);
        }

        #endregion

        #region Get

        public List<Sitter> GetAll()
        {
            return MongoCnn.GetSitterCollection().FindAll().ToList();
        }

        public Sitter GetById(int id)
        {
            if (id == 0)
                return null;
            Sitter s = MongoCnn.GetSitterCollection().FindOne(Query<Sitter>.EQ(m => m.Id, id));
            if (s == null)
                return null;
            PopulateUserProperty(s);
            return s;
        }

        /// <summary>
        /// Get the User object from the AppUser collection
        /// </summary>
        private void PopulateUserProperty(Sitter s)
        {
            s.User = _appUserDal.GetById(s.Id);
            if (s.User == null)
            {
                throw new ValidationException("Unable to set User prop");
            }
        }

        public List<Sitter> GetSittersByIds(List<ParentMySitter> sitterIds)
        {            
            var sitters = new List<Sitter>();
            if (sitterIds != null)
            {
                foreach (var sitter in sitterIds)
                {
                    var s = GetById(sitter.SitterId);
                    if (s != null)
                        sitters.Add(s);
                }
            }
            return sitters;
        }

        public void GetSittersForParent(int sitterId)
        {
            
        }

        #endregion

        #region Update

        public void Update(Sitter sitter)
        {
            MongoCnn.GetSitterCollection().Save(sitter);
        }

        #endregion

        #region Delete

        public void DeleteById(int id)
        {
            IMongoQuery query = Query<Sitter>.EQ(m2 => m2.User.Id, id);
            MongoCnn.GetSitterCollection().Remove(query);
        }

        #endregion

    }
}