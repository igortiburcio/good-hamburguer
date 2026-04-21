namespace GoodHamburger.Domain.Src;

public class Money(decimal amount)
{
    public decimal Amount { get; } = Math.Round(amount, 2, MidpointRounding.AwayFromZero);

    public override string ToString()
    {
        return Amount.ToString("C");
    }
}
