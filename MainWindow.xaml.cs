using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Image_Simplification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Bitmap _bitmap;
        bool _loaded; //if image was loaded = true
        public MainWindow()
        {
            InitializeComponent();
            _bitmap = new(1,1);
            _loaded = false;
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void loadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog _open = new();

            _open.InitialDirectory = "c:\\";
            _open.Filter = "Image Files(*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF";
            _open.RestoreDirectory = true;

            if (_open.ShowDialog() == true)
            {
                //Loads image into bitmap and fills background with it
                _bitmap = new Bitmap(_open.FileName);
                imgBg.Fill = new ImageBrush(BitmapConvert(_bitmap));
                _loaded = true;
            }
            else MessageBox.Show("Select an Image");
        }

        private async void toGray_Click(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                //Manually converts to Grayscale using NTSC Scale 0.299 * Red / 0.587 * Green / 0.114 * Blue
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    for (int y = 0; y < _bitmap.Height; y++)
                    {
                        System.Drawing.Color pixel = _bitmap.GetPixel(x, y);
                        int _temp = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                        _bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.A, _temp, _temp, _temp));
                    }
                    //it's pretty to watch when it's not instant
                    await Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromTicks(1));
                    });
                    imgBg.Fill = new ImageBrush(BitmapConvert(_bitmap));
                }   
            }
        }

        //Same as to gray, but it's either black or white
        private async void BW_Click(object sender, RoutedEventArgs e)
        {
            if (_loaded)
            {
                for (int x = 0; x < _bitmap.Width; x++)
                {
                    for (int y = 0; y < _bitmap.Height; y++)
                    {
                        System.Drawing.Color pixel = _bitmap.GetPixel(x, y);
                        int _temp = (pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114 >= 128) ? 255: 0; //if gray > 128 it's closer to white, else to black
                        _bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(pixel.A, _temp, _temp, _temp));
                    }
                    await Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromTicks(1));
                    });
                    imgBg.Fill = new ImageBrush(BitmapConvert(_bitmap));
                }
            }
        }

        private void pngSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog _save = new SaveFileDialog();
            _save.Filter = "Image Files(*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF";
            _save.ShowDialog();

            //valid file name
            if (_save.FileName != "")
            {
                switch (_save.FilterIndex)
                {
                    case 1:
                        _bitmap.Save(_save.FileName,
                          System.Drawing.Imaging.ImageFormat.Png);
                        break;

                    case 2:
                        _bitmap.Save(_save.FileName,
                          System.Drawing.Imaging.ImageFormat.Bmp);
                        break;

                    case 3:
                        _bitmap.Save(_save.FileName,
                          System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case 4:
                        _bitmap.Save(_save.FileName,
                          System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                }
            }
        }

        //Converts Bitmap to Bitmap Image (so it can be handled by WPF, while maintaining the bitmap which we'll use to manipulate it)
        private static BitmapImage BitmapConvert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}