using Course.Domain.Entities;
using Course.Domain.Exceptions;
using FluentAssertions;

namespace Course.UnitTest
{
    public class CustomerTests
    {

        [Fact]
        public void Consturctor_WhenEmailIsInvalid_ShouldThrowDomainException() 
        {

            // Arrange Act
            var act = () => new Customer(Guid.NewGuid(), "Juan perez", "JuanPerez");


            //Assert
            act.Should().Throw<DomainException>()
                .WithMessage("El cliente requiere un correo valido.");       
        }


        [Fact]
        public void Constructor_WhenDataIsValid_ShouldCreateCustomer()
        {
            //Arrange
            var id = Guid.NewGuid();

            //Act
            var customer = new Customer(id, "Juan Perez", "JuanPerez@gmail.com");

            //Assert
            Assert.Equal(id, customer.Id);
            Assert.Equal("Juan Perez", customer.FullName);
            Assert.Equal("JuanPerez@gmail.com", customer.Email);
        }

    }
}
