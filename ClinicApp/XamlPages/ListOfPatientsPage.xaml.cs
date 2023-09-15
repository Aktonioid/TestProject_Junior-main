using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClinicApp.EntityModels;
using System.Threading;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;

namespace ClinicApp.XamlPages
{
    /// <summary>
    /// Interaction logic for ListOfPatientsPage.xaml
    /// </summary>
    public partial class ListOfPatientsPage : Page
    {
        //the main frame which represents all the forms in the app
        private Frame mainFrame;
        //It's using for storing the last card before pages were switched.
        //with help of this variable we can restore the last appearence whithin the constructor
        private PatientCard tempCard;
        //if the application is initializing for the first time, we will notify the user
        //that he/she should wait until the db is initialized
        private Boolean isInitializingFirstTime = true;

        public ListOfPatientsPage(Frame main)
        {
            InitializeComponent();
            mainFrame = main;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeDataGrids();

        }
        //loads DataGrids asynchronously. 
        private async void InitializeDataGrids()
        {
            //the cts is cancelling async animation task after we are done here
            CancellationTokenSource cts = new CancellationTokenSource();
            if (isInitializingFirstTime)
            {
                dgPatients.Visibility = Visibility.Collapsed;
                lblLoading.Visibility = Visibility.Visible;
            }
            try
            {
                Animation(cts.Token);
                ClinicDataRepository clinicRepo = new ClinicDataRepository();
                dgPatients.ItemsSource = (await clinicRepo.GetPatientCards()).Select(p => p.AsView());
                //after adding a new request or after modifying one,
                //we should refresh the DataGrid to see the changes
                if (tempCard != null)
                    dgRequests.ItemsSource = await clinicRepo.GetPatientRequests(tempCard.Id);
            }
            catch (SqlException)
            {
                var err = ConfigurationSettings.AppSettings["dbError"].ToString();
                MessageBox.Show(err, "Ошибка");
                throw;
            }
            catch
            {
                MessageBox.Show("Что то пошло не так, приложение будет закрыто", "Ошибка");
                throw;
            }

            if (isInitializingFirstTime)
            {
                cts.Cancel();
                lblLoading.Content = "";
                lblLoading.Visibility = Visibility.Collapsed;
                dgPatients.Visibility = Visibility.Visible;
                //don't invoke the loading loop next time
                isInitializingFirstTime = false;
            }
        }
        //removes a patient from the db and refreshes ItemsSource of the DataGrid
        private async void btnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить карту пациента и все его обращения?", "Внимание!",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                
                var currentCard = (dgPatients.SelectedItem as PatientView).card;
                if (currentCard != null)
                {
                    Int32 id = currentCard.Id;
                    ClinicDataRepository clinicRepo = new ClinicDataRepository();
                    try
                    {
                        clinicRepo.DeleteCard(id);
                        dgPatients.ItemsSource = (await clinicRepo.GetPatientCards()).Select(p => p.AsView());
                    }
                    catch (SqlException)
                    {
                        var err = ConfigurationSettings.AppSettings["dbError"].ToString();
                        MessageBox.Show(err, "Ошибка");
                        throw;
                    }
                    catch
                    {
                        MessageBox.Show("Что то пошло не так, приложение будет закрыто", "Ошибка");
                        throw;
                    }

                }
            }
        }
        //gets all requests from the current patient and loads it to dgRequests.ItemSource
        private async void btnPatientRequests_Click(object sender, RoutedEventArgs e)
        {
            var currentCard = (dgPatients.SelectedItem as PatientView).card;
            if (currentCard != null)
            {
                //in order to see the requests we are switching DataGrids here
                ToggleGridViews();
                ClinicDataRepository clinicRepo = new ClinicDataRepository();
                try
                {
                    dgRequests.ItemsSource = await clinicRepo.GetPatientRequests(currentCard.Id);
                    lblPatientName.Content = currentCard.Name;
                }
                catch (SqlException)
                {
                    var err = ConfigurationSettings.AppSettings["dbError"].ToString();
                    MessageBox.Show(err, "Ошибка");
                    throw;
                }
                catch
                {
                    MessageBox.Show("Что то пошло не так, приложение будет закрыто", "Ошибка");
                    throw;
                }
            }
        }

        private void btnAddNewRequest_Click(object sender, RoutedEventArgs e)
        {
            var currentCard = (dgPatients.SelectedItem as PatientView).card;
            //if the btnAddNewRequest has been clicked from the patient's card
            //we are placing current card to the temp variable and invoking AddNewRequest 
            if (currentCard != null)
                tempCard = currentCard;
            //if it has been clicked right after AddNewRequest form went back, we can't get
            //current patient's card from dgPatients.SelectedItem variable. Since we are working
            //with the same patien's card we can get it from tempCard variable
            mainFrame.Content = new AddModifyRequest(tempCard, mainFrame);
        }


        //goes to AddModifyPatient page where a user can manage a patient info
        private void btnModifyPatient_Click(object sender, RoutedEventArgs e)
        {
            var currentCard = (dgPatients.SelectedItem as PatientView).card;
            if (currentCard != null)
            {
                mainFrame.Content = new AddModifyPatient(currentCard, mainFrame);
            }
        }
        //goes to AddModifyRequest page where a user can manage a patient request info
        private void btnModifyRequest_Click(object sender, RoutedEventArgs e)
        {
            var currentRequest = dgRequests.SelectedItem as Request;
            if (currentRequest != null)
            {
                tempCard = currentRequest.Patient;
                mainFrame.Content = new AddModifyRequest(tempCard, currentRequest, mainFrame);
            }
        }
        //removes a patient request from the db and refreshes ItemsSource of the DataGrid
        private async void btnDeleteRequest_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить данное обращение?", "Внимание!",
                MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var currentRequest = dgRequests.SelectedItem as Request;
                if (currentRequest != null)
                {
                    try
                    {
                        ClinicDataRepository clinicRepo = new ClinicDataRepository();
                        clinicRepo.DeleteRequest(currentRequest.RequestId);
                        dgRequests.ItemsSource = await clinicRepo.GetPatientRequests(currentRequest.Patient.Id);
                    }
                    catch (SqlException)
                    {
                        var err = ConfigurationSettings.AppSettings["dbError"].ToString();
                        MessageBox.Show(err, "Ошибка");
                        throw;
                    }
                    catch
                    {
                        MessageBox.Show("Что то пошло не так, приложение будет закрыто", "Ошибка");
                        throw;
                    }
                }
            }
        }
        //switching between two datagrids 
        private void ToggleGridViews()
        {
            if (dgPatients.Visibility == Visibility.Visible)
            {
                dgPatients.Visibility = Visibility.Collapsed;
                dgRequests.Visibility = Visibility.Visible;
                btnAddNewRequest.Visibility = Visibility.Visible;
                spToolsForPatientCards.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblPatientName.Content = null;
                dgPatients.Visibility = Visibility.Visible;
                dgRequests.Visibility = Visibility.Collapsed;
                btnAddNewRequest.Visibility = Visibility.Collapsed;
                spToolsForPatientCards.Visibility = Visibility.Visible;
            }
        }
        //Searching patient card by it's name
        private async void txtBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            var txtBox = sender as TextBox;
            ClinicDataRepository clinicRepo = new ClinicDataRepository();
            try
            {
                dgPatients.ItemsSource = (await clinicRepo.GetPatientCardsByName(txtBox.Text)).Select(p => p.AsView());
            }
            catch (SqlException)
            {
                var err = ConfigurationSettings.AppSettings["dbError"].ToString();
                MessageBox.Show(err, "Ошибка");
                throw;
            }
            catch
            {
                MessageBox.Show("Что то пошло не так, приложение будет закрыто", "Ошибка");
                throw;
            }
        }
        //manage the Loading animation
        private void Animation(CancellationToken token)
        {
            Int32 x = 1;
            Task.Run(() =>
            {
                //the loop will stop when db is initialized and DataDrid is populated 
                while (!token.IsCancellationRequested)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        lblLoading.Content = "Подождите, идет загрузка" + new String('.', x++ % 6);
                    });
                    Thread.Sleep(200);
                }
            }, token);
        }

    }
}
