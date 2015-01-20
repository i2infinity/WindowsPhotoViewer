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
        /// <param name="pageSize">Total number of photos to return. (pass null to avoid pagination)</param>
        /// <returns></returns>
        Task<IEnumerable<Photo>> RetrievePhotos(Int32? pageSize = null);
    }
}