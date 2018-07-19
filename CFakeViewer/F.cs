using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CFakeViewer
{
    /// <summary>
    /// A class for functions
    /// </summary>
    class F
    {
        public void ToggleFullscreen(MainWindow window)
        {
            if (window.WindowState == WindowState.Normal)
            {
                window.WindowStyle = WindowStyle.None;
                window.WindowState = WindowState.Maximized;
                window.fullscreenBtn.Content = "Normal";
            }
            else
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.fullscreenBtn.Content = "Fullscreen";
            }
        }
        public async Task<BitmapImage> GetImageAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            using (Stream stream = await client.GetStreamAsync(url))
            using (MemoryStream ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
