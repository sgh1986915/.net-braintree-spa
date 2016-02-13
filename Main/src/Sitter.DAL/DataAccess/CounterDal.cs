using MongoDB.Bson;
using MongoDB.Driver;
using MySitterHub.DAL.General;

namespace MySitterHub.DAL.DataAccess
{
    public class CounterDal
    {
        public const string AppUserAutoIncrementName = "u";
        public const string JobAutoIncrementName = "j";
        public const int StartNum = 0;

        public int GetNextAppUserId()
        {
            return  GetNextAutoIncrementId(AppUserAutoIncrementName);
        }

        public int GetNextJobSlugId()
        {
            return GetNextAutoIncrementId(JobAutoIncrementName);
        }

        public int GetNextAutoIncrementId(string autoIncrementName)
        {
            InitializeCounterCollectionIfNotExists(false);
            //FutureDev: we may want to turn this off in the future for performance reasons
            const string script = @"
function getNextSequence(name) {
   var ret = db.counter.findAndModify(
          {
            query: { _id: name },
            update: { $inc: { seq: 1 } },
            new: true
          }
   );

return ret.seq;
}";

            //var doc = BsonDocument.Parse(script); //Future
            //EvalArgs args  = new EvalArgs() {};
#pragma warning disable 618
            string val = MongoCnn.GetDbConnection().Eval(script, autoIncrementName).ToString();
#pragma warning restore 618
            int nextId;
            int.TryParse(val, out nextId);

            return nextId;
        }

        public void InitializeCounterCollectionIfNotExists(bool delete)
        {
            MongoCollection<BsonDocument> counterCollection = MongoCnn.GetDbConnection().GetCollection(DbCollection.counter);
            if (DoesCounterCollectionHaveDocuments(counterCollection))
            {
                return;
            }

            const int startNum = StartNum;

            counterCollection.Insert(new BsonDocument { { "_id", AppUserAutoIncrementName }, { "seq", startNum } });
            //counterCollection.Insert(new BsonDocument {{"_id", BabySitterAutoIncrementName}, {"seq", startNum}});
            counterCollection.Insert(new BsonDocument { { "_id", JobAutoIncrementName }, { "seq", startNum } });
        }

        public bool DoesCounterCollectionHaveDocuments(MongoCollection<BsonDocument> counterCollection)
        {
            long count = counterCollection.Count();

            return count > 0;
        }
    }
}