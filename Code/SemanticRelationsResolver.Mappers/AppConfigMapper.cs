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
            //var appConfigContent = await Loader.LoadAsync(filepath);

            return await Task.FromResult(CreateAppConfig(filepath));
        }

        private IAppConfig CreateAppConfig(string filepath)
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
                                        if (appConfig.Elements.Any())
                                        {
                                            var element = appConfig.Elements.Last();
                                            element.Attributes.Add(entity as Attribute);
                                        }
                                    }
                                    else
                                    {
                                        appConfig.Elements.Add(entity as Element);
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return appConfig;
        }

        private IAppConfig CreateAppConfig(dynamic appConfigContent)
        {
            var appConfig = new AppConfig();

            foreach (var element in appConfigContent.dataStructure.element)
            {
                var newElement = EntityFactory.GetEntity(element.entity);

                foreach (var attr in element.attribute)
                {
                    var allowedValues = new List<string>();
                    try
                    {
                        foreach (var allowedValue in attr.allowedValueSet)
                        {
                            allowedValues.Add(allowedValue.value);
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    bool isEditable;
                    bool.TryParse(attr.isEditable, out isEditable);
                    bool isOptional;
                    bool.TryParse(attr.isEditable, out isOptional);

                    newElement.Attributes.Add(new Attribute
                    {
                        Name = attr.name,
                        DisplayName = attr.displayName,
                        IsEditable = isEditable,
                        IsOptional = isOptional,
                        AllowedValuesSet = allowedValues
                    });
                }
            }

            return appConfig;
        }
    }

    internal class ConfigurationPair
    {
        public string ElementName { get; set; }
        public List<Dictionary<string, string>> Attributes { get; set; }
    }
}