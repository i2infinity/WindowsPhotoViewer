using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Models;

namespace PhotoLoaderSDK
{
    public interface IPhotoRetriever
    {
        /// <summary>
        /// Returns the user's selection of photos
        /// </summary>
        /// <param name="filters">Filters to use while retrieving the photos</param>
        /// <returns></returns>
        Task<IEnumerable<Photo>> RetrievePhotos(IEnumerable<KeyValuePair<String, Object>> filters = null);
    }
}