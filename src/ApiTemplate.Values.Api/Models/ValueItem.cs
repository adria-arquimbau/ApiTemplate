namespace ApiTemplate.Values.Api.Models
{
    public class ValueItem
    {
        public string Key { get; }
        public int Value { get; }

        // Needed for deserialization
        public ValueItem() { }

        public ValueItem(ApiTemplate.Values.Domain.Entities.ValueItem item)
        {
            Key = item.Key;
            Value = item.Value;
        }
    }
}
