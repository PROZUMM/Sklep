using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklep.Context
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool isModerator { get; set; }
        public List<Order> Orders { get; set; } = new();

        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
    }
}
