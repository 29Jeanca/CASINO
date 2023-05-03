using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using CASINO.Clases.Extras;
using Npgsql;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Faker;
using System.Data;
using System.Linq;
using System.Windows.Threading;
using System;
using System.Timers;

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
        public static int rangoCrupierTotal;
        public static string[] rangoCrupier = new string[4];
        public static int[] IndicesJugador = new int[4];
        public static int[] IndicesCrupier = new int[4];
        public static int contador = 0;
        public bool semaforo = false;
        public static string rutaAudioVictoria = @"D:\Casino\CASINO\Sonidos\sonidoVictoria.mp3";
        public static string rutaAudioDerrota = @"D:\Casino\CASINO\Sonidos\sonidoDerrota.mp3";
        public BlackJack()
        {
            InitializeComponent();
            Loaded += BlackJack_Loaded;


        }


        private void BlackJack_Loaded(object sender, RoutedEventArgs e)
        {
            nombreUsuario.Content = NombreEnPantalla();
            InicializarVariables();
            txtRangoCrupier.Text = contador.ToString();

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Btn_minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        public void InicializarVariables()
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
        private void Btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        public string NombreEnPantalla()
        {
            var id = Game.idUsuario;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaNombre = new NpgsqlCommand("SELECT nombre FROM usuarios WHERE id='" + id + "'", conexion);
            var nombre = (string)sentenciaNombre.ExecuteScalar();
            conx.CerrarConexion();
            return nombre;
        }

        private void BtnRepartir_Click(object sender, RoutedEventArgs e)
        {
            if (VerificarDinero())
            {
                JuegoUsuario();
                JuegoCrupier();
                contador++;
                ControlarBotones(btnPlantarse, true);
            }
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
                ControlarBotones(btnPlantarse, true);
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
                ControlarBotones(btnRepartir, false);
                rangoJugadorTotal += IndicesJugador[3];
                AvanceBarraProgreso(barraProgreso, rangoJugadorTotal);
            }
        }
        public void JuegoCrupier()
        {
            if (contador == 0)
            {
                RepartirCarta(txtRangoCrupier, txtPaloCrupier, rangoCrupier, 0);
                IndicesCrupier[0] = ObtenerValorRango(rangoCrupier[0]);
                RepartirCarta(txtRangoCrupier2, txtPaloCrupier2, rangoCrupier, 1);
                IndicesCrupier[1] = ObtenerValorRango(rangoCrupier[1]);
                rangoCrupierTotal = IndicesCrupier[0] + IndicesCrupier[1];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);
            }
            if (contador == 1 && rangoCrupierTotal < 17)
            {
                RepartirCarta(txtRangoCrupier3, txtPaloCrupier3, rangoCrupier, 2);
                IndicesCrupier[2] = ObtenerValorRango(rangoCrupier[2]);
                rangoCrupierTotal += IndicesCrupier[2];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);
            }
            if (contador == 2 && rangoCrupierTotal < 17)
            {
                RepartirCarta(txtRangoCrupier4, txtPaloCrupier4, rangoCrupier, 3);
                IndicesCrupier[3] = ObtenerValorRango(rangoCrupier[3]);
                rangoCrupierTotal += IndicesCrupier[3];
                AvanceBarraProgreso(barraProgresoCrupier, rangoCrupierTotal);

            }
        }


        public bool ValidarGanador()
        {

            if (rangoJugadorTotal > 21 || rangoCrupierTotal > 21)
            {
                if (rangoJugadorTotal > 21)
                {
                    textoFin.Text = "YOU LOSE";
                    return true;
                }
                else
                {
                    textoFin.Text = "YOU WIN";
                    return true;

                }

            }
            if (!btnRepartir.IsEnabled)
            {
                if (rangoJugadorTotal > rangoCrupierTotal)
                {
                    textoFin.Text = "YOU WIN";
                    return true;

                }
                else
                {
                    textoFin.Text = "YOU LOSE";
                    return true;

                }
            }


            return false;


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

        private void BtnPlantarse_Click(object sender, RoutedEventArgs e)
        {
            ControlarBotones(btnRepartir, false);
        }
        private void ControlarBotones(Button boton, bool habilitado)
        {
            boton.IsEnabled = habilitado;
        }

    }
}