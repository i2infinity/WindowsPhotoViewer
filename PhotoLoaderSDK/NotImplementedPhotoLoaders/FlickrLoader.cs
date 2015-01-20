using System.Collections.Generic;
using System.Threading.Tasks;
using PhotoLoader.Models;

namespace PhotoLoaderSDK.PhotoLoaders
{
    public class FlickrLoader : IPhotoRetriever
    {
        public Task<IEnumerable<Photo>> RetrievePhotos(int? pageSize = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
