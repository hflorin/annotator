namespace SemanticRelationsResolver.Annotator.Graph
{
    using Domain.Configuration;
    using Mappers;

    public static class MotherObjects
    {
        public static Definition DefaultDefinition
        {
            get
            {
                return new Definition
                {
                    Edge = new EdgeConfig
                    {
                        Entity = ConfigurationStaticData.WordEntityName,
                        LabelAttributeName = "deprel"
                    },
                    Vertex = new VertexConfig
                    {
                        Entity = ConfigurationStaticData.WordEntityName,
                        FromAttributeName = "head",
                        ToAttributeName = "id",
                        LabelAttributeName = "form"
                    }
                };
            }
        }
    }
}