namespace Treebank.Mappers
{
    public static class ConfigurationStaticData
    {
        #region lowercase tag names

        public static readonly string ConfigurationTagName = "configuration";
        public static readonly string DataStructureTagName = "dataStructure";
        public static readonly string TreeStructureTagName = "treeStructure";
        public static readonly string DefinitionsTagName = "definitions";
        public static readonly string ElementsTagName = "elements";
        public static readonly string ElementTagName = "element";
        public static readonly string DefinitionTagName = "definition";
        public static readonly string VertexTagName = "vertex";
        public static readonly string EdgeTagName = "edge";
        public static readonly string AttributesTagName = "attributes";
        public static readonly string AttributeTagName = "attribute";
        public static readonly string AllowedValueSetTagName = "allowedValueSet";

        #endregion

        #region lowercase configuration attributes names

        public static readonly string EntityAttributeName = "entity";
        public static readonly string NameStructureAttributeName = "name";
        public static readonly string DisplayNameAttributeName = "displayName";
        public static readonly string IsOptionalAttributeName = "isOptional";
        public static readonly string IsEditableAttributeName = "isEditable";
        public static readonly string LabelAttributeName = "labelAttributeName";
        public static readonly string SourceVertexAttributeName = "sourceVertexAttributeName";
        public static readonly string TargetVertexAttributeName = "targetVertexAttributeName";
        public static readonly string PositionAttributeName = "position";

        #endregion

        #region lowercase entities values

        public static readonly string AttributeEntityName = "attribute";
        public static readonly string DocumentEntityName = "document";
        public static readonly string SentenceEntityName = "sentence";
        public static readonly string WordEntityName = "word";

        #endregion

        #region file formats

        public static readonly string XmlFormat = "xml";
        public static readonly string ConllxFormat = "conllx";

        #endregion
    }
}