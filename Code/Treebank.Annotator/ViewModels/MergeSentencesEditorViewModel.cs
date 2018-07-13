namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Extensions;
    using Events;
    using Mappers;
    using Mappers.Configuration;
    using Prism.Events;
    using View.Services;
    using Wrapper;
    using Wrapper.Base;
    using Graph;
    using Persistence;

    public class MergeSentencesEditorViewModel : SentenceEditorViewModel
    {
        private SentenceWrapper rightSentence;
        private StringWrapper rightStringWrapper;
        private readonly string leftSentenceConfigFilePath, rightSentenceConfigFilePath, leftSentenceDocumentFilePath, rightSentenceDocumentFilePath, leftSentenceDocumentId;
        private readonly ISentenceLoader sentenceLoader;

        public MergeSentencesEditorViewModel(
            IEventAggregator eventAggregator,
            IAppConfigMapper appConfigMapper,
            SentenceWrapper leftSentence,
            SentenceWrapper rightSentence,
            string leftSentenceDocumentFilePath,
            string leftSentenceDocumentId,
            string rightSentenceDocumentFilePath,
            string leftSentenceConfigFilePath,
            string rightSentenceConfigFilePath,
            IShowInfoMessage showMessage, 
            ISentenceLoader sentenceLoader, 
            IAppConfig appConfig = null,
            DataStructure dataStructure = null):base(eventAggregator, appConfig ?? new AppConfig(), dataStructure ?? new DataStructure(), leftSentence,showMessage)
        {
            if (rightSentence == null)
            {
                throw new ArgumentNullException("rightSentence");
            }

            if (string.IsNullOrEmpty(leftSentenceConfigFilePath))
            {
                throw new ArgumentNullException("leftSentenceConfigFilePath");
            }


            if (string.IsNullOrEmpty(leftSentenceDocumentId))
            {
                throw new ArgumentNullException("leftSentenceDocumentId");
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
            
            this.leftSentenceConfigFilePath = leftSentenceConfigFilePath;
            this.rightSentenceConfigFilePath = rightSentenceConfigFilePath;
            this.leftSentenceDocumentFilePath = leftSentenceDocumentFilePath;
            this.leftSentenceDocumentId = leftSentenceDocumentId;
            this.rightSentenceDocumentFilePath = rightSentenceDocumentFilePath;
            this.sentenceLoader = sentenceLoader;

            LoadSentencesWords().GetAwaiter().GetResult();
            MergeSentences();
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

        public override void CreateSentenceGraph()
        {
            if (GraphBuilder == null)
            {
                GraphBuilder = new GraphBuilder(null);
            }
            

            var logicCore = GraphBuilder.SetupGraphLogic(Sentence);
            
            SentenceGraphLogicCore = logicCore;
        }

        private void MergeSentences()
        {
            var mergedSentence = Sentence.Model.Merge(RightSentence.Model);

            Sentence = new SentenceWrapper(mergedSentence);

            Sentence.UpdateContent();

            EventAggregator.GetEvent<UpdateSentenceEvent>().Publish(new UpdateSentenceEventData
            {
                Sentence = Sentence,
                DocumentId = leftSentenceDocumentId
            });

            RightSentence = null;
            RightSentenceInfo = null;
        }

        private async Task LoadSentencesWords()
        {
            if (Sentence == null || RightSentence == null)
            {
                return;
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
        }
    }
}