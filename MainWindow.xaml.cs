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
        double nowstatx = 1, nowstaty = 1, nowcenterx = System.Windows.SystemParameters.PrimaryScreenWidth /2;
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
            var width = SystemInformation.VirtualScreen.Width;
            var height = SystemInformation.VirtualScreen.Height;
            Bitmap Bmp = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(Bmp))
            using (SolidBrush brush = new SolidBrush(System.Drawing.Color.FromArgb(0, 0, 0)))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }
            IntPtr hBitmap = Bmp.GetHbitmap();
            System.Windows.Media.ImageSource WpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            shoot.BeginInit();
            shoot.StreamSource = new MemoryStream(ReadImageMemory(GetScreenSnapshot()));
            shoot.EndInit();
            disp.Source = shoot;
            disp_.Source = shoot;
            cov.Source = WpfBitmap;
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
        public void Hideit_()
        {
            disp_.Visibility = Visibility.Collapsed;
            return;
        }
        public void Hidecov()
        {
            cov.Visibility = Visibility.Collapsed;
            return;
        }
        public void Transit()
        {
            ScaleTransform scaleTransform = new ScaleTransform
            {
                CenterX = nowcenterx,
                CenterY = this.Height / 2,
                ScaleX = nowstatx,
                ScaleY = nowstaty
        };
            nowstatx = -nowstatx;
            disp.RenderTransform = scaleTransform; 
        }
        public void Transit_()
        {
            ScaleTransform scaleTransform = new ScaleTransform
            {
                CenterX = 0,
                CenterY = this.Height / 2,
                ScaleX = -nowstatx,
                ScaleY = -nowstaty
        };
            disp_.RenderTransform = scaleTransform;
            nowstatx = -nowstatx;
        }
        private void Transforms()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                bool scal_flag = false;
                DateTime starter = DateTime.Now;
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 5.4) { }
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 20)
                {
                    if (DateTime.Now.Subtract(starter).TotalSeconds >= 11 && DateTime.Now.Subtract(starter).TotalSeconds <= 15) { nowstaty = -nowstaty; }
                    if (DateTime.Now.Subtract(starter).TotalSeconds >= 16 && !scal_flag) 
                    { 
                        nowstatx = 0.5; 
                        scal_flag = true;
                        System.Drawing.Rectangle rc = SystemInformation.VirtualScreen;
                        nowcenterx = rc.Width;
                        nowstaty = 1;
                    }
                    disp.Dispatcher.Invoke(new Action(Transit));
                    Thread.Sleep(100);
                }
                nowstatx = 1;nowstaty = 1; disp.Dispatcher.Invoke(new Action(Transit)); 
            }))
            {
                IsBackground = true
            };
            Thread thread_ = new Thread(new ThreadStart(() =>
            {
                DateTime starter = DateTime.Now;
                bool scal_flag = false;
                while (DateTime.Now.Subtract(starter).TotalSeconds <=20 )
                {
                    if (DateTime.Now.Subtract(starter).TotalSeconds <= 15) { }
                    else
                    {
                        if (DateTime.Now.Subtract(starter).TotalSeconds >= 15 && !scal_flag)
                        {
                            nowstatx = 0.5;
                            scal_flag = true;
                            System.Drawing.Rectangle rc = SystemInformation.VirtualScreen;
                            nowcenterx = rc.Width;
                            nowstaty = 1;
                        }
                        disp_.Dispatcher.Invoke(new Action(Transit_));
                        Thread.Sleep(50);
                    }
                }
                nowstatx = 1; nowstaty = 1; disp_.Dispatcher.Invoke(new Action(Transit_));
                System.Windows.MessageBox.Show("Error","",MessageBoxButton.OK,MessageBoxImage.Error,MessageBoxResult.OK,System.Windows.MessageBoxOptions.ServiceNotification);
            }))
            {
                IsBackground = true
            };
            thread.Start();
            thread_.Start();
        }
        private void Countdowns()
        {

            Thread thread = new Thread(new ThreadStart(() =>
            {
                DateTime starter = DateTime.Now;
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 24) { Thread.Sleep(100); }
                disp.Dispatcher.Invoke(new Action(Hideit));
                disp_.Dispatcher.Invoke(new Action(Hideit_));
                cov.Dispatcher.Invoke(new Action(Hidecov));
                while (DateTime.Now.Subtract(starter).TotalSeconds <= 42) { Thread.Sleep(100); }
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