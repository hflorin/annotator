namespace Treebank.Mappers
{
    using System;
    using System.Threading.Tasks;
    using Domain;

    public class DocumentMapperClient
    {
        private readonly IDocumentMapper documentMapper;

        public DocumentMapperClient(IDocumentMapper mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException("mapper");
            }
            documentMapper = mapper;
        }

        public async Task<Document> Map(string filepath, string configFilepath)
        {
            return await documentMapper.Map(filepath, configFilepath);
        }
    }
}