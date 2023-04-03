using CASINO.Clases;
using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
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
        string emailExpresionRegular = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)+)"
                    + @"(?<=[^\.])@(([a-z0-9]+-)?[a-z0-9]+\.)*[a-z]"
                    + @"{2,63}(\.[a-z]{2,})?$";
        string nombreUsuarioExpresionRegular = @"^[^\d]+$";

        string dniUsuarioExpresionRegular = "^[0-9]+$";

        public static bool caritaUser;
        public static bool caritaEmail;
        public static bool caritaPassword;
        public static bool caritaEdad;
        public static bool caritaDni;
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
            }
        }
        /// <summary>
        /// Este método lo que hace es darle el valor a las variables
        /// pone lo que esté en el input en la respectiva variable.
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
        /// <summary>
        /// Inserta los datos de los inputs en la bd
        /// </summary>
        public void Insercion_BaseDatos()
        {
            Inicializar_Inputs();
            List<Jugador> cantJugadores = new List<Jugador>();
            Jugador.AgregarJugador(jugador);
            cantJugadores.Add(jugador);
            if (cantJugadores.Count() > 0)
            {
                MessageBox.Show("Welcome new user " + jugador.Nombre);
            }
            else
            {
                MessageBox.Show("Something went wrong");

            }
        }
        /// <summary>
        /// Convierte las fechas a string y luego se parsea con DateTime
        /// Esto para comparar el año del usuario con el año actual, si es menor a 18
        /// no se permite la inserción
        /// </summary>
        /// <returns></returns>
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
            return restaAnioUsuario_Actual;

        }


        public static bool existenciaEmail = false;
        public static bool existenciaDni = false;

        /// <summary>
        /// Valida si existe el dni, si este existe pone la variable existenciaDni en true.
        /// Si la variable es true no permite hacer la inserción
        /// </summary>
        public void ValidarExistenciaDni()
        {
            dniUsuario = UserDniBox.Text;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaExisteDni = "SELECT dni FROM usuarios WHERE dni=@sdni LIMIT 1";
            NpgsqlCommand comandoDni = new NpgsqlCommand(sentenciaExisteDni, conexion);
            comandoDni.Parameters.AddWithValue("@sdni", dniUsuario);
            NpgsqlDataReader lectorDni = comandoDni.ExecuteReader();
            if (lectorDni.Read())
            {
                conx.CerrarConexion();
                existenciaDni = true;
            }


        }
        /// <summary>
        /// Valida si existe el email, si este existe pone la variable existenciaEmail en true.
        /// Si la variable es true no permite hacer la inserción
        /// </summary>
        public void ValidarExistenciaEmail()
        {
            correoUsuario = UserEmailBox.Text;
            NpgsqlConnection conexion = conx.EstablecerConexion();
            var sentenciaExisteEmail = "SELECT email FROM usuarios WHERE email=@semail LIMIT 1";
            NpgsqlCommand comandoEmail = new NpgsqlCommand(sentenciaExisteEmail, conexion);
            comandoEmail.Parameters.AddWithValue("@semail", correoUsuario);
            NpgsqlDataReader lectorEmail = comandoEmail.ExecuteReader();
            if (lectorEmail.Read())
            {
                conx.CerrarConexion();
                existenciaEmail = true;
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
            if (caritaUser && caritaEmail && caritaPassword && caritaDni)
            {
                Insercion_BaseDatos();
                SignIn signIn = new SignIn();
                signIn.Show();
                Close();
            }
            else
            {
                if (!caritaUser)
                {
                    errorUser.Visibility = Visibility.Visible;
                }
                else
                {
                    errorUser.Visibility = Visibility.Hidden;
                    return;
                }
                if (!caritaEmail)
                {
                    errorEmail.Visibility = Visibility.Visible;
                }
                else
                {
                    errorEmail.Visibility = Visibility.Hidden;

                }
                if (!caritaPassword)
                {
                    errorClave.Visibility = Visibility.Visible;
                }
                else
                {
                    errorClave.Visibility = Visibility.Hidden;

                }
                if (!caritaEdad)
                {
                    errorFecha.Visibility = Visibility.Visible;
                }
                else
                {
                    errorFecha.Visibility = Visibility.Hidden;

                }
                if (!caritaDni)
                {
                    errorDNI.Visibility = Visibility.Visible;
                }
                else
                {
                    errorDNI.Visibility = Visibility.Hidden;

                }
            }

        }
        private void UserNameBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            nombreUsuario = UserNameBox.Text;
            if (Regex.IsMatch(nombreUsuario, nombreUsuarioExpresionRegular) && nombreUsuario.Length >= 4)
            {
                carita_user.IsChecked = true;
                caritaUser = true;
                errorUser.Visibility = Visibility.Hidden;
            }
            else
            {
                carita_user.IsChecked = false;
                caritaUser = false;
                errorUser.Visibility = Visibility.Visible;
            }

        }
        private void UserEmailBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            correoUsuario = UserEmailBox.Text;
            ValidarExistenciaEmail();
            conx.CerrarConexion();
            if (Regex.IsMatch(correoUsuario, emailExpresionRegular) && !existenciaEmail)
            {
                carita_email.IsChecked = true;
                caritaEmail = true;
                errorEmail.Visibility = Visibility.Hidden;

            }
            else
            {
                carita_email.IsChecked = false;
                existenciaEmail = false;
                errorEmail.Visibility = Visibility.Visible;
                caritaEmail = false;

            }
        }
        private void UserPasswordBox_PreviewKeyUp_1(object sender, KeyEventArgs e)
        {
            SecureString claveUsuario = UserPasswordBox.SecurePassword;
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;
            if (claveUsuarioString.Length >= 8)
            {
                carita_password.IsChecked = true;
                caritaPassword = true;
                errorClave.Visibility = Visibility.Hidden;
            }
            else
            {
                carita_password.IsChecked = false;
                caritaPassword = false;
                errorClave.Visibility = Visibility.Visible;
            }
        }

        private void UserDniBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            dniUsuario = UserDniBox.Text;
            ValidarExistenciaDni();
            conx.CerrarConexion();
            if (Regex.IsMatch(dniUsuario, dniUsuarioExpresionRegular) && !existenciaDni && dniUsuario.Length >= 9)
            {
                carita_dni.IsChecked = true;
                caritaDni = true;
                errorDNI.Visibility = Visibility.Hidden;
            }
            else
            {
                carita_dni.IsChecked = false;
                existenciaDni = false;
                errorDNI.Visibility = Visibility.Visible;
                caritaDni = false;

            }

        }

        private void UserBithDate_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void UserBithDate_CalendarClosed(object sender, RoutedEventArgs e)
        {
            var returnConverison = ConversorFechas();

            if (returnConverison >= 18 && returnConverison <= 99)
            {
                carita_fecha.IsChecked = true;
                caritaEdad = true;
                errorFecha.Visibility = Visibility.Hidden;

            }
            else
            {
                carita_fecha.IsChecked = false;
                caritaEdad = false;
                errorFecha.Visibility = Visibility.Visible;
            }
        }
    }
}

