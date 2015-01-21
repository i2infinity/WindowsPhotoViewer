using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace WindowsPhotoViewer
{
    public class ImageItem
    {
        public BitmapImage ImageSource { get; set; }
        
        public String DisplayName { get; set; }

        public String FileName { get; set; }

        public String FilePath { get; set; }

        public StorageFile StorageFile { get; set; }
    }
}
