namespace AppG.Exceptions
{
    public class CustomUnauthorizedAccessException : Exception
    {
        public IList<string> Errors { get; }

        public CustomUnauthorizedAccessException(IList<string> errorMessages) : base()
        {
            Errors = errorMessages;
        }
    }

}
