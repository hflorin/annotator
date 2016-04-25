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

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element :
                        var pair = new ConfigurationPair
                        {
                            ElementName = reader.Name
                        };

                        var entityAttributes = new Dictionary<string, string>();

                        while (reader.MoveToNextAttribute())
                        {
                            entityAttributes.Add(reader.Name, reader.Value);
                        }

                        pair.Attributes.Add(entityAttributes);
                        queue.Add(pair);
                        break;
                    case XmlNodeType.EndElement :
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
                                        EntityFactory.GetEntity(
                                            attributes[ConfigurationStaticData.EntityAttributeName]);

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
                        break;
                }
            }

            return appConfig;
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