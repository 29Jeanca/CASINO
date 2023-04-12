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
        public static string[] rangoJugador = new string[4];
        public static int rangoJugadorTotal;
        public static bool semaforo = true;
        public static string[] rangoCrupier = new string[4];
        public static int[] IndicesJugador = new int[4];
        public static int[] IndicesCrupier = new int[4];
        int rangoCrupierTotal;
        public static int contador = 0;
        public static string rutaAudio = @"D:\Casino\CASINO\Sonidos\sonidoVictoria.mp3";
        public BlackJack()
        {
            InitializeComponent();
            Loaded += BlackJack_Loaded;
            txtRangoCrupier2.Visibility = Visibility.Hidden;
            txtPaloCrupier2.Visibility = Visibility.Hidden;

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
            JuegoUsuario();
            JuegoCrupier();
            contador++;
            btnPlantarse.IsEnabled = true;
        }

        public void JuegoUsuario()
        {
            Baraja.Barajar();
            if (contador == 0)
            {
                RepartirCarta(txtRangoJugador, txtPaloJugador, rangoJugador, 0);
                IndicesJugador[0] = ObtenerValorRango(rangoJugador[0]);
                RepartirCarta(txtRangoJugador2, txtPaloJugador2, rangoJugador, 1);
                IndicesJugador[1] += ObtenerValorRango(rangoJugador[1]);
                rangoJugadorTotal = IndicesJugador[0] + IndicesJugador[1];
                AvanceBarraProgreso(barraProgreso, rangoJugadorTotal);
                btnPlantarse.IsEnabled = true;
            }
            if (contador == 1)
            {
                txtRangoCrupier2.Visibility = Visibility.Visible;
                txtPaloCrupier2.Visibility = Visibility.Visible;
                RepartirCarta(txtRangoJugador3, txtPaloJugador3, rangoJugador, 2);
                IndicesJugador[2] = ObtenerValorRango(rangoJugador[2]);
                rangoJugadorTotal += IndicesJugador[2];
                AvanceBarraProgreso(barraProgreso, rangoJugadorTotal);

            }
            if (contador == 2)
            {
                RepartirCarta(txtRangoJugador4, txtPaloJugador4, rangoJugador, 3);
                IndicesJugador[3] = ObtenerValorRango(rangoJugador[3]);
                btnRepartir.IsEnabled = false;
                rangoJugadorTotal += IndicesJugador[3];
                AvanceBarraProgreso(barraProgreso, rangoJugadorTotal);
            }
        }
        public bool ValidarGanador()
        {
            if (rangoCrupierTotal == 21 || rangoCrupierTotal > rangoJugadorTotal && !btnRepartir.IsEnabled)
            {
                textoVictoria.Text = "You Lose";
                ControladorSonido.ReproducirAudio(rutaAudio);
                return false;
            }
            return true;

        }
        public void JuegoCrupier()
        {
            if (contador == 0)
            {
                RepartirCarta(txtRangoCrupier, txtPaloCrupier, rangoCrupier, 0);
                RepartirCarta(txtRangoCrupier2, txtPaloCrupier2, rangoCrupier, 1);
                IndicesCrupier[0] = ObtenerValorRango(rangoCrupier[0]);
                IndicesCrupier[1] = ObtenerValorRango(rangoCrupier[1]);
                rangoCrupierTotal = IndicesCrupier[0] + IndicesCrupier[1];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);
            }
            if (contador == 1 && rangoCrupierTotal < 17)
            {
                RepartirCarta(txtRangoCrupier3, txtPaloCrupier3, rangoCrupier, 2);
                rangoCrupierTotal += IndicesCrupier[2];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);
            }
            if (contador == 2 && rangoCrupierTotal < 17)
            {
                RepartirCarta(txtRangoCrupier4, txtPaloCrupier4, rangoCrupier, 3);
                rangoCrupierTotal += IndicesCrupier[3];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);
            }
        }
        private void RepartirCarta(TextBlock txtRango, TextBlock txtPalo, string[] rangoArray, int cartaIndex)
        {
            List<Carta> cartasRepartidas = baraja.Repartir(13);
            rangoArray[cartaIndex] = cartasRepartidas[cartaIndex].rango;
            txtRango.Text = $"{cartasRepartidas[cartaIndex].rango}";
            txtPalo.Text = $"{cartasRepartidas[cartaIndex].palo}";
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
        public void AvanceBarraProgreso(ProgressBar Progreso, int total)
        {
            Progreso.Value = total;
        }

        private void btnPlantarse_Click(object sender, RoutedEventArgs e)
        {
            btnRepartir.IsEnabled = false;
        }

    }
}