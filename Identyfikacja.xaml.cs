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
using System.Windows.Threading;

namespace GarageTool
{
    /// <summary>
    /// Interaction logic for Identyfikacja.xaml
    /// </summary>
    public partial class Identyfikacja : Window
    {
        DispatcherTimer Timer_identyfikacja;
        public Identyfikacja()
        {
            InitializeComponent();

            txttmpID.Focus();
            Timer_identyfikacja = new DispatcherTimer();
            Timer_identyfikacja.Tick += dispatcherTimer_Tick;
            Timer_identyfikacja.Interval = new TimeSpan(0, 0, 1);
            Timer_identyfikacja.Start();


        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (txttmpID.Text != "" && txttmpID.Text.Length == 4)
            {

                txtIdentyfikacja.Text = txttmpID.Text;
                txttmpID.Text = "";
                Timer_identyfikacja.Stop();
                ((MainWindow)System.Windows.Application.Current.MainWindow).MyIDentyf = txtIdentyfikacja.Text;
                this.Close();
            }
            else
            {
                txttmpID.Text = "";
            }
        }
        
        public void End(){

            Timer_identyfikacja.Stop();
        }

        private void txttmpID_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
