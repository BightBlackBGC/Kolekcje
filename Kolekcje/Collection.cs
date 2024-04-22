using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolekcje
{
    public class Collection
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public List<string> Items { get; set; } // Dodana właściwość Items

        public Collection()
        {
            Items = new List<string>(); // Inicjalizacja listy elementów
        }

        public override string ToString()
        {
            return Name + " - " + Type;
        }
    }
}
