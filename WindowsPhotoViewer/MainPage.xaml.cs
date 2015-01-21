using System.Collections.Generic;
using System.Threading.Tasks;
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
        private const String TokenNameRecentlyAccessed = "falToken";
        private const String TokenNameSelectedPhotos = "falSelectedToken";
        private const Int32 MaxRecentAccessListCount = 20;
        private IList<Photo> SelectedPhotos; 

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
                //First try loading data from current session
                SelectedPhotos = GetPhotosFromSession(e);

                if (SelectedPhotos.Any())
                {
                    selectedCount.Text = SelectedPhotos.Count + " photo(s) selected";
                    return;
                }

                //If there was no session data, load data from database
                var photos = await GetRecentPhotos();
                SelectedPhotos = photos.MapToStorePhoto();
                selectedCount.Text = String.Format("Displaying {0} most recent photo(s)", photos.Count);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            { }
            finally
            {
                SelectedPhotos = SelectedPhotos ?? new List<Photo>();
                ImagesGrid.ItemsSource = new LazyPhotoLoader(SelectedPhotos);
            }
            
        }

        private IList<Photo> GetPhotosFromSession(LoadStateEventArgs e)
        {
            var results = new List<Photo>();
            if (e.PageState == null || !e.PageState.ContainsKey(TokenNameSelectedPhotos)) return results;
            
            Object value;
            if (!e.PageState.TryGetValue(TokenNameSelectedPhotos, out value)) return results;
            
            var previouslySelectedPhotos = value as IList<Photo>;
            return previouslySelectedPhotos ?? results;
        }

        private async Task<List<StorageFile>> GetRecentPhotos()
        {
            var photos = new List<StorageFile>();
            try
            {
                if (ApplicationData.Current.LocalSettings == null || !ApplicationData.Current.LocalSettings.Values.Any()
                    || !ApplicationData.Current.LocalSettings.Values.ContainsKey(TokenNameRecentlyAccessed)) return photos;

                Object value;
                if (!ApplicationData.Current.LocalSettings.Values.TryGetValue(TokenNameRecentlyAccessed, out value)) return photos;

                var recentPhotos = value as String;
                if (recentPhotos == null || String.IsNullOrEmpty(recentPhotos))
                {
                    return photos;
                }

                foreach (var recentPhoto in recentPhotos.Split(','))
                {
                    try
                    {
                        var file = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(recentPhoto);
                        photos.Add(file);
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch{}
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            { }

            return photos;
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
            if (SelectedPhotos != null && SelectedPhotos.Any())
            {
                e.PageState[TokenNameSelectedPhotos] = SelectedPhotos;
            }
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
            SelectedPhotos = photos as IList<Photo> ?? photos.ToList();
            ImagesGrid.ItemsSource = new LazyPhotoLoader(SelectedPhotos.ToList());
            selectedCount.Text = SelectedPhotos.Count + " photo(s) selected";

            SaveRecentlyAccesedPhotos(SelectedPhotos);
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

        #region Storage Helpers
        
        /// <summary>
        /// Cache recent data to display when the App re-opens
        /// </summary>
        /// <param name="lstPhotos"></param>
        private void SaveRecentlyAccesedPhotos(IEnumerable<Photo> lstPhotos)
        {
            var recentlyAccessedPhotos = new List<String>();
            foreach (var photo in lstPhotos.Take(MaxRecentAccessListCount))
            {
                var token = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(photo.StorageFile);
                recentlyAccessedPhotos.Add(token);
            }

            if (!recentlyAccessedPhotos.Any()) return;

            var localstorage = recentlyAccessedPhotos.Take(MaxRecentAccessListCount)
                .Aggregate("", (current, recentlyAccessedPhoto) => current + (recentlyAccessedPhoto + ","));
            ApplicationData.Current.LocalSettings.Values[TokenNameRecentlyAccessed] = localstorage;
        }

        #endregion
    }
}
