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
        public static string dni;
        public static DateTime fechaActual = DateTime.Now;
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
        public void ConversorFechas()
        {
            var fechaActualString = fechaActual.ToString("dd/MM/yyy");
            DateTime? fechaCumpleUsuario = UserBithDate.SelectedDate;
            var fechaCumpleUsuarioString = fechaCumpleUsuario.ToString();
            DateTime fecha = DateTime.Parse(fechaCumpleUsuarioString);
            int anio = fecha.Year;
            MessageBox.Show(anio + "");
        }

        public bool ValidacionInputs()
        {
            nombreUsuario = UserNameBox.Text;
            correoUsuario = UserEmailBox.Text;
            dni = UserDniBox.Text;
            claveUsuario = new System.Net.NetworkCredential(string.Empty, claveUsuario).Password;


            return true;
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
            ConversorFechas();
        }
    }
}
