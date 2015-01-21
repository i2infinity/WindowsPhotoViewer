using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Models;

namespace PhotoLoader.Interfaces
{
    public interface IRetrievePhotos
    {
        /// <summary>
        /// Returns the user's selection of photos
        /// </summary>
        /// <param name="dataSource">The location/source from which the photos will be selected</param>
        /// <param name="pageSize">Total number of photos to return. (pass null to avoid pagination)</param>
        /// <returns></returns>
        Task<IEnumerable<Photo>> RetrievePhotos(PhotoDataSource dataSource, Int32? pageSize = null);
    }
}
