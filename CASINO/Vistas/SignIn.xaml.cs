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
        public string nombreUsuario = "";
        public string claveUsuario = "";
        public bool validacion = false;
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
        public void Validacion_espacios_vacios()
        {
            nombreUsuario = UserNameBox.Text;
            SecureString claveUsuario = UserPasswordBox.SecurePassword;
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;
            //Acá lo convierte en un string, para poder leer la longitud


            if (nombreUsuario.Length <= 0 || claveUsuarioString.Length <= 0)
            {
                error1.Visibility = Visibility.Visible;
                error2.Visibility = Visibility.Visible;
                validacion = true;
            }

        }
        /// <summary>
        /// Validacion de credenciales, se selecciona únicamente el nombre y la clave para validarlos
        /// Luego de validados pasan a la ventana de Game
        /// </summary>
        public void Validacion_credenciales()
        {
            NpgsqlConnection conexion = conx.EstablecerConexion();
            nombreUsuario = UserNameBox.Text;
            SecureString claveUsuario = UserPasswordBox.SecurePassword;//Esto saca el contenido del input de contraseña
            string claveUsuarioString = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;//Acá lo convierte en un string, para luego poder encriptarlo
            var claveEncriptada = EncriptarClave.Encript(claveUsuarioString);
            var sentenciaExixteUsuario = "SELECT nombre,clave FROM usuarios WHERE nombre= @susuario AND clave = @sclave";
            NpgsqlCommand comando = new NpgsqlCommand(sentenciaExixteUsuario, conexion);
            comando.Parameters.AddWithValue("@susuario", nombreUsuario);
            comando.Parameters.AddWithValue("@sclave", claveEncriptada);
            NpgsqlDataReader lector = comando.ExecuteReader();

            try
            {

                if (lector.Read())
                {
                    MessageBox.Show("Welcome " + nombreUsuario);
                    conx.CerrarConexion();
                }
                else
                {
                    MessageBox.Show("Something went wrong" + "\n" + "Check your credentials");
                    error1.Visibility = Visibility.Visible;
                    error2.Visibility = Visibility.Visible;
                    conx.CerrarConexion();

                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Something went wrong " + e.ToString());
                conx.CerrarConexion();
            }

        }

        private void Btn_Sign_Click(object sender, RoutedEventArgs e)
        {
            if (validacion)
            {
                Validacion_espacios_vacios();
            }
            if (validacion == false)
            {

                Validacion_credenciales();
            }
        }

        private void Btn_Create_Account_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            Close();
            createAccount.Show();
        }
    }
}
