using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Runtime.InteropServices;

namespace GarageTool
{
    /// <summary>
    /// Interaction logic for EditItems.xaml
    /// </summary>
    public partial class EditItems : Window
    {




        DataBaseManager dataBaseManagerEdit;
        BindData bindDataEdit;
        Qrclass QrGenarator = new Qrclass();




        public EditItems()
        {
            try
            {
                dataBaseManagerEdit = new DataBaseManager();
                bindDataEdit = new BindData();
                

                InitializeComponent();
                btnValDate.Visibility = Visibility.Hidden;
                Kalendarz.Visibility = Visibility.Hidden;
                lblId.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[2];
                lblItem.Content = ((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[3];

                cmbItem.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).ItemsList;
                cmbName.ItemsSource = ((MainWindow)System.Windows.Application.Current.MainWindow).ItemsList;
                cmbLokal.ItemsSource = bindDataEdit.BindLokal();
                cmbStatus.ItemsSource = bindDataEdit.BindStatus();
                rdbEdit.IsChecked = true;
                this.Width = 600;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

                
                
        }



       
        private string getInfotoQR()
        {

            return $"{cmbName.Text};{txtID.Text};{txtX.Text};{txtY.Text};{cmbLokal.Text};{txtOwner.Text};{cmbStatus.Text}";

        }

        private void Button_Click(object sender, RoutedEventArgs e) //Edit
        {
            if (cmbItem.SelectedIndex != -1 && cmbId.SelectedIndex != -1)
            {
                if (dataBaseManagerEdit.SetEditedItem(cmbItem.Text, cmbId.Text, cmbName.Text, txtID.Text, txtX.Text, txtY.Text, cmbLokal.Text, txtOwner.Text, cmbStatus.Text))
                {
                    Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 150);
                    this.Width = 600;
                    Kalendarz.Visibility = Visibility.Hidden;
                    btnValDate.Visibility = Visibility.Hidden;
                    MessageBox.Show("Done");
                }
                else
                {
                    MessageBox.Show("try again");
                }

                
        
            }
            else
            {
                MessageBox.Show("Select Item and ID (serial number) to Edit");
            }
            
        }

        private bool CheckIfAllFields() // true jezeli wszystko wypełnione
        {




            if (cmbName.Text != "" && cmbLokal.Text != "" && cmbStatus.Text != "" && txtID.Text != "" && txtX.Text != "" && txtY.Text != "" && txtOwner.Text != "") 
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void cmbAdd_Click(object sender, RoutedEventArgs e)//Add
        {
            if (CheckIfAllFields())
            {
               
                if (dataBaseManagerEdit.AddNewItem(cmbName.Text, txtID.Text, txtX.Text, txtY.Text, cmbLokal.Text, txtOwner.Text.ToUpper(), cmbStatus.Text, Kalendarz.SelectedDate.ToString()))
                {
                    Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 200);

                    cmbName.SelectedIndex = -1;
                    txtID.Text = "";
                    txtX.Text = "";
                    txtY.Text = "";
                    cmbLokal.SelectedIndex = -1;
                    txtOwner.Text = "";
                    cmbStatus.SelectedIndex = -1;
                    MessageBox.Show("Done");
                    Qrimage.Source = null;
                    this.Width = 600;
                    Kalendarz.Visibility = Visibility.Hidden;
                    btnValDate.Visibility = Visibility.Hidden;
                }
                else
                {
                    MessageBox.Show("try again");
                }
                
            }
            else
            {
                if(cmbName.SelectedIndex >= 0 && cmbName.SelectedIndex <= 6)
                {
                    int tmp = int.Parse(txtID.Text) * (-1);
                    dataBaseManagerEdit.TakeBattery(cmbName.Text, tmp.ToString());
                }
                else
                MessageBox.Show("Fill all fields to Add");
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbId.Items.Refresh();

            // dodac bindowanie dla edycji 
            Device item = dataBaseManagerEdit.GetItem(cmbItem.Text, cmbId.Text);
            if (item != null)
            {
                cmbName.Text = item.Name;
                txtID.Text = item.Id;
                txtX.Text = item.Position.X.ToString();
                txtY.Text = item.Position.Y.ToString();
                cmbLokal.SelectedItem = item.Lokal;
                txtOwner.Text = item.Owner;
                cmbStatus.SelectedItem = item.Status;
            }
            else
            {
                MessageBox.Show("Error");
            }


            Qrimage.Source = QrGenarator.createQR(getInfotoQR(),50);
        }

        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItem.Items.Refresh();

            if(cmbItem.SelectedIndex >= 0 && cmbItem.SelectedIndex <= 6)
            {
                //txtID.Visibility = Visibility.Hidden;
                txtX.Visibility = Visibility.Hidden;
                txtY.Visibility = Visibility.Hidden;
                cmbLokal.Visibility = Visibility.Hidden;
                txtOwner.Visibility = Visibility.Hidden;
                cmbStatus.Visibility = Visibility.Hidden;
                lblid.Content = "how many :";
                lblX.Visibility = Visibility.Hidden;
                lblY.Visibility = Visibility.Hidden;
                lblOwner.Visibility = Visibility.Hidden;
                btnMy.Visibility = Visibility.Hidden;
                btnSWS.Visibility = Visibility.Hidden;


                cmbId.Visibility = Visibility.Hidden;
                cmbId.IsEditable = false;
            }
            else
            {
                //txtID.Visibility = Visibility.Visible;
                txtX.Visibility = Visibility.Visible;
                txtY.Visibility = Visibility.Visible;
                cmbLokal.Visibility = Visibility.Visible;
                txtOwner.Visibility = Visibility.Visible;
                cmbStatus.Visibility = Visibility.Visible;
                lblid.Content = "ID :";
                lblX.Visibility = Visibility.Visible;
                lblY.Visibility = Visibility.Visible;
                lblOwner.Visibility = Visibility.Visible;
                btnMy.Visibility = Visibility.Visible;
                btnSWS.Visibility = Visibility.Visible;
            }


            cmbId.ItemsSource = dataBaseManagerEdit.GetIdItems(cmbItem.Text,true);
            cmbId.Items.Refresh();
            if (cmbItem.Text == "REM")
            {
                btnValDate.Visibility = Visibility.Visible;
                //Kalendarz.Visibility = Visibility.Visible;
            }
            else
            {
                btnValDate.Visibility = Visibility.Hidden;
                //Kalendarz.Visibility = Visibility.Hidden;
            }

        }

        private void rdbAdd_Checked(object sender, RoutedEventArgs e)
        {
            cmbName.IsEnabled = true;
            txtID.IsEnabled = true;
        }

        private void rdbEdit_Checked(object sender, RoutedEventArgs e)
        {
            cmbName.IsEnabled = false;
            txtID.IsEnabled = false;
        }

        private void cmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbStatus.Items.Refresh();
            Qrimage.Source = QrGenarator.createQR(getInfotoQR(), 200);
        }


        private void btnSaveQR_Click(object sender, RoutedEventArgs e)
        {
            if (Qrimage.Source != null)
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Qrimage.Source));
                string path = Directory.GetCurrentDirectory();
                using (FileStream stream = new FileStream($"{path}/{cmbName.Text}_{txtID.Text}.jpg", FileMode.Create))
                encoder.Save(stream);
            }
            else
            {
                MessageBox.Show("creatte QR code");
            }
        }

        private void cmbLokal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            txtOwner.Text = Environment.UserName.ToUpper();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txtOwner.Text = "SWS";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            ReportWindow win_Report = new ReportWindow();

            win_Report.ShowDialog();


        }

        private void btnValDate_Click(object sender, RoutedEventArgs e)
        {
            this.Width = 750;
            Kalendarz.Visibility = Visibility.Visible;
        }

        private void cmbName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbName.Items.Refresh();

            if (cmbName.SelectedIndex >= 0 && cmbName.SelectedIndex <= 6)
            {
                //txtID.Visibility = Visibility.Hidden;
                txtX.Visibility = Visibility.Hidden;
                txtY.Visibility = Visibility.Hidden;
                cmbLokal.Visibility = Visibility.Hidden;
                txtOwner.Visibility = Visibility.Hidden;
                cmbStatus.Visibility = Visibility.Hidden;
                lblid.Content = "how many :";

                lblX.Visibility = Visibility.Hidden;
                lblY.Visibility = Visibility.Hidden;
                lblOwner.Visibility = Visibility.Hidden;
                btnMy.Visibility = Visibility.Hidden;
                btnSWS.Visibility = Visibility.Hidden;



                cmbId.Visibility = Visibility.Hidden;
                cmbId.IsEditable = false;
            }
            else
            {
               // txtID.Visibility = Visibility.Visible;
                txtX.Visibility = Visibility.Visible;
                txtY.Visibility = Visibility.Visible;
                cmbLokal.Visibility = Visibility.Visible;
                txtOwner.Visibility = Visibility.Visible;
                cmbStatus.Visibility = Visibility.Visible;
                lblid.Content = "ID :";
                lblX.Visibility = Visibility.Visible;
                lblY.Visibility = Visibility.Visible;
                lblOwner.Visibility = Visibility.Visible;
                btnMy.Visibility = Visibility.Visible;
                btnSWS.Visibility = Visibility.Visible;
                cmbId.Visibility = Visibility.Visible;
                cmbId.IsEditable = true;
            }



            if (cmbName.Text == "REM")
            {
                btnValDate.Visibility = Visibility.Visible;
               // Kalendarz.Visibility = Visibility.Visible;
            }
            else
            {
                btnValDate.Visibility = Visibility.Hidden;
                //Kalendarz.Visibility = Visibility.Hidden;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItem.SelectedIndex != -1 && cmbId.SelectedIndex != -1)
            {
                if (dataBaseManagerEdit.DeleteItem(cmbItem.Text, cmbId.Text))
                {
                    MessageBox.Show("done");
                }
                else
                {
                    MessageBox.Show("failed");
                }
            }
            else
            {
                MessageBox.Show("Select Item");
            }
        }
    }
}
