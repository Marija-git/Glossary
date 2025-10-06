

namespace Glossary.BusinessLogic.Exceptions
{
    public class ForbidException : Exception
    {
        public ForbidException(string message = "Access forbidden.") : base(message) { }
    }
}
