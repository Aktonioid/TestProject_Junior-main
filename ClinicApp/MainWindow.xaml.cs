using System.Windows;
using ClinicApp.XamlPages;
using System.Threading;

namespace ClinicApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new ListOfPatientsPage(mainFrame);
        }

        private void btnAddNewPatient_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new AddModifyPatient();
        }
        private void btnListOfPatients_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Content = new ListOfPatientsPage(mainFrame);
        }

    }
}
