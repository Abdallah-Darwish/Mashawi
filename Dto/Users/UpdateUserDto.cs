using Mashawi.Db.Entities;
using Mashawi.Dto.OrdersAddresses;

namespace Mashawi.Dto.Users
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        //Maybe we should remove this with auto enrollment
        public UpdateOrderAddressDto? Address { get; set; }
        public UserRole? Role { get; set; }
    }
}