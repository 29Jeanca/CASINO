using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace CASINO.Vistas
{
    /// <summary>
    /// Lógica de interacción para CreateAccount.xaml
    /// </summary>
    public partial class CreateAccount : Window
    {
        public static string nombreUsuario;
        public static string claveUsuario;
        public static string correoUsuario;
        public static int edad;
        public static string dniUsuario;
        public static DateTime fechaActual = DateTime.Now;
        public Jugador jugador = new Jugador();//Instancia de la clase jugador para poder hacer la inserción a la BD
        public ConexionBD conx = new ConexionBD();
        public CreateAccount()
        {
            InitializeComponent();
            SecureString secureString = UserPasswordBox.SecurePassword;

        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                //carita_user.IsChecked = true; Esto cambia del sad al happy la carita, lo usaré pronto
            }
        }
        /// <summary>
        /// Este método lo que hace es darle el valor a las variables
        /// pone lo que esté en el input en la respectiva variable.
        /// 
        /// </summary>
        public void Inicializar_Inputs()
        {
            SecureString claveUsuario = UserPasswordBox.SecurePassword;
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;
            var ClaveEncript = EncriptarClave.Encript(claveUsuarioString);
            jugador.Nombre = UserNameBox.Text;
            jugador.Email = UserEmailBox.Text;
            jugador.Clave = ClaveEncript;
            jugador.Edad = ConversorFechas();
            jugador.Dni = UserDniBox.Text;
        }
        public void Insercion_BaseDatos()
        {
            Inicializar_Inputs();
            List<Jugador> cantJugadores = new List<Jugador>();
            Jugador.AgregarJugador(jugador);
            cantJugadores.Add(jugador);
            if (cantJugadores.Count() > 0)
            {
                MessageBox.Show("Se insertó el usuario " + jugador.Nombre);
            }
            else
            {
                MessageBox.Show("No se insertó");

            }
        }

        public int ConversorFechas()
        {
            var fechaActualString = fechaActual.ToString("dd/MM/yyy");//Se pasa la fecha actual a String con el formato día,mes y año
            DateTime? fechaUsuario = UserBithDate.SelectedDate;//Se instancia la fecha del usuario como DateTime, y se le agrega el ? para asegurar que no va a ser nulo
            var fechaCumpleUsuarioString = fechaUsuario.ToString();//Se pasa la fecha del usuario a String
            DateTime fechaCumpleUsuario = DateTime.Parse(fechaCumpleUsuarioString);//Se parsea la fecha del usuario para obtener el año
            int anioUsuario = fechaCumpleUsuario.Year;
            DateTime fechaHoy = DateTime.Parse(fechaActualString);//Se parsea la fecha actual para obtener el año
            int anioHoy = fechaHoy.Year;
            int restaAnioUsuario_Actual = anioHoy - anioUsuario;
            if (restaAnioUsuario_Actual < 18)
            {
                return restaAnioUsuario_Actual;
            }
            else
            {
                return restaAnioUsuario_Actual;

            }
        }

        public bool ValidacionInputs()
        {
            nombreUsuario = UserNameBox.Text;
            correoUsuario = UserEmailBox.Text;
            claveUsuario = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;
            var fechaUsuario = UserBithDate.SelectedDate;
            dniUsuario = UserDniBox.Text;



            return true;
        }
        public void ValidarExistenciaEmailDni()
        {
            correoUsuario = UserEmailBox.Text;
            dniUsuario = UserDniBox.Text;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaExisteEmail = "SELECT email FROM usuarios WHERE email=@semail";
            var sentenciaExisteDni = "SELECT dni FROM usuarios WHERE dni=@sdni";
            NpgsqlCommand comandoEmail = new NpgsqlCommand(sentenciaExisteEmail, conexion);
            NpgsqlCommand comandoDni = new NpgsqlCommand(sentenciaExisteDni, conexion);
            comandoEmail.Parameters.AddWithValue("@semail", correoUsuario);
            comandoDni.Parameters.AddWithValue("@sdni", dniUsuario);
            NpgsqlDataReader lectorEmail = comandoEmail.ExecuteReader();
            NpgsqlDataReader lectorDni = comandoDni.ExecuteReader();
            if (lectorEmail.Read() || lectorDni.Read())
            {
                MessageBox.Show("EL CORREO O EL DNI YA EXISTEN");
                conx.CerrarConexion();
            }

        }
        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Btn_Crear_Cuenta_Click(object sender, RoutedEventArgs e)
        {
            ValidarExistenciaEmailDni();
        }
    }
}
