using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using PhotoLoader.Models;

namespace PhotoLoaderSDK.LocalStoragePhotoLoader
{
    public class LocalStorageLoader : IPhotoRetriever
    {
        private readonly IFilePicker<StorageFile> _filePicker; 

        public LocalStorageLoader(IFilePicker<StorageFile> filePicker)
        {
            this._filePicker = filePicker;
        }

        public async Task<IEnumerable<Photo>> RetrievePhotos(IEnumerable<KeyValuePair<String, Object>> filters = null)
        {
            var results = new List<Photo>();
            _filePicker.AddFilter(".jpg");
            _filePicker.AddFilter(".jpeg");
            _filePicker.AddFilter(".png");

            var files = await _filePicker.PickFiles();
            if (files.Count > 0)
            {
                // Application now has read/write access to the picked file(s)
                results.AddRange(files.Select(file => new Photo
                {
                    FilePath = file.Path,
                    FileName = file.Name,
                    DateCreated = file.DateCreated.UtcDateTime,
                    DisplayName = file.DisplayName,
                }));
            }

            return results;
        }
    }
}
