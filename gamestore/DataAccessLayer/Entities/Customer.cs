
namespace DataAccessLayer.Entities
{
    public class Customer
    {
        public string CustomerId { get; set; } = Guid.NewGuid().ToString();
    }
}
