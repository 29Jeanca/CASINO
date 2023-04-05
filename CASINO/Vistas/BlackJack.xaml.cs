using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CASINO.Vistas
{
    /// <summary>
    /// Lógica de interacción para BlackJack.xaml
    /// </summary>
    public partial class BlackJack : Window
    {
        public static ConexionBD conx = new ConexionBD();
        public static Game game = new Game();
        public static Baraja baraja = new Baraja();
        public BlackJack()
        {
            InitializeComponent();
            Loaded += BlackJack_Loaded;
        }


        private void BlackJack_Loaded(object sender, RoutedEventArgs e)
        {
            nombreUsuario.Content = nombreEnPantalla();

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void btn_minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public string nombreEnPantalla()
        {

            var id = Game.idUsuario;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaNombre = new NpgsqlCommand("SELECT nombre FROM usuarios WHERE id='" + id + "'", conexion);
            var nombre = (string)sentenciaNombre.ExecuteScalar();
            conx.CerrarConexion();
            return nombre;
        }


        private void btnRepartir_Click(object sender, RoutedEventArgs e)
        {
            baraja.Barajar();
            List<Carta> cartasRepartidas = baraja.Repartir(1);
            var rango = "";
            var palo = "";
            foreach (Carta cartas in cartasRepartidas)
            {
                rango += cartas.rango;
                palo += cartas.palo;
            }
            txtpalo.Text = palo;
            txtrango.Text = rango;
            AvancebarraProgreso();
        }
        public void AvancebarraProgreso()
        {
            string rangoCarta = txtrango.Text;
            if (rangoCarta == "J")
            {
                barraProgreso.Value = 11;
            }
            else if (rangoCarta == "Q")
            {
                barraProgreso.Value = 12;
            }
            else if (rangoCarta == "K")
            {
                barraProgreso.Value = 13;
            }
            else if (rangoCarta == "A")
            {
                barraProgreso.Value = 1;
            }
            else
            {
                double rangoCartaD = double.Parse(rangoCarta);
                barraProgreso.Value = rangoCartaD;
            }
        }
    }
}
