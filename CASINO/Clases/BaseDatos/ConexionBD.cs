﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CASINO.Clases.BaseDatos
{
    public class ConexionBD
    {
        NpgsqlConnection conexion = new NpgsqlConnection();

        static String servidor = "localhost";
        static String nombre_base_datos = "proyecto_casino";
        static String usuario = "postgres";
        static String clave = "root";
        static String puerto = "5432";

        String urlConexion = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + clave + ";" + "database=" + nombre_base_datos + ";";

        public NpgsqlConnection establecerConexion()
        {

            try
            {
                conexion.ConnectionString = urlConexion;
                conexion.Open();
                MessageBox.Show("Se conectó correctamente a la Base de datos");

            }

            catch (NpgsqlException e)
            {
                MessageBox.Show("No se pudo conectar a la base de datos de PostgreSQL, error: " + e.ToString());

            }

            return conexion;
        }
    }
}
