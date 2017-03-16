namespace Treebank.Annotator.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Graph;
    using Mappers.Configuration;
    using Prism.Events;
    using View.Services;
    using Wrapper;
    using Wrapper.Base;

    public class DoubleSentenceEditorViewModel : SentenceEditorViewModel
    {
        private readonly GraphBuilder graphBuilder;
        private SentenceWrapper rightSentence;
        private StringWrapper rightStringWrapper;

        public DoubleSentenceEditorViewModel(
            IEventAggregator eventAggregator,
            IAppConfig appConfig,
            SentenceWrapper leftSentence,
            SentenceWrapper rightSentence,
            IShowInfoMessage showMessage):base(eventAggregator, appConfig,new DataStructure(), leftSentence,showMessage)
        {
            if (rightSentence == null)
            {
                throw new ArgumentNullException("rightSentence");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            if (showMessage == null)
            {
                throw new ArgumentNullException("showMessage");
            }

            EventAggregator = eventAggregator;
            RightSentence = rightSentence;
            graphBuilder = new GraphBuilder(appConfig, appConfig.Definitions.First());
            GraphConfigurations = new ObservableCollection<Definition>(appConfig.Definitions);
            SelectedGraphConfiguration = GraphConfigurations.First();
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
            if (Sentence != null && RightSentence != null)
            {
                graphBuilder.CurrentDefinition = SelectedGraphConfiguration;
                var logicCore = graphBuilder.SetupGraphLogic(Sentence, RightSentence);
                SentenceGraphLogicCore = logicCore;
            }
        }
        

    }
}