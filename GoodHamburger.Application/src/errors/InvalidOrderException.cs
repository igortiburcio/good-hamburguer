namespace GoodHamburger.Application.Src.Errors;

public sealed class InvalidOrderException(string message) : Exception(message);
