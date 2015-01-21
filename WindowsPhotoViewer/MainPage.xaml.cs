using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Popups;
using WindowsPhotoViewer.Common;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using PhotoLoader.Interfaces;
using PhotoLoader.Models;

namespace WindowsPhotoViewer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IRetrievePhotos _photoLoader = new PhotoLoaderSDK.PhotoLoader();
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private String _recentlyAccessedPhotos;
        private const String RecentlyAccessedPhotosTokenName = "falToken";
        private const Int32 MaxRecentAccessListCount = 20;

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            //CreateTestItems();
            //ImagesGrid.ItemsSource = new IncrementalLoadingPhotoCollection1(myItems);
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                if (ApplicationData.Current.LocalSettings == null || !ApplicationData.Current.LocalSettings.Values.Any()
                    || !ApplicationData.Current.LocalSettings.Values.ContainsKey(RecentlyAccessedPhotosTokenName)) return;

                Object value;
                if (!ApplicationData.Current.LocalSettings.Values.TryGetValue(RecentlyAccessedPhotosTokenName, out value)) return;

                _recentlyAccessedPhotos = value as String;
                if (_recentlyAccessedPhotos == null || String.IsNullOrEmpty(_recentlyAccessedPhotos))
                {
                    return;
                }

                var photos = new List<StorageFile>();
                foreach (var accessedPhoto in _recentlyAccessedPhotos.Split(','))
                {
                    try
                    {
                        var file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(accessedPhoto);
                        photos.Add(file);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch{}
                }

                ImagesGrid.ItemsSource = new LazyPhotoLoader(photos.MapToStorePhoto());
                selectedCount.Text  =String.Format("Displaying {0} most recent photo(s)", photos.Count);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            { }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.LandscapeFlipped | DisplayOrientations.Landscape;
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CmbDataSources.SelectedItem == null)
            {
                var dialog = new MessageDialog("Please select a valid photo source", "Invalid selection");
                await dialog.ShowAsync();
                return;
            }

            if (CmbDataSources.SelectedItem.ToString() != "Device Storage")
            {
                var dialog = new MessageDialog("Only 'Device Storage' is supported at this point", "Work in progress...");
                CmbDataSources.SelectedItem = "Device Storage";
                await dialog.ShowAsync();
            }

            var photos = await _photoLoader.RetrievePhotos(PhotoDataSource.LocalStorage);
            var lstPhotos = photos as IList<Photo> ?? photos.ToList();
            ImagesGrid.ItemsSource = new LazyPhotoLoader(lstPhotos.ToList());
            selectedCount.Text = lstPhotos.Count + " photo(s) selected";

            //Currently caches only the 200 most recent photos
            _recentlyAccessedPhotos = "";
            foreach (var photo in lstPhotos.Take(MaxRecentAccessListCount))
            {
                var token = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(photo.StorageFile);
                _recentlyAccessedPhotos += token + ",";
            }

            if (!String.IsNullOrWhiteSpace(_recentlyAccessedPhotos))
            {
                ApplicationData.Current.LocalSettings.Values[RecentlyAccessedPhotosTokenName] = _recentlyAccessedPhotos;
            }
        }

        private void ImagesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!e.AddedItems.Any())
                return;

            var selectedImage = e.AddedItems[e.AddedItems.Count - 1] as ImageItem;
            if (Frame != null && selectedImage != null)
            {
                Frame.Navigate(typeof(PhotoPage), selectedImage);
            }
        }
    }
}
