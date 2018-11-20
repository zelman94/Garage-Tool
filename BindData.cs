using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace GarageTool
{
    class BindData {
        //public SortedDictionary<string, string> BindItems(bool connectionstatus) {

        //    SortedDictionary<string, string> Items = new SortedDictionary<string, string>
        //    {
        //         { "Battery AA", "Battery AA"},
        //         { "Battery AAA", "Battery AAA"},
        //         { "Battery AAAA", "Battery AAAA"},
        //         { "Battery 312", "Battery 312"},
        //         { "Battery 13", "Battery 13"},
        //         { "Battery 10", "Battery 10"},
        //         { "miniRITE", "miniRITE"},
        //         { "miniRITE-T", "miniRITE-T"},
        //         { "BTE", "BTE"}
        //    };
        //    return Items;
        //}

            public List<string> BindLokal()
            {
                    List<string> Lokal = new List<string>
                    {
                        { "P"},
                        { "Desk"},
                        { "S"}
                    };
                 return Lokal;
            }
        public List<string> BindStatus()
        {
            List<string> Status = new List<string>
                    {
                        { "Dead"},
                        { "Ok"},
                        { "TAKEN"},
                        { "FREE"},
                        { ""}
                    };
            return Status;
        }


        public static List<string> ReportTypes()
        {
            List<string> Report = new List<string>
                    {
                        { "All items"},
                        { "item per person"},
                        { "Free items"},
                        { "break items"},
                        { "User items"}
                    };
            return Report;
        }

        public List<string > BindItems(bool connectionstatus)
        {

            List<string> Items = new List<string>
            {
                 { "Battery AA"},
                 { "Battery AAA"},
                 { "Battery AAAA"},
                 { "Battery 675"},
                 { "Battery 312"},
                 { "Battery 13"},
                 { "Battery 10"},
                 { "miniRITE"},
                 { "miniRITE-T"},
                 { "BTE"},
                 { "HI-PRO 2"},
                 { "HI-PRO USB"},
                 { "EXPRESSlink3"},
                 { "REM"}
            };
            return Items;
        }
        public SortedDictionary<string, string> BindId(bool connectionstatus,List<string> Items) {

            SortedDictionary<string, string> Ids = new SortedDictionary<string, string>();
            int ile = Items.Count();
            int i = 0;
            while (i<ile)
            {
                Ids.Add(Items[i],i.ToString());
                i++;
            }
            return Ids;
        }
        public string GetcountItems(string Item, bool connectionstatus, MySqlConnection SQLConnection) {
            string tmp = "";
            if (connectionstatus)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT COUNT(*) FROM `Items` WHERE status = 'FREE' AND name = '{Item}'", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        myReader.Read();
                        tmp =  myReader.GetString(0);
                    }
                    //SQLConnection.Close();

                }
                catch (Exception )
                {
                   // SQLConnection.Close();
                    return tmp;
                }


            }else
                {
       
                MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                
                }
            //SQLConnection.Close();
            return tmp;
        }

        public string getTextForUI(List<string> X) // przygotowuje stringa do wyswietlenia moich itemow 
        {

            var result = String.Join("\n", X.ToArray());
            return result;
        }

        // funkcja pomocnicza to bindItemToreturn sprawdza czy już coś występuje na liscie
        private bool checkInstance(List<string> lista,string name, int max) //zwraca false gdy istnieje juz ta nazwa
        {
            int i = 0;           
            while (i < max)
            {
                try
                {
                    if (lista[i] == name)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    
                }

                i++;
            }
            return true;
        }
        //--------

        public List<string> bindItemToreturn (List<Device> namesItems) // zwraca nazwy
        {
            int i = 0;
            List<string> myItemsnames = new List<string>();
            while (i < namesItems.Count)
            {
                if (checkInstance(myItemsnames, namesItems[i].Name, i)) // zeby nie sprawdzac czy ten sam element istnieje
                {
                    myItemsnames.Add(namesItems[i].Name);
                }
                i++;
            }
            return myItemsnames;

        }

        public List<string> bindID_items_Toreturn(List<Device> IDs, string name) // zwraca ID dla nazwy
        {

            int i = 0;
            List<string> myItemsId = new List<string>();
            while (i < IDs.Count)
            {
                if (IDs[i].Name == name)
                {
                    myItemsId.Add(IDs[i].Id);
                }     
                i++;
            }
            return myItemsId;
        }

    }
}
