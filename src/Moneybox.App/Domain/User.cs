using System;

namespace Moneybox.App
{
    public sealed class User
    {
        private User() { }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public static User Load(string name, string email) => 
            new User
            { 
                Id = Guid.NewGuid(),
                Name = name,
                Email = email
            };
    }
}
