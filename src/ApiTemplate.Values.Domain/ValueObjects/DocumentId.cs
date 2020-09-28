using System;
using LanguageExt;

namespace ApiTemplate.Values.Domain.ValueObjects
{
    public class DocumentId : Record<DocumentId>
    {
        private readonly Guid _value;

        public DocumentId(Guid id) : this()
        {
            _value = id;
        }

        protected DocumentId()
        {
        }

        public static implicit operator Guid(DocumentId documentId) => documentId._value;

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}