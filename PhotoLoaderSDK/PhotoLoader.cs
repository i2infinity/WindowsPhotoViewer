using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Interfaces;
using PhotoLoader.Models;
using PhotoLoaderSDK.LocalStoragePhotoLoaders;
using PhotoLoaderSDK.PhotoLoaders;

namespace PhotoLoaderSDK
{
    public class PhotoLoader : IRetrievePhotos
    {
        /// <summary>
        /// Returns the user's selection of photos
        /// Note : Currently only PhotoDataSource.LocalStorage is supported
        /// </summary>
        /// <param name="dataSource"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Photo>> RetrievePhotos(PhotoDataSource dataSource, int? pageSize = null)
        {
            IEnumerable<Photo> results;

            switch (dataSource)
            {
                case PhotoDataSource.LocalStorage:
                    results = await (new LocalStorageLoader(new LocalStorageFilePicker())).RetrievePhotos(pageSize);
                    break;

                case PhotoDataSource.Facebook:
                    results = await (new FacebookLoader()).RetrievePhotos(pageSize);
                    break;

                case PhotoDataSource.Flickr:
                    results = await (new FlickrLoader()).RetrievePhotos(pageSize);
                    break;

                case PhotoDataSource.Instagram:
                    results = await (new InstagramLoader()).RetrievePhotos(pageSize);
                    break;

                default:
                    results = await (new LocalStorageLoader(new LocalStorageFilePicker())).RetrievePhotos(pageSize);
                    break;
            }

            return results;
        }
    }
}
