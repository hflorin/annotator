namespace Treebank.Mappers
{
    using Domain;

    public static class EntityFactory
    {
        public static IEntity GetEntity(string entity)
        {
            var cleanEntity = entity.Trim().ToLowerInvariant();

            if (cleanEntity.Equals("attribute"))
            {
                return new Attribute();
            }

            if (cleanEntity.Equals("word"))
            {
                return new Word();
            }

            if (cleanEntity.Equals("sentence"))
            {
                return new Sentence();
            }

            if (cleanEntity.Equals("document"))
            {
                return new Document();
            }

            throw new UnknownEntityTypeException(string.Format("Unkown entity {0}", entity));
        }
    }
}