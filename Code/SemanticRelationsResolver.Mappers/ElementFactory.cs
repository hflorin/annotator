namespace SemanticRelationsResolver.Mappers
{
    using System;
    using Domain;

    public static class ElementFactory
    {
        public static Element GetElement(string entity)
        {
            var cleanEntity = entity.Trim().ToLowerInvariant();
            if (cleanEntity.Equals("document"))
                return new Document();

            if (cleanEntity.Equals("sentence"))
                return new Sentence();

            if (cleanEntity.Equals("word"))
                return new Word();

            throw new UnknownEntityTypeException();
        }
    }

    public class UnknownEntityTypeException : Exception
    {
    }
}