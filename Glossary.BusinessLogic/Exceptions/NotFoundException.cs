

namespace Glossary.BusinessLogic.Exceptions
{
    public class NotFoundException(string resourceType, string resourceIdentifier)
    : Exception($"{resourceType} with identifier: {resourceIdentifier} doesn't exist.")
    {
    }
}
