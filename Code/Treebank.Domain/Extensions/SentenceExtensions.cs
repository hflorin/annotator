namespace Treebank.Domain.Extensions
{
    using System;
    using System.Linq;

    public static class SentenceExtensions
    {
        public static Sentence Merge(this Sentence left, Sentence right)
        {
            if (left == null || right == null || !left.HasBeenLoaded || !right.HasBeenLoaded)
            {
                return left;
            }

            int lastWordInLeftSentenceId = GetLastWordId(left);

            foreach (var rightWord in right.Words)
            {
                if (!int.TryParse(rightWord.GetAttributeByName("id"), out int rightWordId))
                {
                    throw new InvalidOperationException($"Invalid id value for: sentence id {right.GetAttributeByName("id")}, word id: {rightWord.GetAttributeByName("id")}");
                }

                var newWordId = rightWordId + lastWordInLeftSentenceId;
                rightWord.SetAttributeByName("id", newWordId.ToString());

                if (!int.TryParse(rightWord.GetAttributeByName("head"), out int rightWordHeadId))
                {
                    throw new InvalidOperationException($"Invalid head id value for: sentence id {right.GetAttributeByName("id")}, , word id: {rightWord.GetAttributeByName("id")}, head word id: {rightWord.GetAttributeByName("head")}");
                }

                var newHeadWordId = rightWordHeadId + lastWordInLeftSentenceId;
                rightWord.SetAttributeByName("head", newHeadWordId.ToString());

                left.Words.Add(rightWord);
            }

            return left;
        }

        private static int GetLastWordId(Sentence input)
        {
            int id;
            if (!int.TryParse(input.Words.Last().GetAttributeByName("id"), out id))
            {
                id = input.Words.Count;
            }

            return id;
        }
    }
}