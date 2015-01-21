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
        /// <param name="filters">Filters to use while retrieving the photos</param>
        /// <returns></returns>
        Task<IEnumerable<Photo>> RetrievePhotos(PhotoDataSource dataSource, IEnumerable<KeyValuePair<String, Object>> filters = null);
    }
}
