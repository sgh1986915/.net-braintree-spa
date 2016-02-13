using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB.Driver.Linq;
using MySitterHub.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MySitterHub.DAL.General;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.DataAccess
{
    public class JobDal
    {

        #region Insert

        public void Insert(Job job)
        {
            job.Id = new CounterDal().GetNextJobSlugId();
            MongoCnn.GetJobCollection().Save(job);
        }

        #endregion

        #region Get

        public List<Job> GetAll()
        {
            return MongoCnn.GetJobCollection().FindAll().ToList();
        }

        public Job GetById(int id)
        {
            return MongoCnn.GetJobCollection().FindOne(Query<Job>.EQ(m => m.Id, id));
        }

        public List<Job> GetByParentId(int parentId)
        {
            return MongoCnn.GetJobCollection().AsQueryable().Where(m => m.ParentId == parentId).ToList();
        }

        public Job GetOneJobWithAnyPendingInvites()
        {
            var ret = MongoCnn.GetJobCollection().FindOne(Query<Job>.Where(m =>
                        m.AcceptedSitterId == null &&
                        m.JobInvites.Any(i => i.State == InvitationState.InvitePending)));
            return ret;
        }

        #endregion

        #region Update

        public void Update(Job job)
        {
            Debug.Assert(job.Id != 0);

            MongoCnn.GetJobCollection().Save(job);
        }

        #endregion

        #region Delete

        public void DeleteById(int id)
        {
            IMongoQuery query = Query<Job>.EQ(m2 => m2.Id, id);
            MongoCnn.GetJobCollection().Remove(query);
        }

        #endregion

    }
}