//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using MongoDB.Driver.Linq;
//using MySitterHub.Model;
//using MongoDB.Driver;
//using MongoDB.Driver.Builders;
//using MySitterHub.DAL.General;
//using MySitterHub.Model.Core;

//namespace MySitterHub.DAL.DataAccess
//{
//    public class JobInviteDal
//    {

//        #region Insert

//        public void Insert(int jobId, List<JobInvite> jobInvites)
//        {
//            MongoCnn.GetJobCollection().Save(job);
//        }

//        #endregion

//        #region Get

//        public List<Job> GetAll()
//        {
//            return MongoCnn.GetJobCollection().FindAll().ToList();
//        }

//        public Job GetById(int id)
//        {
//            return MongoCnn.GetJobCollection().FindOne(Query<Job>.EQ(m => m.Id, id));
//        }

//        public List<Job> GetByParentId(int parentId)
//        {
//            return MongoCnn.GetJobCollection().AsQueryable().Where(m => m.ParentId == parentId).ToList();
//        }

//        public Job GetOneOpenJob()
//        {
//            var ret =
//                MongoCnn.GetJobCollection()
//                    .FindOne(
//                        Query<Job>.Where(
//                            m => m.State == JobState.Invited && m.MessagingState == MessagingState.Ready));
//            return ret;
//        }


//        #endregion

//        #region Update

//        public void Update(Job job)
//        {
//            Debug.Assert(job.Id != 0);

//            MongoCnn.GetJobCollection().Save(job);
//        }

//        #endregion

//        #region Delete

//        public void DeleteById(int id)
//        {
//            IMongoQuery query = Query<Job>.EQ(m2 => m2.Id, id);
//            MongoCnn.GetJobCollection().Remove(query);
//        }

//        #endregion

//    }
//}