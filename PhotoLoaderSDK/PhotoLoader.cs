using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Interfaces;
using PhotoLoader.Models;

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
            var photoLoaderFactory = new PhotoLoaderFactory();
            var photoLoader = photoLoaderFactory.GetPhotoRetriever(dataSource);
            return await photoLoader.RetrievePhotos(pageSize);
        }
    }
}
