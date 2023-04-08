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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Net.Mime.MediaTypeNames.Application;
using MessageBox = System.Windows.MessageBox;

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
        public static string filePath = @"D:\Casino\CASINO\txt\iniciados.txt";
        public static int hora;
        public static string nombrePantalla;
        string reglas = "1- The objective of the game is to get a hand with a total value closer to 21 than the dealer's.\n\n" +
                        "2- Each numbered card has its face value, face cards are worth 10, and an ace can be worth 1 or 11, depending on what the player chooses.\n\n" +
                        "3- Players are dealt two cards and can request more to get closer to 21, but if they go over 21, they automatically lose.\n\n" +
                        "4- Once all players have finished their turn, the dealer reveals their cards and tries to make a hand higher than the players' hands without going over 21.\n\n" +
                        "5- If the dealer's and a player's hands have the same value, it's considered a tie, and the original bet is returned.\n\n" +
                        "6- If the player's hand is higher than the dealer's without going over 21, they get double the original bet back.";


        public Game()
        {
            InitializeComponent();
            Loaded += Game_Loaded;
            DateTime horaActual = DateTime.Now;
            hora = horaActual.Hour;

        }
        public void horaSaludo()
        {
            var saludoDia = "Good Morning";
            var saludoTarde = "Good Afternoon";
            var saludoNoche = "Good Night";
            if (hora <= 11)
            {
                saludoUsuario.Text = saludoDia;
            }
            if (hora >= 12)
            {
                saludoUsuario.Text = saludoTarde;
            }
            if (hora >= 19 || hora <= 00)
            {
                saludoUsuario.Text = saludoNoche;
            }

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Game_Loaded(object sender, RoutedEventArgs e)
        {

            horaSaludo();
            seleccionarId();
            inicializarVariables();
        }
        /// <summary>
        /// Este método tiene una sentencia para poder guardar en una variable el id del usuario
        /// Esto funciona para guardarlo en un txt y controlar el inicio de sesión de cada usuario.
        /// </summary>
        public void seleccionarId()
        {
            nombrePantalla = nombreUsuario.Text;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var comando = new NpgsqlCommand("SELECT id FROM usuarios WHERE email='" + SignIn.email + "'", conexion);
            idUsuario = (int)comando.ExecuteScalar();
            conx.CerrarConexion();
            TextReader leerArchivo = new StreamReader(filePath);
            if (!leerArchivo.ReadToEnd().Contains(idUsuario.ToString()))
            {
                MessageBox.Show("Está leyendo el archivo");
                datosUsuario();
                leerArchivo.Close();
                escribirArchivo();
            }



        }
        public void inicializarVariables()
        {

            NpgsqlConnection conexion = conx.EstablecerConexion();
            var comando = new NpgsqlCommand("SELECT saldo FROM jugadoresActivos WHERE id_jugador='" + idUsuario + "'", conexion);
            var saldoUsuario = (string)comando.ExecuteScalar();
            conx.CerrarConexion();
            var saldoInt = int.Parse(saldoUsuario);
            if (saldoInt <= 0)
            {
                txtCantMonedas.Foreground = Brushes.Red;
            }
            txtCantMonedas.Text = saldoInt.ToString();
            conexion = conx.EstablecerConexion();
            var sentenciaEmail = new NpgsqlCommand("SELECT email FROM usuarios WHERE id='" + idUsuario + "'", conexion);
            var email = (string)sentenciaEmail.ExecuteScalar();
            conx.CerrarConexion();
            nombreUsuario.Text = email;

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
            conx.CerrarConexion();

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
            MessageBoxResult result = MessageBox.Show("The value to play is 5000 pepiColons," +
           "please note that UPON ENTRY you will make the PAYMENT.", "IMPORTANT", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                BlackJack black = new BlackJack();
                black.Show();
                Close();
            }
            else
            {
                MessageBox.Show("If you're out of money, remember that in the menu tab you can access a loan." +
                "If you're a premium user, your earnings are doubled and the cost of the game is 15% less.");
            }


        }



        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnAbrirMenu_Click(object sender, RoutedEventArgs e)
        {
            if (barraMenu.IsVisible)
            {
                barraMenu.Visibility = Visibility.Hidden;
            }
            else
            {
                barraMenu.Visibility = Visibility.Visible;

            }


        }

        private void btnReglas_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(reglas, "RULES");
        }
    }
}
