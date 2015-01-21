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
        public static IList<Photo> MapToStorePhoto(this IEnumerable<StorageFile> storePhotos)
        {
            var result = new List<Photo>();
            var photos = storePhotos as IList<StorageFile> ?? storePhotos.ToList();

            if (storePhotos == null || !photos.Any())
            {
                return result;
            }

            foreach (var storePhoto in photos)
            {
                var imageItem = new Photo
                {
                    DisplayName = storePhoto.DisplayName,
                    FileName = storePhoto.Name,
                    FilePath = storePhoto.Path,
                    StorageFile = storePhoto
                };

                result.Add(imageItem);
            }

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
