using System.Text.Json.Serialization;

namespace NewLibrary.Shared.SeedWork
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DateTime? UpdatedDate { get; set; }
        protected BaseEntity()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
