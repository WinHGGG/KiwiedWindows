using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Media;
using System.Windows.Input;
public static class BtMapSource
{
    public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap bmp)
    {
        BitmapSource returnSource;
        try
        {
            returnSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
        catch
        {
            returnSource = null;
            System.Environment.Exit(1);
        }
        return returnSource;
    }
}
namespace KiwiedWindows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        int nowstatx = 1, nowstaty = 1;
        private void Full_Screen()
        {
            //Set Full Screen
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.NoResize;
            this.Topmost = true;
            this.Left = 0.0;
            this.Top = 0.0;
            this.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
            return;
        }
        public Bitmap GetScreenSnapshot()
        {
            System.Drawing.Rectangle rc = SystemInformation.VirtualScreen;
            var bitmap = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics memoryGrahics = Graphics.FromImage(bitmap))
            {
                memoryGrahics.CopyFromScreen(rc.X, rc.Y, 0, 0, rc.Size, CopyPixelOperation.SourceCopy);
            }
            return bitmap;
        }
        public void Initimage()
        {
            BitmapImage shoot = new BitmapImage();
            shoot.BeginInit();
            shoot.StreamSource = new MemoryStream(ReadImageMemory(GetScreenSnapshot()));
            shoot.EndInit();
            disp.Source = shoot;
            return;
        }
        private static byte[] ReadImageMemory(Bitmap btmp)
        {
            BitmapSource bitmapSource = BtMapSource.ToBitmapSource(btmp);
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            return memoryStream.GetBuffer();
        }
        public void Hideit()
        {
            disp.Visibility = Visibility.Collapsed;
            return;
        }
        public void Transit()
        {
            ScaleTransform scaleTransform = new ScaleTransform
            {
                CenterX = this.Width / 2,
                CenterY = this.Height / 2,
                ScaleX = nowstatx
            };
            nowstatx = -nowstatx;
            scaleTransform.ScaleY = nowstaty;
            disp.RenderTransform = scaleTransform; 
        }
        private void Transforms()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                DateTime starter = DateTime.Now;
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 4.8) { }
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 20)
                {
                    disp.Dispatcher.Invoke(new Action(Transit));
                    if (DateTime.Now.Subtract(starter).TotalSeconds >= 10 && DateTime.Now.Subtract(starter).TotalSeconds <= 15) { nowstaty = -nowstaty; }
                    Thread.Sleep(100);
                }
            }))
            {
                //In case Main exits.
                IsBackground = true
            };
            thread.Start();
        }
        private void Countdowns()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                DateTime starter = DateTime.Now;
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 23) { Thread.Sleep(100); }
                disp.Dispatcher.Invoke(new Action(Hideit));
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 41) { Thread.Sleep(100); }
                if (System.IO.File.Exists(@"C:\ProgramData\Uc207Pr4f57t9.Riku.mp4"))
                {
                    try
                    {
                        System.IO.File.Delete(@"C:\ProgramData\Uc207Pr4f57t9.Riku.mp4");
                    }
                    catch (System.IO.IOException e)
                    {
                        Console.WriteLine(e.Message);
                        System.Environment.Exit(1);
                        return;
                    }
                }
                System.Environment.Exit(0);
            }))
            {
                //In case Main exits.
                IsBackground = true
            };
            thread.Start();
        }
        public void Writeout()
        {
            //Get resource from exefile
            string name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".video.mp4";
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(name);
            if (!System.IO.File.Exists(@"C:\ProgramData\Uc207Pr4f57t9.Riku.mp4"))
            {
                try
                {
                    var fileStream = File.Create(@"C:\ProgramData\Uc207Pr4f57t9.Riku.mp4");
                    stream.CopyTo(fileStream);
                    fileStream.Close();
                }
                catch (System.IO.IOException e)
                {
                    System.Windows.MessageBox.Show("Chtholly Lives!", "艾拉");
                    Console.WriteLine(e.Message);
                    System.Environment.Exit(1);
                    return;
                }
            }
        }
        // Handle Alt+F4..
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = true;
            //General Hooks CAN'T ban Ctrl+Alt+Del
        }
        public MainWindow()
        {
            InitializeComponent();
            Initimage();
            Full_Screen();
            Writeout();
            vide.Source = new Uri(@"C:\ProgramData\Uc207Pr4f57t9.Riku.mp4", UriKind.Relative);
            Countdowns();
            Transforms();
            vide.Play();
        }

    }
}