using System;

namespace IntegerDataStructures
{
    public class ContractFailedException : Exception
    {
        public ContractFailedException()
        {
        }

        public ContractFailedException(string message) : base(message) 
        {
        }

        public ContractFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
