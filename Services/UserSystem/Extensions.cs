using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mashawi.Db.Entities;

namespace Mashawi.Services.UserSystem
{
    public static class Extensions
    {
        public static User? GetUser(this ControllerBase controller) => GetUser(controller.HttpContext);
        public static User? GetUser(this HttpContext context) => context.Features.Get<User?>();
    }
}