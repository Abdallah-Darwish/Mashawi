using Mashawi.Db.Entities;
using Mashawi.Dto.OrdersAddresses;

namespace Mashawi.Dto.Users
{
    public class UserDto : UserMetadataDto
    {
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public OrderAddressDto Address { get; set; }
        public string Phone { get; set; }
    }
}