using Course.Domain.Enums;
using Course.Domain.Exceptions;

namespace Course.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Order(Guid id, Guid customerId, DateTimeOffset createdAtUtc)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El pedido requiere un identificador.");
        }

        if (customerId == Guid.Empty)
        {
            throw new DomainException("El pedido requiere un cliente.");
        }

        Id = id;
        CustomerId = customerId;
        CreatedAtUtc = createdAtUtc;
        Status = OrderStatus.Created;
    }

    public Guid Id { get; }

    public Guid CustomerId { get; }

    public DateTimeOffset CreatedAtUtc { get; }

    public OrderStatus Status { get; private set; }

    public Payment? Payment { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public decimal Total => _items.Sum(item => item.Total);

    public void AddItem(Product product, int quantity)
    {
        EnsureCanModify();
        _items.Add(OrderItem.FromProduct(product, quantity));
    }

    public decimal CalculateTotal() => Total;

    public void RegisterPayment(Payment payment)
    {
        if (!_items.Any())
        {
            throw new DomainException("No se puede pagar un pedido sin items.");
        }

        if (payment.OrderId != Id)
        {
            throw new DomainException("El pago no pertenece al pedido.");
        }

        if (payment.Amount != Total)
        {
            throw new DomainException("El monto del pago no coincide con el total.");
        }

        Payment = payment;
        Status = payment.Status == PaymentStatus.Approved ? OrderStatus.Paid : OrderStatus.PaymentRejected;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Cancelled)
        {
            return;
        }

        if (Status == OrderStatus.Paid)
        {
            throw new DomainException("No se puede cancelar un pedido pagado.");
        }

        Status = OrderStatus.Cancelled;
    }

    private void EnsureCanModify()
    {
        if (Status != OrderStatus.Created)
        {
            throw new DomainException("Solo se pueden modificar pedidos en estado creado.");
        }
    }
}
