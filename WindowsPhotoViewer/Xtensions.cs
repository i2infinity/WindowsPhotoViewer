using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
using PhotoLoader.Models;

namespace WindowsPhotoViewer
{
    public static class Xtensions
    {
        public static IList<ImageItem> MapToImageItem(this IEnumerable<Photo> storePhotos)
        {
            var result = new List<ImageItem>();
            var photos = storePhotos as IList<Photo> ?? storePhotos.ToList();

            if (storePhotos == null || !photos.Any())
            {
                return result;
            }

            result.AddRange(from storePhoto in photos
                let uri = new Uri(storePhoto.FilePath)
                let bitmapImage = new BitmapImage(uri) {DecodePixelWidth = 200}
                select new ImageItem
                {
                    DisplayName = storePhoto.DisplayName, FileName = storePhoto.FileName, FilePath = storePhoto.FilePath, ImageSource = bitmapImage,
                });

            return result;
        }

        public async static Task<IList<ImageItem>> MapToImageItemThumbNail(this IEnumerable<Photo> storePhotos)
        {
            var result = new List<ImageItem>();
            var photos = storePhotos as IList<Photo> ?? storePhotos.ToList();

            if (storePhotos == null || !photos.Any())
            {
                return result;
            }

            foreach (var storePhoto in photos)
            {
                var fileThumbnail = await storePhoto.StorageFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 150);
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(fileThumbnail);

                var imageItem = new ImageItem
                {
                    DisplayName = storePhoto.DisplayName,
                    FileName = storePhoto.FileName,
                    FilePath = storePhoto.FilePath,
                    ImageSource = bitmapImage,
                };
                
                result.Add(imageItem);
            }

            return result;
        }

    }
}
