namespace Treebank.Mappers
{
    using Domain;

    public static class EntityFactory
    {
        public static IEntity GetEntity(string entity)
        {
            var cleanEntity = entity.Trim().ToLowerInvariant();

            if (cleanEntity.Equals(ConfigurationStaticData.AttributeEntityName))
            {
                return new Attribute();
            }

            if (cleanEntity.Equals(ConfigurationStaticData.WordEntityName))
            {
                return new Word();
            }

            if (cleanEntity.Equals(ConfigurationStaticData.SentenceEntityName))
            {
                return new Sentence();
            }

            if (cleanEntity.Equals(ConfigurationStaticData.DocumentEntityName))
            {
                return new Document();
            }

            throw new UnknownEntityTypeException(string.Format("Unkown entity {0}", entity));
        }
    }
}