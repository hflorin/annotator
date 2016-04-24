namespace SemanticRelationsResolver.Mappers
{
    using System;
    using Domain;
    using Attribute = Domain.Attribute;

    public static class EntityFactory
    {
        public static IEntity GetEntity(string entity)
        {
            var cleanEntity = entity.Trim().ToLowerInvariant();
            if (cleanEntity.Equals("document"))
                return new Document();

            if (cleanEntity.Equals("sentence"))
                return new Sentence();

            if (cleanEntity.Equals("word"))
                return new Word();

            if (cleanEntity.Equals("attribute"))
                return new Attribute();

            throw new UnknownEntityTypeException();
        }
    }

    public class UnknownEntityTypeException : Exception
    {
    }
}