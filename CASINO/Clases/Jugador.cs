using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CASINO.Clases
{
    public class Jugador : Persona
    {
        public static ConexionBD conx = new ConexionBD();
        public static int Saldo { get; set; }
        public static void AgregarJugador(Jugador jugador)
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaAgregar = "INSERT INTO usuarios (nombre, clave, email,edad,dni) VALUES('" + jugador.Nombre + "', '" + jugador.Clave + "', '" + jugador.Email + "'" + "," +
            "'" + jugador.Edad + "', '" + jugador.Dni + "')";
            NpgsqlCommand comando = new NpgsqlCommand(sentenciaAgregar, conexion);
            NpgsqlDataReader lector = comando.ExecuteReader();
        }
    }
}
