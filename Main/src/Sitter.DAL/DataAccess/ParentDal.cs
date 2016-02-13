using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.DataAccess
{
    public class ParentDal
    {
        private readonly CounterDal _counterDal;

        public ParentDal()
        {
            _counterDal = new CounterDal();
        }

        #region Insert

        public void Insert(Parent parent)
        {
            MongoCnn.GetParentCollection().Insert(parent);
        }

        #endregion

        #region Get

        public List<Parent> GetAll()
        {
            return MongoCnn.GetParentCollection().FindAll().ToList();
        }

        public Parent GetById(int id)
        {
            return MongoCnn.GetParentCollection().FindOne(Query<Parent>.EQ(m => m.Id, id));
        }

        public Parent GetByEmail(string email)
        {
            return MongoCnn.GetParentCollection().FindOne(Query<Parent>.EQ(m => m.User.Email, email));
        }

        public List<int> GetAllParentIdsWhoHaveSitter(int sitterId)
        {
            var ret = new List<int>();
            foreach (var p in MongoCnn.GetParentCollection().FindAll())
            {
                var hasSitter = p.Sitters !=null && p.Sitters.Any(m => m.SitterId == sitterId);
                if (hasSitter)
                    ret.Add(p.Id);
            }
            return ret;
        }

        public Parent GetOneParentInviteSitterToSignup()
        {
            Parent p = MongoCnn.GetParentCollection().AsQueryable().FirstOrDefault(m => m.InviteToSignup != null && m.InviteToSignup.Any(si => si.InviteStatus == InvitationState.InvitePending));
            return p;
        } 

        #endregion

        #region Update

        public void Update(Parent parent)
        {
            MongoCnn.GetParentCollection().Save(parent);
        }

        #endregion

        #region Delete

        public void DeleteById(int id)
        {
            IMongoQuery query = Query<Parent>.EQ(m2 => m2.Id, id);
            MongoCnn.GetParentCollection().Remove(query);
        }

        #endregion

    }
}