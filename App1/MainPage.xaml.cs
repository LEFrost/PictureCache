using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace App1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private List<test> _test;
        ObservableCollection<Class1> showlist = new ObservableCollection<Class1>();
        public MainPage()
        {
            this.InitializeComponent();
            string json = httpclient();
            string b = @"http://tnfs.tngou.net/img";
            show.ItemsSource = showlist;
            JObject j = JObject.Parse(json);
            for (int i = 0; i < 5; i++)
            {
                test t = new test()
                {
                    Image1 = b + j["tngou"][i]["img"].ToString()
                };
                unknown(t.Image1, i);
                Test.Add(t);
            }
        }

        public List<test> Test
        {
            get
            {
                return _test ?? (_test = new List<test>());
            }

            set
            {
                _test = value;
            }
        }

        public string httpclient()
        {
            HttpClient http = new HttpClient();
            string content = "";
            HttpResponseMessage response = http.GetAsync(@"http://www.tngou.net/tnfs/api/list").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                content = response.Content.ReadAsStringAsync().Result;
            return content;
        }
        public async void unknown(string url, int i)
        {
            WriteableBitmap wb = await GetWriteableBitmapAsync(url);
            string name = i.ToString() + ".png";
            await SaveImageAsync(wb, name);

        }
        public async Task<IBuffer> getBufferHttp(string url)
        {
            Windows.Web.Http.HttpClient http = new Windows.Web.Http.HttpClient();

            string content = "";
            var response = await http.GetBufferAsync(new Uri(url));
            return response;
        }
        public async Task<WriteableBitmap> GetWriteableBitmapAsync(string url)
        {
            try
            {
                IBuffer buffer = await getBufferHttp(url);
                if (buffer != null)
                {
                    BitmapImage bi = new BitmapImage();
                    WriteableBitmap wb = null;
                    Stream stream1;
                    using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                    {
                        stream1 = stream.AsStreamForWrite();
                        await stream1.WriteAsync(buffer.ToArray(), 0, (int)buffer.Length);
                        await stream1.FlushAsync();
                        stream.Seek(0);
                        await bi.SetSourceAsync(stream);
                        wb = new WriteableBitmap(bi.PixelWidth, bi.PixelHeight);
                        stream.Seek(0);
                        await wb.SetSourceAsync(stream);

                        return wb;
                    }

                }
                else return null;
            }
            catch
            {
                return null;
            }
        }
        public async Task SaveImageAsync(WriteableBitmap image, string filename)
        {
            StorageFolder localFolder =
    Windows.Storage.ApplicationData.Current.LocalFolder;
            try
            {
                if (image == null)
                {
                    return;
                }
                Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                if (filename.EndsWith("jpg"))
                    BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
                else if (filename.EndsWith("png"))
                    BitmapEncoderGuid = BitmapEncoder.PngEncoderId;
                else if (filename.EndsWith("bmp"))
                    BitmapEncoderGuid = BitmapEncoder.BmpEncoderId;
                else if (filename.EndsWith("tiff"))
                    BitmapEncoderGuid = BitmapEncoder.TiffEncoderId;
                else if (filename.EndsWith("gif"))
                    BitmapEncoderGuid = BitmapEncoder.GifEncoderId;
                var folder = await localFolder.CreateFolderAsync("images_cache", CreationCollisionOption.OpenIfExists);
                var file = await folder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);

                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                    Stream pixelStream = image.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                              (uint)image.PixelWidth,
                              (uint)image.PixelHeight,
                              96.0,
                              96.0,
                              pixels);
                    await encoder.FlushAsync();
                }
            }
            catch
            {

            }
        }
        public async Task WriteToFileAsync(StorageFolder folder, SoftwareBitmap sb, string fileName)
        {

            if (sb != null)
            {
                // save image file to cache
                StorageFile file = await folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    encoder.SetSoftwareBitmap(sb);
                    await encoder.FlushAsync();
                }
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFolder image_cache = await localFolder.GetFolderAsync("images_cache");
            IReadOnlyList<StorageFile> files = null;
            try
            {
                files =await image_cache.GetFilesAsync();
            }
            catch (Exception xe)
            {
                Debug.Write(xe);
            }
            foreach (var item in files)
            {

                showlist.Add(new Class1() { Image = item.Path });
                    //BitmapImage bi = new BitmapImage();
                    //WriteableBitmap wb = null;
                    //Stream stream1;
                    //byte[] b;
                    //using (IRandomAccessStreamWithContentType stream = await item.OpenReadAsync())
                    //{
                    //    stream1 = stream.AsStreamForRead();
                    //    b = new byte[stream1.Length];
                    //    await stream1.ReadAsync(b, 0, (int)stream1.Length);
                    //    await stream1.FlushAsync();
                    //    stream.Seek(0);
                    //    await bi.SetSourceAsync(stream);
                    //    wb = new WriteableBitmap(bi.PixelWidth, bi.PixelHeight);
                    //    showlist.Add(new Class1() { Image = wb });


                    //}
                
            }
        }
    }
    public class test : INotifyPropertyChanged
    {
        private string image;

        public string Image1
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
                onp(nameof(image));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void onp(string n)
        {
            if (PropertyChanged != null)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        }
    }


}

