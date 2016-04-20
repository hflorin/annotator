namespace SemanticRelationsResolver.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Configuration;
    using Events;
    using Loaders;
    using Prism.Events;
    using Attribute = Domain.Attribute;

    public class AppConfigMapper : IAppConfigMapper
    {
        public IResourceLoader Loader { get; set; }

        public IEventAggregator EventAggregator { get; set; }

        public async Task<IAppConfig> Map(string filepath)
        {
            var appConfigContent = await Loader.LoadAsync(filepath);

            return await Task.Run(CreateAppConfig(appConfigContent));
        }

        private bool CheckPropertyExists(dynamic d, string properrtyName)
        {
            Type type = d.GetType();

            return type.GetProperties().Any(p => p.Name.Equals(properrtyName));
        }

        private IAppConfig CreateAppConfig(dynamic appConfigContent)
        {
            if (!CheckPropertyExists(appConfigContent, "configuration"))
            {
                EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                    .Publish("[Exception] Missing configuration tag.");
            }

            if (!CheckPropertyExists(appConfigContent, "dataStructure"))
            {
                EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                    .Publish("[Exception] Missing dataStructure tag.");
            }

            if (!CheckPropertyExists(appConfigContent, "elements"))
            {
                EventAggregator.GetEvent<DocumentLoadExceptionEvent>()
                    .Publish("[Exception] Missing elements tag.");
            }

            var appConfig = new AppConfig();

            foreach (var element in appConfigContent.configuration.dataStructure.elements)
            {
                var newElement = ElementFactory.GetElement(element.entity);

                foreach (var attribute in element.attributes)
                {
                    var allowedValues = new List<string>();
                    if (CheckPropertyExists(attribute, "allowedValueSet"))
                    {
                        foreach (var allowedValue in attribute.allowedValueSet)
                        {
                            allowedValues.Add(allowedValue.value);
                        }
                    }

                    bool isEditable;
                    bool.TryParse(attribute.isEditable, out isEditable);
                    bool isOptional;
                    bool.TryParse(attribute.isEditable, out isOptional);

                    newElement.Attributes.Add(new Attribute
                    {
                        Name = attribute.name,
                        DisplayName = attribute.displayName,
                        IsEditable = isEditable,
                        IsOptional = isOptional,
                        AllowedValuesSet = allowedValues
                    });
                }
            }

            return appConfig;
        }
    }
}