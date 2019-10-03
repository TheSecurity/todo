using System;
using ToDo.Domain;

namespace ToDo.WebApi.Entities
{
    public class User : IUnique
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}
