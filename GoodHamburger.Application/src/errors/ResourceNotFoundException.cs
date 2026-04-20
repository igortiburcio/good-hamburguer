namespace GoodHamburger.Application.Src.Errors;

public sealed class ResourceNotFoundException(string message) : Exception(message);
