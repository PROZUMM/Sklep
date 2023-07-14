using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sklep.Context
{
    public class Cart
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<Item> Items { get; set; }
    }
}
