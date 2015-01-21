using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using PhotoLoader.Models;

namespace WindowsPhotoViewer
{
    public class LazyPhotoLoader : ObservableCollection<ImageItem>, ISupportIncrementalLoading
    {
        private readonly IList<Photo> _selectedPhotos;
        private readonly Int32 _defaultPageSize;
        private Int32 _actualCount;

        public LazyPhotoLoader(IList<Photo> allPhotos)
        {
            _selectedPhotos = allPhotos;
            _actualCount = 0;
            _defaultPageSize = 40;
            HasMoreItems = allPhotos.Any();
        }

        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            return InnerLoadMoreItemsAsync(count).AsAsyncOperation();
        }

        public bool HasMoreItems { get; private set; }

        private async Task<LoadMoreItemsResult> InnerLoadMoreItemsAsync(uint expectedCount)
        {
            var images = await _selectedPhotos.Skip(_actualCount).Take(_defaultPageSize).MapToImageItemThumbNail();

            foreach (var imageItem in images)
            {
                Add(imageItem);
            }

            _actualCount += images.Count;

            if (_actualCount >= _selectedPhotos.Count)
            {
                HasMoreItems = false;
            }

            return new LoadMoreItemsResult
            {
                Count = (uint)_actualCount
            };
        }
    }
}
