using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExploradorArchivos.Core.Modelos;
using ExploradorArchivos.ViewModels;

namespace ExploradorArchivos.UI.Vistas
{
    /// <summary>
    /// Interaction logic for VentanaPrincipal.xaml
    /// </summary>
    public partial class VentanaPrincipal : Window
    {
        public VentanaPrincipal()
        {
            InitializeComponent();
        }

        private void Fila_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow fila &&
                fila.Item is ElementoArchivo elemento &&
                elemento.EsCarpeta &&
                DataContext is VentanaPrincipalViewModel viewModel)
            {
                viewModel.CargarCarpeta(elemento.RutaAbsoluta);
            }
        }
    }
}