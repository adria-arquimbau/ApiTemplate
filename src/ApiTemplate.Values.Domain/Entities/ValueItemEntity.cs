using System;

namespace ApiTemplate.Values.Domain.Entities
{
    public class ValueItemEntity : IEquatable<ValueItemEntity>
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public int Value { get; set; }

        public ValueItemEntity(string key, int value)
        {
            Id = new Guid();
            Key = key;
            Value = value;
        }

        public bool Equals(ValueItemEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Key == other.Key && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ValueItemEntity)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Key, Value);
        }

        public static bool operator ==(ValueItemEntity left, ValueItemEntity right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueItemEntity left, ValueItemEntity right)
        {
            return !Equals(left, right);
        }
    }
}
