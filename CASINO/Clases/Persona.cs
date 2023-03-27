using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASINO.Clases
{
    public class Persona
    {
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
        public string Edad { get; set; }
        public string Dni { get; set; }
        public Persona() { }

        public Persona(string Nombre, string Email, string Clave, string Edad, string Dni)
        {
            this.Nombre = Nombre;
            this.Email = Email;
            this.Clave = Clave;
            this.Edad = Edad;
            this.Dni = Dni;

        }
    }
}
