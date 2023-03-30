namespace SharedCore.Model.Exceptions
{
    [Serializable]
    public class AppException : Exception
    {
        public string? Detail { get; }
        public IDictionary<string, object>? Extensions { get; }

        public AppException(
            string message,
            string? detail = null,
            IDictionary<string, object>? extensions = null,
            Exception? innerException = null)
                : base(message, innerException)
        {
            Detail = detail;
            Extensions = extensions ?? new Dictionary<string, object>();
        }

        protected AppException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
