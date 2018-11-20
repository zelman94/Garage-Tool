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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Windows.Threading;

//ZXing.net zainstalowany nugat do QR kodów


namespace GarageTool
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    /// 





    public partial class MainWindow : Window
    {
        DataBaseManager dataBaseManager = new DataBaseManager();
        BindData bindData = new BindData();
        jezyki Jezyki = new jezyki();
        Qrclass QrGenerator = new Qrclass();
        FTP ftp;
        public List<string> Stringi;
        public List<string> ItemsList;
        public string Battery_toRent;
        public string MyIDentyf=Environment.UserName.ToString().ToUpper();
        
        public int count_down, count_down_started;
       public static Identyfikacja Identyfikacji_okno;



        public MainWindow()
        {
            InitializeComponent();

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                System.Windows.MessageBox.Show("Application XXXX already running. Only one instance of this application is allowed");
                this.Close();
            }


            //MyIDentyf = "PAZE";
            txtUser.Text = MyIDentyf;
            imgtmpQR.Source = QrGenerator.createQR(MyIDentyf, 100);
            imgtmpQR.ToolTip = MyIDentyf.ToString().ToUpper();
            ftp = new FTP();


            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            if (!Directory.Exists(@"C:\ProgramData\Garage"))
            {
                Directory.CreateDirectory(@"C:\ProgramData\Garage");
            }

            
            if (!dataBaseManager.DB_connection) //jezeli nie ma polaczenia z BD
            {
                //lblConnectionStatus.Content = "Brak połączenia z Bazą Danych";
            }
            else
            {
               // lblConnectionStatus.Content = "";
            }
            //string[] tmpItems = bindData.BindItems(dataBaseManager.DB_connection);
            // bindowanie z cmbItem

            //bindowanie listy jezyków
            cmbLeng.ItemsSource = Jezyki.bindJezyki();
            cmbLeng.DisplayMemberPath = "Key";
            cmbLeng.SelectedValuePath = "Value";
            cmbLeng.SelectedIndex = 1;
            Stringi = Jezyki.select(cmbLeng.Text.ToString());
            updateStrings();
            //-----------------------------------------

            //bindowanie Itemów


            //cmbItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            //cmbItem.AutoCompleteSource = AutoCompleteSource.ListItems;

            ItemsList = bindData.BindItems(dataBaseManager.DB_connection);
            cmbItem.ItemsSource = ItemsList;
            cmbItemFind.ItemsSource = bindData.BindItems(dataBaseManager.DB_connection);

            //-----------------------------------------
            //bindowanie my items

            txtMyItems.Text = bindData.getTextForUI(dataBaseManager.CheckMyBorrowItems(MyIDentyf));

            //---------------------

            //bindowanie moich itemow do zwrotu                
            cmbItemReturn.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());
            cmbNameTransfer.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());
            //---------------------------------
            Bind_AllAvailableItems();


            //DateTime dateTime = DateTime.UtcNow.Date;
            //var encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgtmpQR.Source));
            //string path = Directory.GetCurrentDirectory();
            //using (FileStream stream = new FileStream($"{path}/{dateTime.ToShortDateString()}.jpg", FileMode.Create))
            //encoder.Save(stream);
            //---- wybieram na poczatek selection mode dla zwrotow i wypozyczen
            rbnSelectmode.IsChecked = true;
            rbnQRmode.IsChecked = false;
            rbnQRmode.IsEnabled = false;
            rbnQRmode.Visibility = Visibility.Hidden;
            rbnSelectmode.Visibility = Visibility.Hidden;
            txtqedatazw.Visibility = Visibility.Hidden;



            if (!ftp.czyAktualnaversia())
            {
                btnUpdate.IsEnabled = true;
                btnUpdate.ToolTip = ftp.checkVersionOnServer();
                try
                {
                    System.Windows.MessageBox.Show("update available");
                    btnUpdate.Visibility = System.Windows.Visibility.Visible;
                }
                catch (Exception x)
                {
                    btnUpdate.Visibility = System.Windows.Visibility.Visible;
                    System.Windows.MessageBox.Show("update available");
                }
            }
            else
            {
                try
                {

                    System.Windows.MessageBox.Show("");
                }
                catch (Exception x)
                {
                    btnUpdate.Visibility = System.Windows.Visibility.Hidden;
                    Console.WriteLine("");
                }
            }
            
        }

        static void OnProcessExit(object sender, EventArgs e)
        {

           // Identyfikacji_okno.End();
          // dataBaseManager.SQLConnection.Close();
        }

        public void BindComboboxes()
        {


        }


        private void cmbItel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }// smiec po UI

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbId.Items.Refresh();
            Device tmp = dataBaseManager.GetItem(cmbItem.Text, cmbId.Text);
            try
            {
                SelectedQRItem.Source = QrGenerator.createQR(tmp.Get_stringfor_QR(tmp), 150);
                btnWyp.FontWeight = FontWeights.Bold;
                btnzwrot.IsEnabled = false;
                if (cmbItem.Text =="REM")
                {
                    btnWyp.ToolTip = "End of Validation " + tmp.Validation;
                }
                else
                {
                    btnWyp.ToolTip = null;
                }
                
            }
            catch (Exception x)
            {
                //System.Windows.Forms.MessageBox.Show("try again");
            }
            if (cmbId.SelectedIndex!= -1)
            {
                lblSelItemQR.Content = "Selected Item:";
            }
            else lblSelItemQR.Content = "";

        } // smiec po UI

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            // Point pos = new Point(1,2);
            try
            {
                if (cmbId.Text!="")
                {
                    count_down = count_down_started;

                    Device item = dataBaseManager.GetItem(cmbItem.Text, cmbId.Text);

                    Map map = new Map(item);
                    map.ShowDialog();
                }
               
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString() + " ALSO\n " + ee.ToString());
            }

        }

        private void cmbLeng_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbLeng.Items.Refresh();
            Stringi = Jezyki.select(cmbLeng.Text.ToString());
            updateStrings();
            if (Stringi!=null)
            {
                Stringi.Clear();
            }
            
        }
        private void updateStrings()
        {
            try
            {
                btnShow.Content = Stringi[0];
                btnWyp.Content = Stringi[1];
                lblId.Content = Stringi[2];
                lblItem.Content = Stringi[3];
                lblLang.Content = Stringi[4];
                TbRent.Header = Stringi[5];
                TbSett.Header = Stringi[6];
                lblAvailableItems.Content = Stringi[7]; //nr 8 to komunikat brak dostepu do BD nr 9 w windows1 labela
                lblUser.Content = Stringi[11];
                lblpass.Content = Stringi[12];
                btnpassconfirm.Content = Stringi[13];
            }
            catch (Exception)
            {


            }



        }

        private void cmbItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItem.Items.Refresh();
            txtAvailableItems.Text = bindData.GetcountItems(cmbItem.Text.ToString(), dataBaseManager.DB_connection, dataBaseManager.SQLConnection);
            
            if (cmbItem.SelectedIndex <= 6 && cmbItem.SelectedIndex != -1)
            {
                Window1 win2 = new Window1(cmbItem.SelectedIndex);
                win2.ShowDialog();
                txtAvailableItems.Text = dataBaseManager.Execute($"Select `sztuki` From `Items` Where `name` ='{cmbItem.Text}'");
                //System.Windows.Forms.MessageBox.Show("battery to Rent : "+ Battery_toRent);

            }
            else
            {
                cmbId.ItemsSource = bindData.BindId(dataBaseManager.DB_connection, dataBaseManager.GetIdItems(cmbItem.Text.ToString(),false));
                cmbId.DisplayMemberPath = "Key";
                cmbId.SelectedValuePath = "Value";
            }



        }

        private void btnpassconfirm_Click(object sender, RoutedEventArgs e)
        {
            if (dataBaseManager.confirmadminuser(txtUser.Text.ToUpper(), txtPass.Password.ToString()))             
            {
                EditItems win_Edit = new EditItems();
                win_Edit.ShowDialog();
                txtUser.Clear();
                txtPass.Clear();
                Refresh_bindedData();
            }
            else
            {
                txtUser.Clear();
                txtPass.Clear();
            }




        }

        private void btnWyp_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItem.SelectedIndex != -1 && cmbId.SelectedIndex != -1)
            {
                //set do bazy danych 
                dataBaseManager.SetHire(imgtmpQR.ToolTip.ToString().ToUpper(), cmbItem.Text,cmbId.Text, DateTime.Today.ToShortDateString());
                Refresh_bindedData();

                cmbItem.SelectedIndex = -1;
                cmbId.SelectedIndex = -1;
                lblSelItemQR.Content = "";
                SelectedQRItem.Source = null;
                count_down = count_down_started;

                txtAvailableItems.Text = "0";

                btnWyp.FontWeight = FontWeights.Normal;
                btnzwrot.IsEnabled = true;
                btnWyp.ToolTip = null;

            }

            if (cmbItem.SelectedIndex >= 0 && cmbItem.SelectedIndex <= 6)
            {
                if(dataBaseManager.TakeBattery(cmbItem.Text, Battery_toRent))
                {
                    System.Windows.MessageBox.Show("Done");
                }
                else
                {
                    System.Windows.MessageBox.Show("Error");
                }
                cmbItem.SelectedIndex = -1;
                
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (cmbItemReturn.SelectedIndex != -1 && cmbIdReturn.SelectedIndex != -1)
            {
                //set do bazy danych 
                dataBaseManager.SetReturnItem(cmbItemReturn.Text,cmbIdReturn.Text, DateTime.Today.ToShortDateString());
                Refresh_bindedData();

                cmbItemReturn.SelectedIndex = -1;
                cmbIdReturn.SelectedIndex = -1;
                lblSelItemQR.Content = "";
                SelectedQRItem.Source = null;

                btnzwrot.FontWeight = FontWeights.Normal;
                btnWyp.IsEnabled = true;
                btnzwrot.ToolTip = null;
            }

        }

        private void cmbItemReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbItemReturn.Items.Refresh();
            //bindowanie moich ID itemow do zwrotu                
            cmbIdReturn.ItemsSource = bindData.bindID_items_Toreturn(dataBaseManager.GetMyRentItemsNames(), cmbItemReturn.Text);
            //---------------------------------
        }

        private void Bind_AllAvailableItems()
        {
            List<string> itemki= new List<string>();

            foreach (var item in ItemsList)
            {
                if (item.Contains("Battery"))
                {

                    itemki.Add(item + ": " + dataBaseManager.Execute($"Select `sztuki` From `Items` where `name` = '{item}' "));
                }
                else
                itemki.Add(item + ": " + dataBaseManager.GetCountItems(item));
            }

            txtAllAvailableItems.Text = bindData.getTextForUI(itemki);

        }

        private void Refresh_bindedData()
        {

            //bindowanie Itemów
            
            ItemsList = bindData.BindItems(dataBaseManager.DB_connection);
            cmbItem.ItemsSource = ItemsList;
            cmbItemReturn.ItemsSource = ItemsList;
            cmbItem.Items.Refresh();
            cmbItemReturn.Items.Refresh();

            //-----------------------------------------
            //bindowanie my items

            txtMyItems.Text = bindData.getTextForUI(dataBaseManager.CheckMyBorrowItems(MyIDentyf));

            //---------------------

            Bind_AllAvailableItems();

           
            cmbItem.ItemsSource = ItemsList;
            //bindowanie moich itemow do zwrotu                
            cmbItemReturn.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());
            cmbNameTransfer.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());

        }

        private void btnsaveQR_Click(object sender, RoutedEventArgs e)
        {
            if (imgtmpQR.Source != null)
            {
                DateTime dateTime = DateTime.UtcNow.Date;
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imgtmpQR.Source));
                string path = Directory.GetCurrentDirectory();
                using (FileStream stream = new FileStream($"{path}/{Environment.UserName.ToUpper()}.jpg", FileMode.Create))
                    encoder.Save(stream);
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("creatte QR code");
            }
        }

        private void cmbIdReturn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbIdReturn.Items.Refresh();
            Device tmp = dataBaseManager.GetItem(cmbItemReturn.Text, cmbIdReturn.Text);
            btnzwrot.FontWeight = FontWeights.Bold;
            btnWyp.IsEnabled = false;

            try
            {
                SelectedQRItem.Source = QrGenerator.createQR(tmp.Get_stringfor_QR(tmp), 150);
                if (cmbItemReturn.Text=="REM")
                {
                    btnzwrot.ToolTip = "End of Validation " + tmp.Validation;
                }
                
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show("try again");
            }
            if (cmbIdReturn.SelectedIndex != -1)
            {
                lblSelItemQR.Content = "Selected Item:";
            }
            else lblSelItemQR.Content = "";
        }

       


        private void Timer_Tick_ReadWypozycz(object sender, EventArgs e)
        {
            if (txtQrReadWypo.Text != "")
            {
                try
                {
                    Device tmp = QrGenerator.GetItemFromstring(txtQrReadWypo.Text);

                    cmbItem.SelectedValue = tmp.Name;
                    cmbId.ItemsSource = bindData.BindId(dataBaseManager.DB_connection, dataBaseManager.GetIdItems(cmbItem.Text.ToString(), false));
                    cmbId.Items.Refresh();
                    cmbId.Text = tmp.Id;
                    txtQrReadWypo.Text = "";
                    
                }
                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(txtQrReadWypo.Text);
                    txtQrReadWypo.Text = "";

                }

            }
        }


        private void Timer_Tick_ReadZwrot(object sender, EventArgs e)
        {
            if (txtqedatazw.Text != "")
            {
                try
                {
                    Device tmp = QrGenerator.GetItemFromstring(txtqedatazw.Text);

                    cmbItemReturn.SelectedValue = tmp.Name;
                    cmbIdReturn.ItemsSource = bindData.bindID_items_Toreturn(dataBaseManager.GetMyRentItemsNames(), cmbItemReturn.Text);
                    cmbIdReturn.Items.Refresh();
                    cmbIdReturn.Text = tmp.Id;
                    txtqedatazw.Text = "";
                    
                }

                catch (Exception)
                {
                    System.Windows.Forms.MessageBox.Show(txtqedatazw.Text);
                    txtQrReadWypo.Text = "";
                  
                }
            }
        }






        private void rbnQRmode_Checked(object sender, RoutedEventArgs e)
        {

            cmbItem.SelectedIndex = -1;
            cmbId.SelectedIndex = -1;
            cmbItemReturn.SelectedIndex = -1;
            cmbIdReturn.SelectedIndex = -1;
            rbnQRmode.IsChecked = true;
            rbnSelectmode.IsChecked = false;
            txtQRtoreadandfind.Focus();
        }

        private void rbnSelectmode_Checked(object sender, RoutedEventArgs e)
        {
            rbnQRmode.IsChecked = false;
            rbnSelectmode.IsChecked = true;

        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            ftp.DownloadFTPFiles("ftp://garage%2540zadanko-z-zutu.cba.pl@zadanko-z-zutu.cba.pl/Garage/Garage.rar", "garage@zadanko-z-zutu.cba.pl", "Santiego94");
        }



        private void Timer_Tick_Identyfi(object sender, EventArgs e)
        {
            if (txtqedatazw.Text != "")
            {
                imgtmpQR.Source = QrGenerator.createQR(txtqedatazw.Text.ToUpper(), 100);
                imgtmpQR.ToolTip = txtqedatazw.Text.ToUpper();
                MyIDentyf = txtqedatazw.Text;
                txtqedatazw.Text = "";
            }
        }


        private void Timer_Tick_test(object sender, EventArgs e)
        {
            Device item_fromDB=null;
            if (txtQRtoreadandfind.Text != "")
            {
                Device tmp = QrGenerator.GetItemFromstring(txtQRtoreadandfind.Text);
                
                try
                {
                     item_fromDB = dataBaseManager.GetItem(tmp.Name, tmp.Id);
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("Check your QR code");
                }

                try
                {
                    if (item_fromDB.Owner == "SWS")
                    {
                        cmbItem.Text = item_fromDB.Name;
                        cmbId.Text = item_fromDB.Id;
                    }
                    else if (item_fromDB.Owner == MyIDentyf)
                    {
                        cmbItemReturn.Text = item_fromDB.Name;
                        cmbIdReturn.Text = item_fromDB.Id;
                    }
                    txtQRtoreadandfind.Text = "";
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show("error");
                    txtQRtoreadandfind.Text = "";

                }
                

            }
        }


       


        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath); // to start new instance of application
            this.Close(); //to turn off current app
        }

        private void cmbIdTransfer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cmbNameTransfer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbNameTransfer.Items.Refresh();
            cmbIdTransfer.ItemsSource = bindData.bindID_items_Toreturn(dataBaseManager.GetMyRentItemsNames(), cmbNameTransfer.Text);
            cmbIdTransfer.Items.Refresh();
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            if (cmbNameTransfer.Text=="" || cmbIdTransfer.Text=="" || TxtTransfer.Text.Length != 4)
            {
                System.Windows.MessageBox.Show("Fill all fields");
            }
            else
            {
                // tutaj wpisać kod na transfer 

                dataBaseManager.Transfer(cmbNameTransfer.Text,cmbIdTransfer.Text,TxtTransfer.Text.ToUpper());

                cmbNameTransfer.SelectedIndex = -1;
                cmbIdTransfer.SelectedIndex = -1;
                TxtTransfer.Text = "";
                cmbNameTransfer.ItemsSource = bindData.bindItemToreturn(dataBaseManager.GetMyRentItemsNames());
            }
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (cmbItemFind.SelectedIndex != -1)
            {
                if (cmbIdFind.SelectedIndex != -1)
                { // tutaj wpisać kod na wyszukupianie po ID 
                    Device tmp = dataBaseManager.GetItem(cmbItemFind.Text, cmbIdFind.Text);
                    try
                    {
                        System.Windows.MessageBox.Show(tmp.Owner);
                    }
                    catch (Exception)
                    {
                        System.Windows.MessageBox.Show("Error");
                    }
                    

                }
                else
                {
                    System.Windows.MessageBox.Show(bindData.getTextForUI(dataBaseManager.GetOwnersItem(cmbItemFind.Text)));
                    // tutaj wpisać wyszukiwanie tylko po nazwie 
                }
                cmbItemFind.SelectedIndex = -1;
                cmbIdFind.SelectedIndex = -1;
            }
            else
            {
                System.Windows.MessageBox.Show("please select Item");
            }
        }



        private void cmbItemFind_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // bindowanie ID 
            cmbItemFind.Items.Refresh();
            cmbIdFind.ItemsSource = dataBaseManager.GetIdItems(cmbItemFind.Text,true);
            cmbIdFind.Items.Refresh();
        }

        private void btnRefreshAvailableItems_Click(object sender, RoutedEventArgs e)
        {
            Bind_AllAvailableItems();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow win_Report = new ReportWindow();
            win_Report.ShowDialog();
        }

        private void btnIdentyfi_Click(object sender, RoutedEventArgs e)
        {
            imgtmpQR.Source = null;
            imgtmpQR.ToolTip = MyIDentyf;
       
            txtqedatazw.Focus();
            MyIDentyf = txtqedatazw.Text;
        }



    }
}
