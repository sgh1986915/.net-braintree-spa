using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MySitterHub.Model.Core;

namespace MySitterHub.DAL.General
{
    public class MongoBsonClassMapper
    {
        public static void Map()
        {
            BsonClassMap.RegisterClassMap<Job>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(m => m.State).SetRepresentation(BsonType.String);
                cm.GetMemberMap(m => m.CloseReason).SetRepresentation(BsonType.String);
                cm.GetMemberMap(m => m.PaymentState).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<AppUser>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(m => m.UserRole).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<TxtMsg>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap.SetRepresentation(BsonType.ObjectId);
                cm.GetMemberMap(m => m.State).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<TxtMsgInbound>(cm =>
            {
                cm.AutoMap();            
                cm.GetMemberMap(m => m.InboundMessageType).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<TxtMsgOutbound>(cm =>
            {
                cm.AutoMap();
                //cm.IdMemberMap.SetRepresentation(BsonType.ObjectId);
                cm.GetMemberMap(m => m.OutboundMessageType).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<TxtMsgAwaitingResponse>(cm =>
            {
                cm.AutoMap();
                cm.IdMemberMap.SetRepresentation(BsonType.ObjectId);
                cm.GetMemberMap(m => m.AwaitingResponseType).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<JobInvite>(cm =>
            {
                cm.AutoMap();
                cm.GetMemberMap(m => m.State).SetRepresentation(BsonType.String);
            });

            BsonClassMap.RegisterClassMap<Sitter>(cm =>
            {
                cm.AutoMap();
                cm.UnmapProperty(m => m.User);
            });

            BsonClassMap.RegisterClassMap<Parent>(cm =>
            {
                cm.AutoMap();
                cm.UnmapProperty(m => m.User);
            });
        }
    }
}