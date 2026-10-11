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

namespace labaa2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show(
                (new BigNumber(1) + new BigNumber(2)) + "\n" +
                (new BigNumber(999) + new BigNumber(1)) + "\n" +
                (new BigNumber(999999) + new BigNumber(1)) + "\n" +
                (new BigNumber(0) + new BigNumber(0)) + "\n" +
                (new BigNumber(12345678) + new BigNumber(87654322)));
        }
    }
}