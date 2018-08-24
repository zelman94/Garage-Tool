using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using AutoUpdaterDotNET;

//https://github.com/ravibpatel/AutoUpdater.NET


namespace GarageTool
{
     // można dorobić logike na update
    class FTP
    {
       private FtpWebRequest request;

        public FTP()
        {
            AutoUpdater.Start();
        }
        public string checkVersionOnServer()
        {
            WebClient request = new WebClient();
            string url = "ftp://garage%2540zadanko-z-zutu.cba.pl@zadanko-z-zutu.cba.pl/Garage/info.txt";
            request.Credentials = new NetworkCredential("garage@zadanko-z-zutu.cba.pl", "Santiego94");

            try
            {
                byte[] newFileData = request.DownloadData(url);
                string fileString = System.Text.Encoding.UTF8.GetString(newFileData);
                return fileString;
            }
            catch (WebException e)
            {
                // Do something such as log error, but this is based on OP's original code
                // so for now we do nothing.
                return "ERROR";
            }

        }
        public bool czyAktualnaversia() //false zwraca gdy nie jest aktualna czyli masz update
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            string assemblyVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string serversVER = checkVersionOnServer();

            if (serversVER != assemblyVersion && serversVER != "ERROR")
            {
                return false;
            }
            return true;
        }


        public void DownloadFTPFiles(string ftpAddress, string UserName, string Password)
        {
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(ftpAddress);

            try
            {
                reqFTP.UsePassive = true;
                reqFTP.UseBinary = true;
                reqFTP.KeepAlive = false;
                reqFTP.Credentials = new NetworkCredential(UserName, Password);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();

                Stream responseStream = response.GetResponseStream();
                List<string> files = new List<string>();
                StreamReader reader = new StreamReader(responseStream);
                while (!reader.EndOfStream)
                    files.Add(reader.ReadLine());
                reader.Close();
                responseStream.Dispose();
               // List<periodicalfile> lstFiles = new List<periodicalfile>();
                //Loop through the resulting file names.
                string ftpPath = string.Empty;
                WebClient wc = new WebClient();
                foreach (var fileName in files)
                {
                    var parentDirectory = "";
                    if (fileName.Contains(".rar"))
                    {
                        ftpPath = ftpAddress ;
                        wc.Credentials = new NetworkCredential(UserName, Password);
                        wc.DownloadFile(ftpAddress, @"C:\ProgramData\Garage\" + fileName);//DOWLOAD FROM FTP
                        MessageBox.Show("Update downloaded : \n"+ @"C:\ProgramData\Garage\"+fileName);
                    }
                    else
                    {
                        //If the filename has no extension, then it is just a folder. 
                        //Run this method again as a recursion of the original:
                        parentDirectory += fileName + " / ";
                        try
                        {
                            DownloadFTPFiles(ftpAddress + "/" + parentDirectory, UserName, Password);
                        }
                        catch (Exception ex)
                        {
                          
                        }
                    }
                }
            }
            catch (Exception excpt)
            {
                reqFTP.Abort();
                
            }
        }



        public bool ConectToFTP()
        {
            try
            {
                request = (FtpWebRequest)WebRequest.Create("ftp://garage%2540zadanko-z-zutu.cba.pl@zadanko-z-zutu.cba.pl/Garage/info.txt");
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;


                request.Credentials = new NetworkCredential("garage@zadanko-z-zutu.cba.pl", "Santiego94");
                request.KeepAlive = false;
                request.UseBinary = true;
                request.UsePassive = true;

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
