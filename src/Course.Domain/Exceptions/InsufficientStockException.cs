namespace Course.Domain.Exceptions;

public sealed class InsufficientStockException : DomainException
{
    public InsufficientStockException(string productName, int requested, int available)
        : base($"Stock insuficiente para {productName}. Solicitado: {requested}, disponible: {available}.")
    {
        ProductName = productName;
        Requested = requested;
        Available = available;
    }

    public string ProductName { get; }

    public int Requested { get; }

    public int Available { get; }
}
