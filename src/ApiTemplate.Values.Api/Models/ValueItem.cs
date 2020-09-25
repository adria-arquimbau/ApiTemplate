namespace ApiTemplate.Values.Api.Models
{
    public class ValueItem
    {
        public string Key { get; }
        public int Value { get; }

        // Needed for deserialization
        public ValueItem() { }

        public ValueItem(ApiTemplate.Values.Domain.Entities.ValueItemEntity itemEntity)
        {
            Key = itemEntity.Key;
            Value = itemEntity.Value;
        }
    }
}
