using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Sklep.Context
{
    public class Order
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
       
        public List<Item> OrderedItems { get; set; } = new();

        public DateTime Date { get; set; }

        public double fullPrice { get; set; }
    }
}
