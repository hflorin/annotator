namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Linq;
    using Mappers;
    using Mappers.Configuration;
    using Prism.Events;
    using View.Services;
    using Wrapper;
    using Wrapper.Base;
    using Graph;
    using Persistence;

    public class DoubleSentenceEditorViewModel : SentenceEditorViewModel
    {
        private SentenceWrapper rightSentence;
        private StringWrapper rightStringWrapper;
        private readonly IAppConfigMapper appConfigMapper;
        private readonly string leftSentenceConfigFilePath, rightSentenceConfigFilePath, leftSentenceDocumentFilePath, rightSentenceDocumentFilePath;
        private readonly ISentenceLoader sentenceLoader;

        public DoubleSentenceEditorViewModel(
            IEventAggregator eventAggregator,
            IAppConfigMapper appConfigMapper,
            SentenceWrapper leftSentence,
            SentenceWrapper rightSentence,
            string leftSentenceDocumentFilePath,
            string rightSentenceDocumentFilePath,
            string leftSentenceConfigFilePath,
            string rightSentenceConfigFilePath,
            IShowInfoMessage showMessage, 
            ISentenceLoader sentenceLoader):base(eventAggregator, new AppConfig(), new DataStructure(), leftSentence,showMessage)
        {
            if (rightSentence == null)
            {
                throw new ArgumentNullException("rightSentence");
            }

            if (string.IsNullOrEmpty(leftSentenceConfigFilePath))
            {
                throw new ArgumentNullException("leftSentenceConfigFilePath");
            }

            if (string.IsNullOrEmpty(rightSentenceConfigFilePath))
            {
                throw new ArgumentNullException("rightSentenceConfigFilePath");
            }

            if (string.IsNullOrEmpty(leftSentenceDocumentFilePath))
            {
                throw new ArgumentNullException("leftSentenceDocumentFilePath");
            }

            if (string.IsNullOrEmpty(rightSentenceDocumentFilePath))
            {
                throw new ArgumentNullException("rightSentenceDocumentFilePath");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            if (showMessage == null)
            {
                throw new ArgumentNullException("showMessage");
            }

            if (appConfigMapper == null)
            {
                throw new ArgumentNullException("appConfigMapper");
            }

            if (sentenceLoader == null)
            {
                throw new ArgumentNullException("sentenceLoader");
            }

            RightSentence = rightSentence;
            this.appConfigMapper = appConfigMapper;
            this.leftSentenceConfigFilePath = leftSentenceConfigFilePath;
            this.rightSentenceConfigFilePath = rightSentenceConfigFilePath;
            this.leftSentenceDocumentFilePath = leftSentenceDocumentFilePath;
            this.rightSentenceDocumentFilePath = rightSentenceDocumentFilePath;
            this.sentenceLoader = sentenceLoader;
        }

        public StringWrapper RightSentenceInfo
        {
            get { return rightStringWrapper; }
            set
            {
                rightStringWrapper = value;
                OnPropertyChanged();
            }
        }

        public SentenceWrapper RightSentence
        {
            get { return rightSentence; }
            set
            {
                rightSentence = value;
                InvalidateCommands();
                OnPropertyChanged();
            }
        }

        public override async void CreateSentenceGraph()
        {
            if (Sentence == null || RightSentence == null)
            {
                return;
            }

            if (GraphBuilder == null)
            {
                GraphBuilder = new GraphBuilder(null);
            }

            if (Sentence.Words == null || !Sentence.Words.Any())
            {
                var sentenceLoaded =
                await sentenceLoader.LoadSentenceWords(Sentence.Id.Value, leftSentenceDocumentFilePath, leftSentenceConfigFilePath);

                if (sentenceLoaded == null)
                    return;

                Sentence = new SentenceWrapper(sentenceLoaded);
            }

            if (RightSentence.Words == null || !RightSentence.Words.Any())
            {
                var sentenceLoaded =
                await sentenceLoader.LoadSentenceWords(RightSentence.Id.Value, rightSentenceDocumentFilePath, rightSentenceConfigFilePath);

                if (sentenceLoaded == null)
                    return;

                RightSentence = new SentenceWrapper(sentenceLoaded);
            }

            var logicCore = await GraphBuilder.SetupGraphLogic(Sentence, RightSentence, appConfigMapper, leftSentenceConfigFilePath, rightSentenceConfigFilePath);
            SentenceGraphLogicCore = logicCore;
        }
    }
}