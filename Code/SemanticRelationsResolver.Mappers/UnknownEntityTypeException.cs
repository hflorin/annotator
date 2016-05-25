namespace SemanticRelationsResolver.Mappers
{
    using System;

    public class UnknownEntityTypeException : Exception
    {
        public UnknownEntityTypeException()
        {
        }

        public UnknownEntityTypeException(string message)
            : base(message)
        {
        }
    }
}