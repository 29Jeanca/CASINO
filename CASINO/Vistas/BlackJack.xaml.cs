using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        public static string rango1;
        public static string rango2;
        public static string rango3;

        public BlackJack()
        {
            InitializeComponent();
            Loaded += BlackJack_Loaded;
        }


        private void BlackJack_Loaded(object sender, RoutedEventArgs e)
        {
            nombreUsuario.Content = nombreEnPantalla();
            inicializarVariables();

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
        public void inicializarVariables()
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var comando = new NpgsqlCommand("SELECT saldo FROM jugadoresActivos WHERE id_jugador='" + Game.idUsuario + "'", conexion);
            var saldoUsuario = (string)comando.ExecuteScalar();
            conx.CerrarConexion();
            var saldoInt = int.Parse(saldoUsuario);
            if (saldoInt <= 0)
            {
                CantidadpepiColons.Foreground = Brushes.Red;
            }
            CantidadpepiColons.Text = saldoInt.ToString();

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
            EmpezarJuego();
        }
        public void EmpezarJuego()
        {
            if (VerificarDinero())
            {
                Game game = new Game();
                game.Show();
                Close();
                //baraja.Barajar();
                //List<Carta> cartasRepartidas = baraja.Repartir(4);
                //rango1 = cartasRepartidas[0].rango;
                //txtrango.Text = $"{cartasRepartidas[0].rango}";
                //txtpalo.Text = $"{cartasRepartidas[0].palo}";
                //rango2 = cartasRepartidas[1].rango;
                //txtrango2.Text = $"{cartasRepartidas[1].rango}";
                //txtpalo2.Text = $"{cartasRepartidas[1].palo}";
                //rango3 = cartasRepartidas[2].rango;
                //txtrango3.Text = $"{cartasRepartidas[2].rango}";
                //txtpalo3.Text = $"{cartasRepartidas[2].palo}";
                //AvanceBarraProgreso();
            }
            else
            {
                MessageBox.Show("You have insufficient funds.");
                Game game = new Game();
                game.Show();
                Close();
            }
        }
        public bool VerificarDinero()
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaDinero = new NpgsqlCommand("SELECT saldo FROM jugadoresActivos WHERE id_jugador= '" + Game.idUsuario + "'", conexion);
            var dinero = (string)sentenciaDinero.ExecuteScalar();
            conx.CerrarConexion();
            int dineroInt = int.Parse(dinero);
            if (dineroInt >= 5000)
            {
                return true;
            }
            return false;
        }
        public int ObtenerValorRango(string rango)
        {
            if (rango == "J" || rango == "Q" || rango == "K" || rango == "A")
            {
                return 10;
            }
            return int.Parse(rango);

        }
        public void AvanceBarraProgreso()
        {
            var valorBarra = ObtenerValorRango(rango1) + ObtenerValorRango(rango2);
            barraProgreso.Value = valorBarra;
        }
    }
}

