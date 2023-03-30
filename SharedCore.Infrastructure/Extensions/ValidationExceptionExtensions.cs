using FluentValidation;

namespace SharedCore.Infrastructure.Extensions
{
    public static class ValidationExceptionExtensions
    {
        public static IDictionary<string, object> GetErrorsDictionary(this ValidationException exception)
        {
            var errorsDictionary = new Dictionary<string, object>();

            foreach (var error in exception.Errors!)
            {
                errorsDictionary.TryAdd(error.PropertyName, error.ErrorMessage);
            }

            return errorsDictionary;
        }
    }
}
