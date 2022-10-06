namespace DecidioTestExcersice.Errors
{
    public class InvalidMailAddressException: Exception
    {
        public InvalidMailAddressException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
