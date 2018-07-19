using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CFakeViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private F f { get; set; }
        ImageWithName current;
        ImageWithName prev;
        ImageWithName next;
        int counter = 0;
        string mainUrl = "http://cfake.com/show/p";
        public MainWindow()
        {
            InitializeComponent();
            f = new F();
        }

        private void FullscreenClick(object sender, RoutedEventArgs e)
        {
            f.ToggleFullscreen(this);
        }
        private void ColapseClick(object sender, RoutedEventArgs e)
        {
            GridOverlay.Opacity = 0.0;
            expandBtn.Opacity = 0.5;
            GridOverlay.IsEnabled = false;
            Canvas.SetZIndex(GridOverlay, 999);
            Canvas.SetZIndex(expandBtn, 1000);
        }
        private void ExpandClick(object sender, RoutedEventArgs e)
        {
            expandBtn.Opacity = 0.0;
            GridOverlay.Opacity = 0.5;
            GridOverlay.IsEnabled = true;
            Canvas.SetZIndex(expandBtn, 999);
            Canvas.SetZIndex(GridOverlay, 1000);
        }

        private void KeyboardClick(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape: f.ToggleFullscreen(this); break;
                case Key.F: f.ToggleFullscreen(this); break;
                case Key.Up: App.Current.Shutdown(); break;
                case Key.Left: ShowPrevAsync(); break;
                case Key.Right: ShowNextAsync(); break;
                /*case Key.T: TestAsync(); break;*/
            }
        }

        private async void ShowNextAsync()
        {
            prev = current;
            if (next != null)
                current = next;
            else
                MessageBox.Show("No more images");

            imageBox.Source = current.Image;
            nameButton.Content = current.Name;
            try
            {
                ulong i = current.ID + 1;
                /*if (favNewest)
                {
                    i = i % 30;
                    if (i == 0)
                    {
                        Names = await GetNames(++currentList);
                    }

                    while (true)
                    {
                        while (true)
                        {
                            if (fav.Find(s => s == Names[(int)i]) != null)
                            {
                                i = (ulong)currentList * 30 + i;
                                goto FoundNext;
                            }
                            else
                            {
                                i++;
                                if (i == 30)
                                {
                                    i = 0;
                                    currentList++;
                                    break;
                                }
                            }
                        }
                        Names = await GetNames(currentList);
                    }
                }
            FoundNext:*/
                next = await GetImageWithNameAsync(i);
            }
            catch
            {
                next = null;
            }
        }

        private async void ShowPrevAsync()
        {
            if (prev != null)
            {
                next = current;
                current = prev;

                imageBox.Source = current.Image;
                nameButton.Content = current.Name;
                if (current.ID != 0)
                {
                    ulong i = current.ID - 1;
                    /*if (favNewest)
                    {
                        i = i % 30;
                        if (i == 0 && currentList != 0)
                        {
                            Names = await GetNames(--currentList);
                        }

                        while (true)
                        {
                            while (true)
                            {
                                if (fav.Find(s => s == Names[(int)i]) != null)
                                {
                                    i = (ulong)currentList * 30 + i;
                                    goto FoundNext;
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        i = 29;
                                        currentList--;
                                        break;
                                    }
                                    else
                                        i--;
                                }
                            }
                            if (currentList == -1)
                            {
                                prev = null;
                                return;
                            }
                            Names = await GetNames(currentList);
                        }
                    }
                FoundNext:*/
                    prev = await GetImageWithNameAsync(i);
                }
                else
                    prev = null;
            }
        }

        private async Task<ImageWithName> GetImageWithNameAsync(ulong v)
        {
            string url = mainUrl + v.ToString();
            string pageSource;

            using (HttpClient client = new HttpClient())
                pageSource = await client.GetStringAsync(url);

            pageSource = pageSource.Replace("\n", "");
            pageSource = pageSource.Replace("\t", "");

            string nameCompare = "<td width=\"100%\"><a href=\"/picture/";
            int nameIndex = pageSource.IndexOf(nameCompare) + nameCompare.Length;
            pageSource = pageSource.Remove(0, nameIndex);
            string name = pageSource.Substring(0, pageSource.IndexOf("/"));
            name = name.Replace('_', ' ');
            if (name == "")
                return null;

            string imgCompare = "<img src=\"/medias/photos/";
            int imgIndex = pageSource.IndexOf(imgCompare) + imgCompare.Length;
            pageSource = pageSource.Remove(0, imgIndex);
            string filename = pageSource.Substring(0, pageSource.IndexOf('"'));
            string imgUrl = "http://cfake.com/medias/photos/" + filename;

            var image = await f.GetImageAsync(imgUrl);

            return new ImageWithName(image, name, filename, v);
        }

        private async void LoadAsync(object sender, RoutedEventArgs e)
        {
            current = await GetImageWithNameAsync(0);
            next = await GetImageWithNameAsync(1);

            imageBox.Source = current.Image;
            nameButton.Content = current.Name;
        }
    }

    public class ImageWithName
    {
        public ImageWithName(BitmapImage image, string name, string filename, ulong id)
        {
            Image = image;
            Name = name;
            FileName = filename;
            ID = id;
        }

        public BitmapImage Image { get; set; }
        public String Name { get; set; }
        public String FileName { get; set; }
        public ulong ID { get; set; }
    }
}
