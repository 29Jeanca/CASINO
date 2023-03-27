using CASINO.Clases.BaseDatos;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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
        //Ambas variables son para desactivar el label del error
        public bool error1 = false;
        public bool error2 = false;
        //Instancia para la conexión a la base de datos
        public string nombreUsuario = "";
        public string claveUsuario = "";
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

        }
        /// <summary>
        /// Validacion de credenciales, se selecciona únicamente el nombre y la clave para validarlos
        /// Luego de validados pasan a la ventana de Game
        /// </summary>
        public void Validacion_credenciales()
        {
            //MessageBox.Show("Entra a la validación");
            NpgsqlConnection conexion = conx.establecerConexion();
            nombreUsuario = UserNameBox.Text;
            claveUsuario = UserPasswordBox.Text;
            var claveEncriptada = EncriptarClave.Encript(claveUsuario);
            var sentenciaExixteUsuario = "SELECT nombre,clave FROM usuarios WHERE nombre= @susuario AND clave = @sclave";
            NpgsqlCommand comando = new NpgsqlCommand(sentenciaExixteUsuario, conexion);
            comando.Parameters.AddWithValue("@susuario", nombreUsuario);
            comando.Parameters.AddWithValue("@sclave", claveUsuario);
            NpgsqlDataReader lector = comando.ExecuteReader();
            if (lector.Read())
            {
                MessageBox.Show("Funciona");
            }
            else
            {
                MessageBox.Show(nombreUsuario);
                MessageBox.Show(claveUsuario);

            }

        }

        private void Btn_Sign_Click(object sender, RoutedEventArgs e)
        {
            Validacion_credenciales();
        }
    }
}
