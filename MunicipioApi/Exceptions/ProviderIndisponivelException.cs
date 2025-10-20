namespace MunicipioApi.Api.Exceptions
{
    public class ProviderIndisponivelException : Exception
    {
        public ProviderIndisponivelException(string message) : base(message) { }
        public ProviderIndisponivelException(string message, Exception innerException) : base(message, innerException) { }
    }
}