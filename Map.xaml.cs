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
using System.IO;
namespace GarageTool
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : Window
    {
        public Map() { }
        Item item;
        Qrclass QrGenerator = new Qrclass();
        public Map(Item item)
        {
            try
            {


            InitializeComponent();
            string path = Directory.GetCurrentDirectory();
            imgmapa.Source = new BitmapImage(new Uri($"{path}/Image.jpg", UriKind.Absolute));
            ImgQRSelectedItem.Source = QrGenerator.createQR($"{item.Name};{item.Id};{item.Position.X};{item.Position.Y};{item.Owner};{item.Status}",50);

            this.item = item;

            // szafa:
            if (item.Lokal != "S")
            {
                foreach (UIElement element in mapa.Children)
                {
                    try
                    {
                        Ellipse childType = element as Ellipse;
                        if (childType.Name == $"dot_{item.Position.X}_{item.Position.Y}")
                        {
                            childType.Fill = new SolidColorBrush(Colors.Red);
                            // childType.IsEnabled = true;
                        }
                    }
                    catch (Exception ) { }
                }
            }
            else
            {
                    foreach (UIElement element in mapa.Children)
                    {
                        try
                        {
                            Ellipse childType = element as Ellipse;
                            if (childType.Name == $"dot_{item.Position.X}_{item.Position.Y}_s")
                            {
                                childType.Fill = new SolidColorBrush(Colors.Red);
                                // childType.IsEnabled = true;
                            }
                        }
                        catch (Exception ) { }
                    }
                }
            }
            catch (Exception Ex)
            {

                MessageBox.Show(Ex.ToString());
            }



            //margin.Left = item.Position.X;
            //margin.Top = item.Position.Y;

            //dot.Margin = margin;

        }



    }
}
