namespace Treebank.Mappers.Algos
{
    using System.Collections.Generic;

    public class CheckGraphResult
    {
        public CheckGraphResult()
        {
            DisconnectedWordIds = new List<string>();
            Cycles = new List<List<string>>();
        }

        public List<string> DisconnectedWordIds { get; set; }
        public List<List<string>> Cycles { get; set; }
    }
}