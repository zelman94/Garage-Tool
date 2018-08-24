using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GarageTool
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        DataBaseManager dataBaseManagerReport = new DataBaseManager();
        string order_by = "";
        string group_by = "";
        public ReportWindow()
        {
            InitializeComponent();
            bindCmb();
            SetStartUI();
        }

        private void bindCmb()
        {
            cmbTypeReport.ItemsSource = BindData.ReportTypes();
        }

        private void SetStartUI()
        {
            cmbTypeReport.SelectedIndex = 0;
            txtNameReport.Text = cmbTypeReport.Text + "_" + DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void cmbTypeReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbTypeReport.Items.Refresh();
            txtNameReport.Text = cmbTypeReport.Text + "_" + DateTime.Now.ToString("yyyy-MM-dd");

            if (cmbTypeReport.SelectedIndex == 0) // All items
            {
                rbnName.IsEnabled = true;
                rbnOwner.IsEnabled = true;
                rbnStatus.IsEnabled = true;
                rbnName.IsChecked = true;
                rbnasc.IsChecked = true;
            }

            if (cmbTypeReport.SelectedIndex == 1) // item per person
            {
                rbnName.IsEnabled = false;
                rbnOwner.IsEnabled = false;
                rbnStatus.IsEnabled = false;
                rbnOwner.IsChecked = true;
                rbnasc.IsChecked = true;
            }

            if (cmbTypeReport.SelectedIndex == 2)  //Free items
            {
                rbnName.IsEnabled = true;
                rbnOwner.IsEnabled = false;
                rbnStatus.IsEnabled = false;
                rbnName.IsChecked = true;
                rbnasc.IsChecked = true;
            }

            if (cmbTypeReport.SelectedIndex == 3)  //break items
            {
                rbnName.IsEnabled = true;
                rbnOwner.IsEnabled = true;
                rbnStatus.IsEnabled = true;
                rbnName.IsChecked = true;
                rbnasc.IsChecked = true;
            }

            if (cmbTypeReport.SelectedIndex == 4)  //User items
            {
                rbnName.IsEnabled = true;
                rbnOwner.IsEnabled = true;
                rbnStatus.IsEnabled = true;
                rbnOwner.IsChecked = true;
                rbnasc.IsChecked = true;
                txtUserName.IsEnabled = true;
            }
            else
            {
                txtUserName.IsEnabled = false;
            }
        }
            private void generate(string fileName, string SQL, PdfPTable Table)
        {
            List<List<string>> Lista_itemow = new List<List<string>>();


            System.Windows.Forms.SaveFileDialog saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog1.Filter = "PDF File|*.pdf";
            saveFileDialog1.Title = "Save Report File";

            saveFileDialog1.FileName = fileName;
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != fileName)
            {
                fileName = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }


                FileStream fs;
            try
            {
                fs = new FileStream(fileName , FileMode.Create, FileAccess.Write, FileShare.None);
            }
            catch (IOException x)
            {
                MessageBox.Show("Report is already generated");
                return;
            }
            catch (Exception y)
            {
                return;
            }
            Document doc = new Document();
            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());

            PdfWriter writer = PdfWriter.GetInstance(doc, fs);
            doc.Open();

            //dodać wybieranie miejsca na zapis pliku 
           
            doc.Add(Table);
            
                try
                {

                    Lista_itemow = (dataBaseManagerReport.GetStock(SQL)); // lista obiektow do raportu

                    for (int i = 0; i < Lista_itemow.Count; i++)
                    {
                        PdfPTable table = new PdfPTable(Table.NumberOfColumns);
                    bool FLAG = false; // flaga na pojawienie się battery w cell
                    string sztuki = "";
                    foreach (var cell in Lista_itemow[i])
                    {

                        if (FLAG)
                        {


                            table.AddCell(cell + " / " + sztuki );
                            FLAG = false;
                            sztuki = "0";
                        }
                        else
                        {
                            if (cell.Contains("Battery"))
                            {
                                table.AddCell(cell);
                                FLAG = true;
                                sztuki = dataBaseManagerReport.Execute($"SELECT `sztuki` FROM `Items` Where `name` = '{cell}'");
                            }
                            else // jezeli cos innego to : 
                            { 
                                    table.AddCell(cell);
                            }
                        }
                            
                        }
                        doc.Add(new PdfPTable(table));

                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("please try again, during generate report was connection error");
                    // return;
                }
            

            try
            {
                doc.Close();
                MessageBox.Show("done");
            }
            catch (Exception x)
            {


            }


        }

        private void btnGenerateReport_Click(object sender, RoutedEventArgs e)
        {
            string SQL = "";
            PdfPTable table;
            cmbTypeReport.Items.Refresh();
            switch (cmbTypeReport.SelectedIndex)
            {
               
                case 0:
                    SQL = $"SELECT `name`,`id`,`x`,`y`,`lokalizacja`, `owner`,`status`,`Validacja`  FROM `Items` ORDER BY `{group_by}` {order_by}";
                    table = new PdfPTable(8);
                    table.AddCell("name");
                    table.AddCell("id / count");
                    table.AddCell("X");
                    table.AddCell("Y");
                    table.AddCell("Local");
                    table.AddCell("Owner");
                    table.AddCell("Status");
                    table.AddCell("Validacja");
                    generate(txtNameReport.Text,SQL,table);
                    break;
                case 1:    //item per person
                    SQL = $"SELECT `name`,`id`,`owner`,`status`,`sztuki` FROM `Items` ORDER BY `{group_by}` {order_by}";
                    table = new PdfPTable(4);
                    table.AddCell("name");
                    table.AddCell("id / count");
                    table.AddCell("Owner");                   
                    table.AddCell("Status");
                    generate(txtNameReport.Text, SQL, table);
                    break;
                case 2:
                    SQL = $"SELECT `name`,`id`,`lokalizacja`, `sztuki` FROM `Items` WHERE `status` = 'FREE' ORDER BY `{group_by}` {order_by}";
                    table = new PdfPTable(3); // all free
                    table.AddCell("name");
                    table.AddCell("id / count");
                    table.AddCell("Local");
                    //table.AddCell("Status");
                    generate(txtNameReport.Text, SQL, table);
                    break;
                case 3:
                    SQL = $"SELECT `name`,`id / count`,`owner`, `lokalizacja`,`sztuki` FROM `Items` WHERE `status` = 'Dead' ORDER BY `{group_by}` {order_by}";
                    table = new PdfPTable(4); // break items
                    table.AddCell("name");
                    table.AddCell("id");
                    table.AddCell("Owner");
                    table.AddCell("Local");
                    generate(txtNameReport.Text, SQL, table);
                    break;
                case 4:
                    SQL = $"SELECT `name`,`id / count`,`status`,`sztuki` FROM `Items` WHERE `owner` = '{txtUserName.Text}' ORDER BY `{group_by}` {order_by}";
                    table = new PdfPTable(3); //User items
                    table.AddCell("name");
                    table.AddCell("id");
                    table.AddCell("Status");
                    generate(txtNameReport.Text, SQL, table);
                    break;

                default:
                    break;
            }
            
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            order_by = "DESC";
        }

        private void rbnasc_Checked(object sender, RoutedEventArgs e)
        {
            order_by = "";
        }

        private void rbnName_Checked(object sender, RoutedEventArgs e)
        {
            group_by = "name";
        }

        private void rbnOwner_Checked(object sender, RoutedEventArgs e)
        {
            group_by = "owner";
        }

        private void rbnStatus_Checked(object sender, RoutedEventArgs e)
        {
            group_by = "status";
        }
    }
}
