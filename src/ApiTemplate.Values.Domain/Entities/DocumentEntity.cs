using ApiTemplate.Values.Domain.ValueObjects;

namespace ApiTemplate.Values.Domain.Entities
{
    public class DocumentEntity
    {
        public DocumentId Identifier { get; }
        public string Name { get; }
        public string Extension { get; }

        public DocumentEntity(DocumentId identifier, string name, string extension)
        {
            Identifier = identifier;
            Name = name;
            Extension = extension;
        }
    }
}
