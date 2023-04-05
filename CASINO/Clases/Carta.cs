using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASINO.Clases
{
    public class Carta
    {
        public string palo { get; set; }
        public string rango { get; set; }
        public int valor { get; set; }
        public Carta(String palo, string rango)
        {
            this.palo = palo;
            this.rango = rango;
            if (rango == "J")
                valor = 11;
            else if (rango == "Q")
                valor = 12;
            else if (rango == "K")
            {
                valor = 13;
            }
            else if (rango == "A")
                valor = 14;
            else
                valor = int.Parse(rango);
        }


    }
}
