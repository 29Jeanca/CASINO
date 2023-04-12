using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASINO.Clases
{
    public class Baraja
    {
        public static List<Carta> Cartas { get; set; }

        public Baraja()
        {
            Cartas = new List<Carta>();
            string[] palos = new string[] { "Corazones", "Diamantes", "Tréboles", "Picas" };
            string[] rangos = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

            foreach (string palo in palos)
            {
                foreach (string rango in rangos)
                {
                    Cartas.Add(new Carta(palo, rango));
                }
            }
        }
        public static void Barajar()
        {
            Random random = new Random();
            int cantCartas = Cartas.Count;
            while (cantCartas > 1)
            {
                cantCartas--;
                int k = random.Next(cantCartas + 1);
                Carta valor = Cartas[k];
                Cartas[k] = Cartas[cantCartas];
                Cartas[cantCartas] = valor;
            }
        }
        public List<Carta> Repartir(int cantidad)
        {
            List<Carta> cartasRepartidas = new List<Carta>();
            for (int i = 0; i < cantidad; i++)
            {
                cartasRepartidas.Add(Cartas[0]);
                Cartas.RemoveAt(0);
            }
            return cartasRepartidas;
        }


    }
}
