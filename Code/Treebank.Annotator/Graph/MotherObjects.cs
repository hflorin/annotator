namespace Treebank.Annotator.Graph
{
    using Mappers;
    using Mappers.Configuration;

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
                        LabelAttributeName = "deprel",
                        SourceVertexAttributeName = "head",
                        TargetVertexAttributeName = "id",
                    },
                    Vertex = new VertexConfig
                    {
                        Entity = ConfigurationStaticData.WordEntityName,
                        LabelAttributeName = "form"
                    }
                };
            }
        }
    }
}