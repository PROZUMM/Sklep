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
        public List<Item> Items { get; set; } = new();

        public virtual User User { get; set; }
    }
}
