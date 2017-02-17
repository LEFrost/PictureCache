using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace App1
{
    class Class1 :INotifyPropertyChanged
    {
        private string _Image;

        public string Image
        {
            get
            {
                return _Image;
            }

            set
            {
                _Image = value;
                if (PropertyChanged != null)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Image)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
