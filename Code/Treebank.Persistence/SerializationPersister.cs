namespace Treebank.Persistence
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using Domain;
    using Mappers.Serialization.Models;
    using Sentence = Domain.Sentence;
    using Word = Domain.Word;

    public class SerializationPersister : IPersister
    {
        public async Task Save(Document document, string filepathToSaveTo = "", bool overwrite = true)
        {
            var filepath = string.IsNullOrWhiteSpace(filepathToSaveTo) ? document.FilePath : filepathToSaveTo;

            if (string.IsNullOrWhiteSpace(filepath))
            {
                return;
            }

            var fileName = Path.GetFileName(filepath);
            var newFilepath = filepath.Replace(fileName, "New" + fileName);

            var treebank = LoadTreebank(document.FilePath);

            DeleteSentencesFromDocumentIfRemovedInCurrentSession(document, treebank);

            foreach (var sentence in document.Sentences)
            {
                if (IsNewSentence(sentence, treebank))
                {
                    AddNewSentenceToTreebank(sentence, treebank);
                }
                else
                {
                    SaveModifiedSentenceToTreebank(sentence, treebank);
                }
            }

            var ser = new XmlSerializer(typeof(Treebank));
            using (var writer = File.OpenWrite(newFilepath))
            {
                ser.Serialize(writer, treebank, new XmlSerializerNamespaces(new[] {XmlQualifiedName.Empty}));
            }

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            File.Move(newFilepath, filepath);
            document.FilePath = filepath;
        }

        private static void DeleteSentencesFromDocumentIfRemovedInCurrentSession(Document document, Treebank treebank)
        {
            var persistedSentencesIds = treebank.Sentences.Select(s => s.Id);
            var documentSentencesIds = document.Sentences.Select(s => s.GetAttributeByName("id"));

            var sentencesToRemove = persistedSentencesIds.Except(documentSentencesIds).ToList();

            sentencesToRemove.ForEach(id =>
            {
                var sentenceToRemove =
                    treebank
                        .Sentences
                        .FirstOrDefault(ts => ts.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
                treebank.Sentences.Remove(sentenceToRemove);
            });
        }

        private static void SaveModifiedSentenceToTreebank(Sentence sentence, Treebank treebank)
        {
            var outputSentence =
                treebank.Sentences.FirstOrDefault(
                    s => s.Id.Equals(sentence.GetAttributeByName("id"), StringComparison.InvariantCultureIgnoreCase));

            if (outputSentence != null)
            {
                CopySentenceAttributes(sentence, outputSentence);

                if (sentence.Words.Count > 0)
                {
                    DeleteWordsFromSentenceIfRemovedInCurrentSession(sentence, outputSentence);

                    foreach (var sentenceWord in sentence.Words)
                    {
                        if (IsNewWord(sentenceWord, outputSentence))
                        {
                            AddNewWord(sentenceWord, outputSentence);
                        }
                        else
                        {
                            SaveModifiedWord(sentenceWord, outputSentence);
                        }
                    }
                }
            }
        }

        private static void CopySentenceAttributes(Sentence sentence,
            Mappers.Serialization.Models.Sentence outputSentence)
        {
            if (outputSentence == null || sentence == null)
            {
                return;
            }
            var sentenceId = string.IsNullOrWhiteSpace(sentence.GetAttributeByName("newid")) ? 
                sentence.GetAttributeByName("id") : sentence.GetAttributeByName("newid");
            outputSentence.Id = sentenceId;
            outputSentence.CitationPart = sentence.GetAttributeByName("citation-part");
            outputSentence.Date = sentence.GetAttributeByName("date");
            outputSentence.Parser = sentence.GetAttributeByName("parser");
            outputSentence.User = sentence.GetAttributeByName("user");
        }

        private static void DeleteWordsFromSentenceIfRemovedInCurrentSession(Sentence sentence,
            Mappers.Serialization.Models.Sentence outputSentence)
        {
            var persistedWordsIds = outputSentence.Words.Select(s => s.Id);
            var documentWordsIds = sentence.Words.Select(s => s.GetAttributeByName("id"));

            var sentencesToRemove = persistedWordsIds.Except(documentWordsIds).ToList();

            sentencesToRemove.ForEach(id =>
            {
                var sentenceToRemove =
                    outputSentence
                        .Words
                        .FirstOrDefault(ts => ts.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase));
                outputSentence.Words.Remove(sentenceToRemove);
            });
        }

        private static void SaveModifiedWord(Word sentenceWord, Mappers.Serialization.Models.Sentence outputSentence)
        {
            var outputWord =
                outputSentence.Words.FirstOrDefault(
                    w =>
                        w.Id.Equals(sentenceWord.GetAttributeByName("id"),
                            StringComparison.InvariantCultureIgnoreCase));

            if (outputWord != null)
            {
                CopyWordAttributes(sentenceWord, outputWord);
            }
        }

        private static void CopyWordAttributes(Word sentenceWord, Mappers.Serialization.Models.Word outputWord)
        {
            if (sentenceWord == null || outputWord == null)
            {
                return;
            }

            outputWord.Id = sentenceWord.GetAttributeByName("id");
            outputWord.Chunk = sentenceWord.GetAttributeByName("chunk");
            outputWord.DepRel = sentenceWord.GetAttributeByName("deprel");
            outputWord.Form = sentenceWord.GetAttributeByName("form");
            outputWord.Head = sentenceWord.GetAttributeByName("head");
            outputWord.Lemma = sentenceWord.GetAttributeByName("lemma");
            outputWord.Postag = sentenceWord.GetAttributeByName("postag");
        }

        private static void AddNewWord(Word sentenceWord, Mappers.Serialization.Models.Sentence outputSentence)
        {
            outputSentence.Words.Add(MapOutputWord(sentenceWord));
        }

        private static Mappers.Serialization.Models.Word MapOutputWord(Word sentenceWord)
        {
            return new Mappers.Serialization.Models.Word
            {
                Id = sentenceWord.GetAttributeByName("id"),
                Chunk = sentenceWord.GetAttributeByName("chunk"),
                DepRel = sentenceWord.GetAttributeByName("deprel"),
                Form = sentenceWord.GetAttributeByName("form"),
                Head = sentenceWord.GetAttributeByName("head"),
                Lemma = sentenceWord.GetAttributeByName("lemma"),
                Postag = sentenceWord.GetAttributeByName("postag")
            };
        }

        private static bool IsNewWord(Word sentenceWord, Mappers.Serialization.Models.Sentence outputSentence)
        {
            return
                !outputSentence.Words.Any(
                    w => w.Id.Equals(sentenceWord.GetAttributeByName("id"), StringComparison.InvariantCultureIgnoreCase));
        }

        private static void AddNewSentenceToTreebank(Sentence sentence, Treebank treebank)
        {
            treebank.Sentences.Add(MapOutputSentence(sentence));
        }

        private static Mappers.Serialization.Models.Sentence MapOutputSentence(Sentence sentence)
        {
            var sentenceId = string.IsNullOrWhiteSpace(sentence.GetAttributeByName("newid")) ?
                sentence.GetAttributeByName("id") : sentence.GetAttributeByName("newid");

            return new Mappers.Serialization.Models.Sentence
            {
                Id = sentenceId,
                CitationPart = sentence.GetAttributeByName("citation-part"),
                Date = sentence.GetAttributeByName("date"),
                Parser = sentence.GetAttributeByName("parser"),
                User = sentence.GetAttributeByName("user"),
                Words = sentence.Words.Select(MapOutputWord).ToList()
            };
        }

        private static bool IsNewSentence(Sentence sentence, Treebank treebank)
        {
            return !treebank.Sentences.Any(
                s => s.Id.Equals(sentence.GetAttributeByName("id"), StringComparison.InvariantCultureIgnoreCase));
        }

        private static Treebank LoadTreebank(string filepath)
        {
            var ser = new XmlSerializer(typeof(Treebank));
            using (var reader = XmlReader.Create(filepath))
            {
                return (Treebank) ser.Deserialize(reader);
            }
        }
    }
}