namespace GoodHamburger.Application.Src.Errors;

public sealed class DuplicateOrderItemsException(string message) : Exception(message);
