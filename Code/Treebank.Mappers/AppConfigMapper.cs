namespace Treebank.Mappers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Prism.Events;

    using Domain;
    using Configuration;

    public class AppConfigMapper : IAppConfigMapper
    {
        public IEventAggregator EventAggregator { get; set; }

        public async Task<IAppConfig> Map(string filepath)
        {
            return await Task.FromResult(CreateAppConfig(filepath));
        }

        private static IAppConfig CreateAppConfig(string filepath)
        {
            var appConfig = new AppConfig
            {
                Filepath = filepath,
                Name = Path.GetFileName(filepath)
            };

            using (var reader = new XmlTextReader(filepath))
            {
                var queue = new List<ConfigurationPair>();

                var isParsingDataStructure = false;
                var isParsingTreeStructure = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            var pair = new ConfigurationPair { ElementName = reader.Name };

                            var entityAttributes = new Dictionary<string, string>();

                            while (reader.MoveToNextAttribute())
                            {
                                entityAttributes.Add(reader.Name, reader.Value);
                            }

                            pair.Attributes.Add(entityAttributes);
                            queue.Add(pair);

                            if (pair.ElementName.Equals(ConfigurationStaticData.DataStructureTagName))
                            {
                                var dataStructure = new DataStructure();
                                appConfig.DataStructures.Add(dataStructure);

                                isParsingDataStructure = true;
                                isParsingTreeStructure = false;
                            }
                            else if (pair.ElementName.Equals(ConfigurationStaticData.TreeStructureTagName))
                            {
                                isParsingDataStructure = false;
                                isParsingTreeStructure = true;
                            }

                            break;
                        case XmlNodeType.EndElement:
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
            }

            return appConfig;
        }

        private static void ParseTreeStructure(ICollection<ConfigurationPair> queue, IAppConfig appConfig)
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
                        appConfig.Definitions.Add(
                            new Definition { Name = attributes[ConfigurationStaticData.NameStructureAttributeName] });
                    }
                    else if (elementName.Equals(ConfigurationStaticData.VertexTagName))
                    {
                        var definition = appConfig.Definitions.Last();

                        var vertexConfig = new VertexConfig
                                               {
                                                   Entity =
                                                       attributes[ConfigurationStaticData.EntityAttributeName], 
                                                   LabelAttributeName =
                                                       attributes[ConfigurationStaticData.LabelAttributeName]
                                               };

                        definition.Vertex = vertexConfig;
                    }
                    else if (elementName.Equals(ConfigurationStaticData.EdgeTagName))
                    {
                        var definition = appConfig.Definitions.Last();

                        var edgeConfig = new EdgeConfig
                                             {
                                                 Entity =
                                                     attributes[ConfigurationStaticData.EntityAttributeName], 
                                                 LabelAttributeName =
                                                     attributes[ConfigurationStaticData.LabelAttributeName], 
                                                 SourceVertexAttributeName =
                                                     attributes[ConfigurationStaticData.SourceVertexAttributeName], 
                                                 TargetVertexAttributeName =
                                                     attributes[ConfigurationStaticData.TargetVertexAttributeName]
                                             };

                        definition.Edge = edgeConfig;
                    }
                }
            }

            queue.Clear();
        }

        private static void ParseDataStructure(ICollection<ConfigurationPair> queue, IAppConfig appConfig)
        {
            if (queue.Count <= 0)
            {
                return;
            }

            var dataStructure = appConfig.DataStructures.LastOrDefault();

            if (dataStructure == null)
            {
                return;
            }

            foreach (var item in queue)
            {
                var elementName = item.ElementName;

                if (string.IsNullOrWhiteSpace(elementName))
                {
                    break;
                }

                foreach (var attributes in item.Attributes)
                {
                    if (elementName.Equals(ConfigurationStaticData.DataStructureTagName))
                    {
                        foreach (var attribute in attributes)
                        {
                            if (attribute.Key == "format")
                            {
                                dataStructure.Format = attribute.Value;
                                break;
                            }
                        }
                    }
                    else if (elementName.Equals(ConfigurationStaticData.AllowedValueSetTagName))
                    {
                        if (dataStructure.Elements.Any())
                        {
                            var element = dataStructure.Elements.Last();
                            if (element.Attributes.Any())
                            {
                                var lastAttribute = element.Attributes.Last();
                                lastAttribute.AllowedValuesSet = attributes.Values;
                            }
                        }
                    }
                    else if (elementName.Equals(ConfigurationStaticData.ExceptForValuesOfSetTagName))
                    {
                        if (dataStructure.Elements.Any())
                        {
                            var element = dataStructure.Elements.Last();
                            if (element.Attributes.Any())
                            {
                                var lastAttribute = element.Attributes.Last();

                                if (!attributes.ContainsKey(ConfigurationStaticData.ElementNameAttributeName))
                                {
                                    throw new System.MissingFieldException("Provide the name of one of the attribute elements in the except values for element.");
                                }

                                var result = new ExceptValuesOf
                                {
                                    AttributeName = attributes[ConfigurationStaticData.ElementNameAttributeName],
                                    Values = new List<string>()
                                };

                                foreach (var pair in attributes)
                                {
                                    if (!pair.Key.Equals(ConfigurationStaticData.ElementNameAttributeName))
                                    {
                                        result.Values.Add(pair.Value);
                                    }
                                }

                                lastAttribute.ExceptedValuesOfSet.Add(result);
                            }
                        }
                    }
                    else if (attributes.ContainsKey(ConfigurationStaticData.EntityAttributeName))
                    {
                        var entity = EntityFactory.GetEntity(attributes[ConfigurationStaticData.EntityAttributeName]);

                        if (entity is Element)
                        {
                            var asElement = entity as Element;
                            asElement.Entity = attributes[ConfigurationStaticData.EntityAttributeName];
                        }

                        if (entity is Attribute)
                        {
                            var attribute = entity as Attribute;
                            if (dataStructure.Elements.Any())
                            {
                                var element = dataStructure.Elements.Last();

                                attribute.DisplayName = attributes[ConfigurationStaticData.DisplayNameAttributeName];
                                attribute.Name = attributes[ConfigurationStaticData.NameStructureAttributeName];
                                attribute.IsOptional =
                                    bool.Parse(attributes[ConfigurationStaticData.IsOptionalAttributeName]);
                                attribute.IsEditable =
                                    bool.Parse(attributes[ConfigurationStaticData.IsEditableAttributeName]);

                                if (attributes.ContainsKey(ConfigurationStaticData.PositionAttributeName))
                                {
                                    attribute.Position =
                                        int.Parse(attributes[ConfigurationStaticData.PositionAttributeName]);
                                }

                                element.Attributes.Add(attribute);
                            }
                        }
                        else
                        {
                            var element = entity as Element;

                            if (element != null)
                            {
                                element.DisplayName = attributes[ConfigurationStaticData.DisplayNameAttributeName];
                                element.Name = attributes[ConfigurationStaticData.NameStructureAttributeName];
                                element.IsOptional =
                                    bool.Parse(attributes[ConfigurationStaticData.IsOptionalAttributeName]);

                                dataStructure.Elements.Add(element);
                            }
                        }
                    }
                }
            }

            queue.Clear();
        }
    }
}