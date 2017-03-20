using System.IO;
using System.Threading.Tasks;
using Treebank.Mappers;
using Treebank.Mappers.LightWeight;
using Prism.Events;
using Treebank.Domain;

namespace Treebank.Persistence
{
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
                var lowercaseExtension = extension.Substring(1).ToLowerInvariant();

                DocumentMapperClient documentMapper = null;

                if (lowercaseExtension.Equals(ConfigurationStaticData.XmlFormat))
                    documentMapper =
                        new DocumentMapperClient(
                            new LightDocumentMapperWithReader
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });
                else if (lowercaseExtension.Equals(ConfigurationStaticData.ConllxFormat)
                         || lowercaseExtension.Equals(ConfigurationStaticData.ConllFormat))
                    documentMapper =
                        new DocumentMapperClient(
                            new LightConllxDocumentMapper
                            {
                                AppConfigMapper = appConfigMapper,
                                EventAggregator = eventAggregator
                            });

                if (documentMapper == null)
                    return new Sentence();

                return await documentMapper.LoadSentence(sentenceId, documentFilePath, configFilePath);
            }

            return new Sentence();
        }
    }
}