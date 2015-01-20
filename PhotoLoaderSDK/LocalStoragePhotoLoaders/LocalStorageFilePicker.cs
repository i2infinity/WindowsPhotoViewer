using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using PhotoLoader.Models;

namespace PhotoLoaderSDK.LocalStoragePhotoLoaders
{
    public class LocalStorageFilePicker : IFilePicker<StorageFile>
    {
        private readonly FileOpenPicker _filePicker;

        public LocalStorageFilePicker()
        {
            _filePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.List,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
        }

        public void AddFilter(string fileType)
        {
            _filePicker.FileTypeFilter.Add(fileType);
        }

        public async Task<IReadOnlyList<StorageFile>> PickFiles(Boolean allowMultiple = true)
        {
            return await _filePicker.PickMultipleFilesAsync();
        }
    }
}
