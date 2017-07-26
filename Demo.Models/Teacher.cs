using EasyDo.Domain;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Demo.Models
{
    [Entity(DbName ="HC",TableName = "HC.Teacher")]
    public class Teacher : IEntity<string>
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string TName { get; set; }
    }
}
