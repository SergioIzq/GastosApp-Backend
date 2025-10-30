using System;
using System.Collections.Generic;

namespace AppG.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IList<string> errors)
            : base()
        {
            Errors = errors;
        }


        public IList<string> Errors { get; }
    }
}
