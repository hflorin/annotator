using System.IO;
using System.Threading.Tasks;
using Treebank.Mappers;
using Treebank.Mappers.LightWeight;
using Prism.Events;
using Treebank.Domain;

namespace Treebank.Persistence
{
    using System;
    using Mappers.Serialization;

    public class SentenceLoader : ISentenceLoader
    {
        private readonly IAppConfigMapper appConfigMapper;
        private readonly IEventAggregator eventAggregator;

        public SentenceLoader(IEventAggregator eventAggregator, IAppConfigMapper appConfigMapper)
        {
            this.eventAggregator = eventAggregator;
            this.appConfigMapper = appConfigMapper;
        }

        public async Task<Sentence> LoadSentenceWords(string sentenceId, string documentFilePath, string configFilePath)
        {
            var extension = Path.GetExtension(documentFilePath);
            if (extension != null)
            {
                var lowercaseExtension = extension.Substring(1);

                DocumentMapperClient documentMapper = null;

                if (lowercaseExtension.Equals(ConfigurationStaticData.XmlFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    documentMapper =
                        new DocumentMapperClient(
                            new SerializationDocumentMapper
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });
                }
                else if (lowercaseExtension.Equals(ConfigurationStaticData.ConllxFormat, StringComparison.InvariantCultureIgnoreCase)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConllFormat, StringComparison.InvariantCultureIgnoreCase)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConlluFormat, StringComparison.InvariantCultureIgnoreCase))
                {
                    documentMapper =
                        new DocumentMapperClient(
                            new LightConllxDocumentMapper
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });
                }

                if (documentMapper == null)
                {
                    return new Sentence();
                }

                return await documentMapper.LoadSentence(sentenceId, documentFilePath, configFilePath);
            }

            return new Sentence();
        }
    }
}