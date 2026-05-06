using Course.Domain.Exceptions;

namespace Course.Domain.Entities;

public sealed class Customer
{
    public Customer(Guid id, string fullName, string email)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("El cliente requiere un identificador.");
        }

        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new DomainException("El cliente requiere un nombre.");
        }

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("El cliente requiere un correo valido.");
        }

        Id = id;
        FullName = fullName;
        Email = email;
    }

    public Guid Id { get; }

    public string FullName { get; }

    public string Email { get; }
}
