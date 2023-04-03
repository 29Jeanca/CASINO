using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Net.Mime.MediaTypeNames.Application;

namespace CASINO.Vistas
{
    /// <summary>
    /// Lógica de interacción para Game.xaml
    /// </summary>
    public partial class Game : Window
    {
        public static ConexionBD conx = new ConexionBD();
        public static int idUsuario;
        public static string pepiColons;
        public static List<int> ints = new List<int>();
        public static string filePath = @"D:\Casino\CASINO\txt\iniciados.txt";
        public Game()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        /// <summary>
        /// Este método tiene una sentencia para poder guardar en una variable el id del usuario
        /// Esto funciona para guardarlo en un txt y controlar el inicio de sesión de cada usuario.
        /// </summary>
        public void seleccionarId()
        {
            var nombrePantalla = nombreUsuario.Text;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var comando = new NpgsqlCommand("SELECT id FROM usuarios WHERE email='" + nombrePantalla + "'", conexion);
            idUsuario = (int)comando.ExecuteScalar();
            MessageBox.Show(idUsuario + "");
            TextReader leerArchivo = new StreamReader(filePath);
            if (!leerArchivo.ReadToEnd().Contains(idUsuario.ToString()))
            {
                datosUsuario();
                escribirArchivo();

            }
        }
        public void inicializarVariables()
        {
            var cantidadMonedas = txtCantMonedas.Text;
            int.Parse(cantidadMonedas);
            txtCantMonedas.Text = Jugador.Saldo = "20000";

        }

        /// <summary>
        /// Inserta dentro de la tabla jugadoresActivos el id de ese jugador y las variables de saldo, deuda
        /// y si es un jugador premium o no.
        /// </summary>
        public void datosUsuario()
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaUsuario = "INSERT INTO jugadoresActivos (id_jugador,saldo,deuda,premium) VALUES('" + idUsuario + "','" + Jugador.Saldo + "','" + Jugador.Deuda + "','" + Jugador.Premium + "')";
            NpgsqlCommand comando = new NpgsqlCommand(sentenciaUsuario, conexion);
            comando.ExecuteNonQuery();

        }
        /// <summary>
        /// Escribe en el txt el id del usuario
        /// </summary>
        public void escribirArchivo()
        {


            if (!File.Exists(filePath))
            {
                TextWriter usuariosIniciados = new StreamWriter(filePath);
                string texto = idUsuario.ToString();
                usuariosIniciados.WriteLine(texto);
                usuariosIniciados.Close();
            }
            else
            {
                TextWriter usuariosIniciados2 = File.AppendText(filePath);
                string texto = idUsuario.ToString();
                usuariosIniciados2.WriteLine(texto);
                usuariosIniciados2.Close();
            }

        }


        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnJugar_Click(object sender, RoutedEventArgs e)
        {
            seleccionarId();
            conx.CerrarConexion();
            //BlackJack black = new BlackJack();
            //black.Show();
            //Close();
        }



        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
