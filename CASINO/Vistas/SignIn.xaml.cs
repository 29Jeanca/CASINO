using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
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
    /// Lógica de interacción para SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        //Instancia para la conexión a la base de datos
        public string correoUsuario = "";
        public string claveUsuario = "";
        public bool validacion;
        public ConexionBD conx = new ConexionBD();
        public SignIn()
        {
            InitializeComponent();
        }

        private void btn_minimizar_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btn_cerrar_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        public bool Validacion_espacios_vacios()
        {
            correoUsuario = UserEmailBox.Text;
            SecureString claveUsuario = UserPasswordBox.SecurePassword;
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;
            //Acá lo convierte en un string, para poder leer la longitud


            if (string.IsNullOrEmpty(correoUsuario))
            {
                error1.Visibility = Visibility.Visible;
                return true;
            }
            if (string.IsNullOrEmpty(claveUsuarioString))
            {
                error2.Visibility = Visibility.Visible;
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Validacion de credenciales, se selecciona únicamente el correo y la clave para validarlos
        /// Luego de validados pasan a la ventana de Game
        /// </summary>
        public bool Validacion_credenciales()
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            correoUsuario = UserEmailBox.Text;
            SecureString claveUsuario = UserPasswordBox.SecurePassword;//Esto saca el contenido del input de contraseña
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;//Acá lo convierte en un string, para luego poder encriptarlo
            var claveEncriptada = EncriptarClave.Encript(claveUsuarioString);
            var sentenciaExixteUsuario = "SELECT email,clave FROM usuarios WHERE email= @semail AND clave = @sclave";
            NpgsqlCommand comando = new NpgsqlCommand(sentenciaExixteUsuario, conexion);
            comando.Parameters.AddWithValue("@semail", correoUsuario);
            comando.Parameters.AddWithValue("@sclave", claveEncriptada);
            NpgsqlDataReader lector = comando.ExecuteReader();



            if (lector.Read())
            {
                MessageBox.Show("Welcome " + correoUsuario);
                conx.CerrarConexion();
                return true;
            }
            else
            {
                error1.Text = "Something went wrong check your credentials";
                error2.Text = "Something went wrong check your credentials";
                error1.Visibility = Visibility.Visible;
                error2.Visibility = Visibility.Visible;
                conx.CerrarConexion();
                return false;
            }


        }

        private void Btn_Sign_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                if (Validacion_espacios_vacios())
                {
                    return;
                }
                if (Validacion_credenciales())

                {
                    Game game = new Game();
                    game.nombreUsuario.Text = correoUsuario;
                    game.Show();
                    Close();
                    return;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                throw;
            }


        }

        private void Btn_Create_Account_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            Close();
            createAccount.Show();
        }

        private void UserNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            error1.Visibility = Visibility.Hidden;
            error2.Visibility = Visibility.Hidden;
        }
    }
}