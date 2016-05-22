namespace SemanticRelationsResolver.Mappers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using Domain;
    using Domain.Configuration;
    using Loaders;
    using Prism.Events;

    public class AppConfigMapper : IAppConfigMapper
    {
        public IResourceLoader Loader { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public async Task<IAppConfig> Map(string filepath)
        {
            return await Task.FromResult(CreateAppConfig(filepath));
        }

        private static IAppConfig CreateAppConfig(string filepath)
        {
            var appConfig = new AppConfig();

            var reader = new XmlTextReader(filepath);

            var queue = new List<ConfigurationPair>();

            var isParsingDataStructure = false;
            var isParsingTreeStructure = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element :
                        var pair = new ConfigurationPair {ElementName = reader.Name};

                        var entityAttributes = new Dictionary<string, string>();

                        while (reader.MoveToNextAttribute())
                        {
                            entityAttributes.Add(reader.Name, reader.Value);
                        }

                        pair.Attributes.Add(entityAttributes);
                        queue.Add(pair);

                        if (pair.ElementName.Equals(ConfigurationStaticData.DataStructureTagName))
                        {
                            isParsingDataStructure = true;
                            isParsingTreeStructure = false;
                        }
                        else if (pair.ElementName.Equals(ConfigurationStaticData.TreeStructureTagName))
                        {
                            isParsingDataStructure = false;
                            isParsingTreeStructure = true;
                        }

                        break;
                    case XmlNodeType.EndElement :
                        if (isParsingDataStructure)
                        {
                            ParseDataStructure(queue, appConfig);
                        }
                        else if (isParsingTreeStructure)
                        {
                            ParseTreeStructure(queue, appConfig);
                        }
                        break;
                }
            }

            ValidateDateAppConfig(appConfig);

            return appConfig;
        }

        private static void ValidateDateAppConfig(IAppConfig appConfig)
        {
            //todo: validate the tree structure part, the attributes used to build the tree must be defined
        }

        private static void ParseTreeStructure(List<ConfigurationPair> queue, IAppConfig appConfig)
        {
            foreach (var item in queue)
            {
                var elementName = item.ElementName;

                if (string.IsNullOrWhiteSpace(elementName))
                {
                    break;
                }

                foreach (var attributes in item.Attributes)
                {
                    if (elementName.Equals(ConfigurationStaticData.DefinitionTagName))
                    {
                        appConfig.Definitions.Add(new Definition
                        {
                            Name = attributes[ConfigurationStaticData.NameStructureAttributeName]
                        });
                    }
                    else if (elementName.Equals(ConfigurationStaticData.VertexTagName))
                    {
                        var definition = appConfig.Definitions.Last();

                        var vertexConfig = new VertexConfig
                        {
                            Entity = attributes[ConfigurationStaticData.EntityAttributeName],
                            FromAttributeName = attributes[ConfigurationStaticData.FromAttributeName],
                            LabelAttributeName = attributes[ConfigurationStaticData.LabelAttributeName],
                            ToAttributeName = attributes[ConfigurationStaticData.ToAttributeName]
                        };

                        definition.Vertex = vertexConfig;
                    }
                    else if (elementName.Equals(ConfigurationStaticData.EdgeTagName))
                    {
                        var definition = appConfig.Definitions.Last();

                        var edgeConfig = new EdgeConfig
                        {
                            Entity = attributes[ConfigurationStaticData.EntityAttributeName],
                            LabelAttributeName = attributes[ConfigurationStaticData.LabelAttributeName]
                        };

                        definition.Edge = edgeConfig;
                    }
                }
            }

            queue.Clear();
        }

        private static void ParseDataStructure(List<ConfigurationPair> queue, IAppConfig appConfig)
        {
            foreach (var item in queue)
            {
                var elementName = item.ElementName;

                if (string.IsNullOrWhiteSpace(elementName))
                {
                    break;
                }

                foreach (var attributes in item.Attributes)
                {
                    if (elementName.Equals(ConfigurationStaticData.AllowedValueSetTagName))
                    {
                        if (appConfig.Elements.Any())
                        {
                            var element = appConfig.Elements.Last();
                            if (element.Attributes.Any())
                            {
                                var lastAttribute = element.Attributes.Last();
                                lastAttribute.AllowedValuesSet = attributes.Values;
                            }
                        }
                    }
                    else if (attributes.ContainsKey(ConfigurationStaticData.EntityAttributeName))
                    {
                        var entity =
                            EntityFactory.GetEntity(attributes[ConfigurationStaticData.EntityAttributeName]);

                        if (entity is Attribute)
                        {
                            var attribute = entity as Attribute;
                            if (appConfig.Elements.Any())
                            {
                                var element = appConfig.Elements.Last();

                                attribute.DisplayName =
                                    attributes[ConfigurationStaticData.DisplayNameAttributeName];
                                attribute.Name =
                                    attributes[ConfigurationStaticData.NameStructureAttributeName];
                                attribute.IsOptional =
                                    bool.Parse(attributes[ConfigurationStaticData.IsOptionalAttributeName]);
                                attribute.IsEditable =
                                    bool.Parse(attributes[ConfigurationStaticData.IsEditableAttributeName]);

                                element.Attributes.Add(attribute);
                            }
                        }
                        else
                        {
                            var element = entity as Element;

                            if (element != null)
                            {
                                element.DisplayName =
                                    attributes[ConfigurationStaticData.DisplayNameAttributeName];
                                element.Name =
                                    attributes[ConfigurationStaticData.NameStructureAttributeName];
                                element.IsOptional =
                                    bool.Parse(attributes[ConfigurationStaticData.IsOptionalAttributeName]);

                                appConfig.Elements.Add(element);
                            }
                        }
                    }
                }
            }

            queue.Clear();
        }
    }

    internal class ConfigurationPair
    {
        public ConfigurationPair()
        {
            Attributes = new List<Dictionary<string, string>>();
        }

        public string ElementName { get; set; }

        public List<Dictionary<string, string>> Attributes { get; set; }
    }
}