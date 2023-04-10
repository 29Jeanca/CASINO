using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using CASINO.Clases.Extras;
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
        public static string[] rangoJugador = new string[3];
        public static int rangoJugadorTotal;
        public static string[] rangoCrupier = new string[3];
        public static int rangoCrupierTotal;
        public static int contador = 0;
        public static string rutaAudio = @"D:\Casino\CASINO\Sonidos\sonidoVictoria.mp3";
        public BlackJack()
        {
            InitializeComponent();
            Loaded += BlackJack_Loaded;
        }


        private void BlackJack_Loaded(object sender, RoutedEventArgs e)
        {
            nombreUsuario.Content = nombreEnPantalla();
            inicializarVariables();
            txtRangoCrupier.Text = contador.ToString();

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
            int[] rangosInt = new int[4];
            contador++;
            List<Carta> cartasRepartidas = baraja.Repartir(3);
            if (VerificarDinero() && contador == 1)
            {
                baraja.Barajar();
                rangoJugador[0] = cartasRepartidas[0].rango;
                txtRangoJugador.Text = $"{cartasRepartidas[0].rango}";
                txtPaloJugador.Text = $"{cartasRepartidas[0].palo}";
                rangoJugador[1] = cartasRepartidas[1].rango;
                txtRangoJugador2.Text = $"{cartasRepartidas[1].rango}";
                txtPaloJugador2.Text = $"{cartasRepartidas[1].palo}";
                rangosInt[0] = ObtenerValorRango(rangoJugador[0]);
                rangosInt[1] = ObtenerValorRango(rangoJugador[1]);
                rangoJugadorTotal = rangosInt[0] + rangosInt[1];
                AvanceBarraProgreso(rangoJugadorTotal);
                btnPlantarse.IsEnabled = true;
            }
            else if (contador == 2 && btnPlantarse.IsEnabled)
            {
                rangoJugador[2] = cartasRepartidas[2].rango;
                txtRangoJugador3.Text = $"{cartasRepartidas[2].rango}";
                txtPaloJugador3.Text = $"{cartasRepartidas[2].palo}";
                btnRepartir.IsEnabled = false;
                rangosInt[0] = ObtenerValorRango(rangoJugador[0]);
                rangosInt[1] = ObtenerValorRango(rangoJugador[1]);
                rangosInt[2] = ObtenerValorRango(rangoJugador[2]);
                rangoJugadorTotal = rangosInt[0] + rangosInt[1] + rangosInt[2];
                AvanceBarraProgreso(rangoJugadorTotal);
            }

            if (!VerificarDinero())
            {
                MessageBox.Show("If you don't have money in the menu, you can access a loan. " +
                    "Remember that if you are PREMIUM, you have the benefits " +
                    "of:\r\n\r\nx2 earnings\r\n15% reduction in the price of playing.", "IMPORTANT");
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
            else
            {
                return int.Parse(rango);
            }

        }
        public void AvanceBarraProgreso(int total)
        {
            barraProgreso.Value = total;
        }

        private void btnPlantarse_Click(object sender, RoutedEventArgs e)
        {

            ControladorSonido.ReproducirAudio(rutaAudio);
        }

    }
}

