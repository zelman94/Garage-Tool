using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing.Common;
using ZXing;
using ZXing.QrCode;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GarageTool
{
    class Qrclass
    {
        public string decoded;
        QrCodeEncodingOptions options; // options = new QrCodeEncodingOptions();
        public Qrclass()
        {
           
            options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = 50,
                Height = 50,
            };
            var writer = new BarcodeWriter();
            writer.Format = BarcodeFormat.QR_CODE;
            writer.Options = options;
        }



        public ImageSource ImageSourceForBitmap(Bitmap bmp)
        {
            var handle = bmp.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch {
                return null;
            }
        }

        public ImageSource createQR(string tekst, int wymiar)
        {
            if (tekst=="")
            {
                //pictureBox1.Image = null;
                System.Windows.Forms.MessageBox.Show("Text not found", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            else
            {
                options.Width = wymiar;
                options.Height = wymiar;
                var qr = new ZXing.BarcodeWriter();
                qr.Options = options;
                qr.Format = ZXing.BarcodeFormat.QR_CODE;
                var result = new Bitmap(qr.Write(tekst.Trim()));
                ImageSource tmp = ImageSourceForBitmap(result);
                try
                {
                    ((EditItems)System.Windows.Application.Current.MainWindow).Qrimage.Source = tmp;
                }
                catch (Exception)
                {
                    
                }
               
                return tmp;
            }
        }
        public Device GetItemFromstring(string item)
        {
            Char delimiter = ';';
            try
            {
                String[] substrings = item.Split(delimiter);
                List<string> dane = new List<string>();
                foreach (var substring in substrings)
                    dane.Add(substring);

                System.Windows.Point tmp = new System.Windows.Point(Double.Parse(dane[2]), Double.Parse(dane[3]));
                return new Device(dane[0], dane[1], tmp, dane[4], dane[5], dane[6]);
            }
            catch (Exception)
            {
                return null;
            }

        }

        public Device GetInfoQr()
        {
            Device QrItem;//= new Item();

            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory(); ;
            openFileDialog1.Filter = "QRCode (*.JPG)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {                            
                            try
                            {
                                Bitmap bitmap = new Bitmap(openFileDialog1.FileName);
                                BarcodeReader reader = new BarcodeReader { AutoRotate = true, TryInverted = true };

                                Result result = reader.Decode(bitmap);
                                decoded = result.ToString().Trim();

                                return QrItem = GetItemFromstring(decoded);
                                //textBox1.Text = decoded;
                            }
                            catch (Exception)
                            {
                               // System.Windows.Forms.MessageBox.Show("Image not found", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }


           

            return null;
        }

        
    }
}
