using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql;
using System.Diagnostics;
using System.Windows;
using System.IO;

namespace GarageTool
{
    class DataBaseManager
    {
        /// <summary>
        /// Matko bosko... w tej klasie jest taki syf, że o Jezus.
        /// </summary>
        public bool DB_connection; //jezeli jest polaczenie z BD 
        public MySqlConnection SQLConnection;
        private Stopwatch time;

        public DataBaseManager()
        {
            SQLConnection = ConnectToDB();
            
        }

        private MySqlConnection ConnectToDB()
        {
            try
            {
                string tmp = "server=10.128.64.19;" +
                                    "database=Garage;" +
                                   "uid=changer;" +
                                   "password=changer;SslMode=none;Connection Timeout=5";


                tmp = "server=zadanko-z-zutu.cba.pl;" +
                                   "database=zelman_2;" +
                                  "uid=zelman;" +
                                  "password=Santiego94;SslMode=none;";

                // string tmp = "Server=localhost;"
                //+ "Database=Garage;uid = root;password = 1234;SslMode =none;";

                MySqlConnection sqlConn = new MySqlConnection(tmp);
                sqlConn.Open();
               // sqlConn.Close();
                DB_connection = true;
                return sqlConn;

            }

            catch (Exception e)
            {
                MessageBox.Show("Wystąpił nieoczekiwany błąd podczas polaczenia z BD!");
                Console.WriteLine(e.Message);
                DB_connection = false;
                return null;
            }

        }

        public string GetCountItems(string name) {
            string count ="";


            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                   // SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT Count(*) FROM `Items` WHERE `owner` = 'SWS' AND `name` = '{name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            count = myReader.GetString(0);
                        }

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                  
                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");                    
                }
            }

            return count;
        }
        public List<string> GetIdItems(string item, bool AllorNO) {
            List<string> AllAvailableId = new List<string>();
            if (DB_connection)
            {
                string zapytanie = "";
                if (AllorNO) // jezeli true to wszystkie
                {
                    zapytanie = $"SELECT id FROM `Items` WHERE name = '{item}'";
                }
                else
                {
                    zapytanie = $"SELECT id FROM `Items` WHERE status = 'FREE' AND name = '{item}'";
                }


                MySqlDataReader myReader;
                try
                {
                   // SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(zapytanie, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            AllAvailableId.Add(myReader.GetString(0));
                        }

                    }
                }
                catch (Exception )
                {
                }
            }
            else
            {
                //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
            }
            
            return AllAvailableId;
        }
        public void SetHire(string user, string item, string id,string when) { // wysylamy aktualizacje do BD że Item o id jest wypoyczony przez usera


            if (DB_connection)
            {


                string wyszukaj_item = $"SELECT * FROM `Items` WHERE `name` = '{item}' AND `id` = '{id}'";


                MySqlDataReader myReader;
                MySqlDataReader myReader2;
                try
                {
                    try
                    {
                        SQLConnection.Open();
                    }
                    catch (Exception)
                    {
                        //juz otwarte i fajnie
                    }
                    
                    using (MySqlCommand myCommand = new MySqlCommand(wyszukaj_item, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Read();

                        string edytuj = $"UPDATE `Items` SET `name`='{myReader.GetString(0)}',`id`='{myReader.GetString(1)}',`x`='{myReader.GetString(2)}',`y`='{myReader.GetString(3)}',`lokalizacja`='{myReader.GetString(4)}',`owner`='{user}',`status`='TAKEN' WHERE `name` = '{item}' AND `id` = '{id}'";
                        myReader.Close();
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader2 = myCommand2.ExecuteReader(); //ustawienie wypozyczenie
                        }
                    }
                }
                catch (Exception )
                {

                }
            }
            else
            {

            }



        }
        public bool SetReturnItem(string item, string id, string when) { // wysylamy aktualizacje do BD że Item o id wrocil


            if (DB_connection)
            {
                    string wyszukaj_item = $"SELECT * FROM `Items` WHERE `name` = '{item}' AND `id` = '{id}'";


                MySqlDataReader myReader;
                MySqlDataReader myReader2;
                try
                {
                    try
                    {
                        SQLConnection.Open();
                    }
                    catch (Exception)
                    {
                       // jak ju otwarte to super
                    }

                    
                    using (MySqlCommand myCommand = new MySqlCommand(wyszukaj_item, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        myReader.Read();

                        string edytuj = $"UPDATE `Items` SET `name`='{myReader.GetString(0)}',`id`='{myReader.GetString(1)}',`x`='{myReader.GetString(2)}',`y`='{myReader.GetString(3)}',`lokalizacja`='{myReader.GetString(4)}',`owner`='SWS',`status`='FREE' WHERE `name` = '{item}' AND `id` = '{id}'";
                        myReader.Close();
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader2 = myCommand2.ExecuteReader(); //ustawienie zwrotu
                        }        
                    }
   
                }
                catch (Exception )
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                    SQLConnection.Close();
                }
            }
            else
            {
               // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
              
            }

            return false;
        }

        public List<List<string>> GetStock(string SQL) { // pobieramy ile czego mamy i dajemy do raportu

            List<List<string>> item = new List<List<string>>();
            List<string> tmp = new List<string>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    using (MySqlCommand myCommand = new MySqlCommand(SQL, SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {

                            //Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));

                          
                                for (int i = 0; i < myReader.FieldCount; i++)
                                 {
                                    try
                                    {
                                       tmp.Add(myReader.GetString(i));
                                    }
                                    catch (FormatException x)
                                    {
                                        MessageBox.Show(x.ToString());

                                    }
                                    catch (Exception x)
                                    {
                                        tmp.Add(" ");
                                    }
                                }
                                                           
                            item.Add(new List<string>(tmp));
                            tmp.Clear();
                        }
                        return item;

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                    //closeConnection();
                }
            }
            else
            {
                try
                {
                    MessageBox.Show("Connect to DB");
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                    // closeConnection();
                }

            }
            return null;


        }

        public string Execute(string pytaj)
        {
            string zwrotny = "0";


            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    // SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand(pytaj , SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            zwrotny = myReader.GetString(0);
                        }

                    }
                }
                catch (Exception x)
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);

                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                }
            }

            return zwrotny;
        }

        public List<string> CheckMyBorrowItems( string name) { 

            List<string> MyItems = new List<string>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `name`,`id` FROM `Items` WHERE `owner` = '{name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            MyItems.Add(myReader.GetString(0) + ", Id: " + myReader.GetString(1));
                        }

                    }
                }
                catch (Exception x)
                {
                    try
                    {
                        MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);

                    }
                    catch (Exception)
                    {

                    }
                    // SQLConnection.Close();
                }
            }
            else
            {
                try
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");

                }
                
            }

            return MyItems;

        }

        public bool confirmadminuser(string user,string pass)
        {
            bool confirmation = false;
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT Count(*) FROM `admin` WHERE `user` = '{user}' AND `pass` = '{pass}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        myReader.Read();
                        
                         if (myReader.GetString(0) != "0")
                         {
                            confirmation = true;
                         }       
                        
                    }

                }
                catch (Exception x)
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);

                }
            }
            else
            {
                try
                {
                    MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #2");

                }
               
            }

            try
            {
                //SQLConnection.Close();
            }
            catch (Exception)
            {

                Console.WriteLine("Cannot Close Connection to DB");

            }

            return confirmation;

        }

        public bool SetEditedItem(string OldName, string Old_ID, string Name, string ID, string X, string Y, string Lokal, string Owner, string Status)
        {
            // set do BD

            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                 

                        string edytuj = $"UPDATE `Items` SET `name`='{Name}',`id`='{ID}',`x`='{X}',`y`='{Y}',`lokalizacja`='{Lokal}',`owner`='{Owner}',`status`='{Status}' WHERE `name` = '{OldName}' AND `id` = '{Old_ID}'";
                      
                        using (MySqlCommand myCommand2 = new MySqlCommand(edytuj, SQLConnection))
                        {
                            myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                        }

                    return true;
                }
                catch (Exception )
                {

                    return false;
                }
            }
            else
            {

                return false;
            }

            //closeConnection();



        }

        public bool DeleteItem(string name, string id)
        {
            string SQL = "";
            SQL =$"DELETE FROM `Items` WHERE `name` = '{name}' AND `id` = '{id}'";

            try
            {
                using (MySqlCommand myCommand2 = new MySqlCommand(SQL, SQLConnection))
                {
                    MySqlDataReader myReader;
                    myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool AddNewItem(string Name, string ID, string X, string Y, string Lokal, string Owner, string Status, string validation = "")
        {
            // set do BD

            if (DB_connection)
            {


                MySqlDataReader myReader;
                try
                {
                    string dodaj;
                    //SQLConnection.Open();
                    if (validation !="")
                    {
                        dodaj = $"INSERT INTO `Items` VALUES ('{Name}','{ID}','{X}','{Y}','{Lokal}','{Owner}','{Status}','{validation}')";
                    }
                    else
                    {
                        dodaj = $"INSERT INTO `Items` VALUES ('{Name}','{ID}','{X}','{Y}','{Lokal}','{Owner}','{Status}')";
                    }

                    

                    using (MySqlCommand myCommand2 = new MySqlCommand(dodaj, SQLConnection))
                    {
                        myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                    }

                    return true;
                }
                catch (Exception )
                {

                    return false;
                }
            }
            else
            {

                return false;
            }
            
        }
       
        public List<Device> GetMyRentItemsNames()
        {
            List<Device> MyItems = new List<Device>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `name`,`id`,`X`,`Y` FROM `Items` WHERE `owner` = '{((MainWindow)System.Windows.Application.Current.MainWindow).MyIDentyf}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {                  
                            Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));
                            Device tmp = new Device(myReader.GetString(0), myReader.GetString(1),tmp_pkt);
                            MyItems.Add(tmp);
                        }

                    }
                }
                catch (Exception x)
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                }
            }
            else
            {
                try
                {
                   // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                }
            }
            return MyItems;
        }

        public List<String> GetOwnersItem(string Name)
        {
            List < String > owners= new List<String>();

            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `owner` From `Items` WHERE `name` = '{Name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {

                            owners.Add(myReader.GetString(0));
                        }

                    }
                }
                catch (Exception x)
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                }
            }
            else
            {
                try
                {
                    // MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                }
            }


            return owners;
        }
  


        public bool TakeBattery(string Name, string ile)
        {
            if (DB_connection)
            {
                string dostepnaIlosc="0";

                MySqlDataReader myReader;
                try
                {
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT `sztuki` From `Items` WHERE `name` = '{Name}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        while(myReader.Read())
                        dostepnaIlosc = myReader.GetString(0);                       

                    }

                    int zostaloSztuk = int.Parse(dostepnaIlosc) - int.Parse(ile);
                    if (zostaloSztuk < 0)
                    {
                        System.Windows.MessageBox.Show("can not take battery :c");
                        return false;
                    }
                    string Transfer = $"UPDATE `Items` SET `sztuki`='{zostaloSztuk.ToString()}' WHERE `name` = '{Name}'";


                    using (MySqlCommand myCommand2 = new MySqlCommand(Transfer, SQLConnection))
                    {
                        myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                    }
                    if (!ile.Contains("-"))
                    {
                        System.Windows.MessageBox.Show("Item Added");
                    }
                    
                }
                catch (Exception)
                {

                    System.Windows.MessageBox.Show("no access to data base #2");
                    return false;
                }
            }
            else
            {
                System.Windows.MessageBox.Show("no access to data base #1");
                return false;
            }
            return true;
        }


        public Device GetItem(string name, string Id) // zwraca obiekt Item 
        {
            Device item ;
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {
                    
                   
                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT * FROM `Items` WHERE `name` = '{name}' AND `id` = '{Id}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));
                            //string tmp = myReader.GetString(0);
                            //tmp = myReader.GetString(1);
                            //tmp = myReader.GetString(5);
                            //tmp = myReader.GetString(4);
                            //tmp = myReader.GetString(6);
                            //tmp = myReader.GetString(7);
                            try
                            {
                                item = new Device(myReader.GetString(0), myReader.GetString(1), tmp_pkt, myReader.GetString(5), myReader.GetString(4), myReader.GetString(6), myReader.GetString(7));

                            }
                            catch (Exception x)
                            {
                                item = new Device(myReader.GetString(0), myReader.GetString(1), tmp_pkt, myReader.GetString(5), myReader.GetString(4), myReader.GetString(6));

                            }
                            return item;
                        }

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                    //closeConnection();
                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                   // closeConnection();
                }

            }
            return null;
        }


        public List<string> GetStringItem(string name, string Id) // lista list  
        {
           List<string> item = new List<string>();
            if (DB_connection)
            {
                MySqlDataReader myReader;
                try
                {


                    using (MySqlCommand myCommand = new MySqlCommand($"SELECT * FROM `Items` WHERE `name` = '{name}' AND `id` = '{Id}' ", SQLConnection))
                    {
                        myReader = myCommand.ExecuteReader();
                        //SQLConnection.Close();

                        while (myReader.Read())
                        {
                            Point tmp_pkt = new Point(Convert.ToDouble(myReader.GetString(2)), Convert.ToDouble(myReader.GetString(3)));

                            try
                            {
                                item.Add(myReader.GetString(0));
                                item.Add(myReader.GetString(1));
                                item.Add(tmp_pkt.X.ToString());
                                item.Add(tmp_pkt.Y.ToString());
                                item.Add(myReader.GetString(5));
                                item.Add(myReader.GetString(4));
                                item.Add(myReader.GetString(6));
                                item.Add(myReader.GetString(7));

                            }
                            catch (Exception x)
                            {
                                item.Clear();
                                item.Add(myReader.GetString(0));
                                item.Add(myReader.GetString(1));
                                item.Add(tmp_pkt.X.ToString());
                                item.Add(tmp_pkt.Y.ToString());
                                item.Add(myReader.GetString(5));
                                item.Add(myReader.GetString(4));
                                item.Add(myReader.GetString(6));
                                item.Add("");
                            }
                            return item;
                        }

                    }
                }
                catch (Exception x)
                {
                    MessageBox.Show(x.ToString());
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8] + "\n" + x);
                    //closeConnection();
                }
            }
            else
            {
                try
                {
                    //MessageBox.Show(((MainWindow)System.Windows.Application.Current.MainWindow).Stringi[8]);
                }
                catch (Exception)
                {
                    MessageBox.Show("Error Data Base Manager #1");
                    // closeConnection();
                }

            }
            return null;
        }




        public void Transfer(string Item_name, string Item_ID, string owner)
        {
            if (DB_connection)
            {


                MySqlDataReader myReader;
                try
                {
                    //SQLConnection.Open();

                    string Transfer = $"UPDATE `Items` SET `owner`='{owner}' WHERE `name` = '{Item_name}' AND `id` = '{Item_ID}'";


                    using (MySqlCommand myCommand2 = new MySqlCommand(Transfer, SQLConnection))
                    {
                        myReader = myCommand2.ExecuteReader(); //ustawienie po edycji itemu
                    }

                    System.Windows.MessageBox.Show("Item transferred");
                }
                catch (Exception)
                {

                    System.Windows.MessageBox.Show("no access to data base #2");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("no access to data base #1");
            }
        }


    }
}
    

