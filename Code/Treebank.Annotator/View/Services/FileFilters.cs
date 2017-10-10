namespace Treebank.Annotator.View.Services
{
    public static class FileFilters
    {
        public const string XmlFilesOnlyFilter = "XML files (*.xml)|*.xml";

        public const string XmlAndConllxAndConlluFilesOnlyFilter = "XML files (*.xml)|*.xml|CONLLX files (*.conllx)|*.conllx|CONLL files (*.conll)|*.conll|CONLLU files (*.conllu)|*.conllu";

        public const string AllFilesFilter = "All files (*.*)|*.*";
    }
}