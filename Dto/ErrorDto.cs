using System.Collections.Generic;

namespace Mashawi.Dto
{
    public class ErrorDto
    {
        public string Description { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}