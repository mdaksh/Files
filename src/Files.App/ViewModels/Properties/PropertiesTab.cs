using Files.App.DataModels.NavigationControlItems;
using Files.App.Filesystem;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Files.App.ViewModels.Properties
{
    public abstract class PropertiesTab : Page, IDisposable
    {
        public IShellPage AppInstance = null;

        public BaseProperties BaseProperties { get; set; }

        public SelectedItemsPropertiesViewModel ViewModel { get; set; }

        protected IProgress<float> hashProgress;

        protected virtual void Properties_Loaded(object sender, RoutedEventArgs e)
        {
            if (BaseProperties != null)
            {
                BaseProperties.GetSpecialProperties();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var np = e.Parameter as Views.Properties.PropertyNavParam;

            AppInstance = np.AppInstanceArgument;
            ViewModel = new SelectedItemsPropertiesViewModel();

            if (np.navParameter is LibraryItem library)
            {
                BaseProperties = new LibraryProperties(ViewModel, np.tokenSource, DispatcherQueue, library, AppInstance);
            }
            else if (np.navParameter is ListedItem item)
            {
                if (item.PrimaryItemAttribute == StorageItemTypes.File || item.IsZipItem)
                {
                    BaseProperties = new FileProperties(ViewModel, np.tokenSource, DispatcherQueue, hashProgress, item, AppInstance);
                }
                else if (item.PrimaryItemAttribute == StorageItemTypes.Folder)
                {
                    BaseProperties = new FolderProperties(ViewModel, np.tokenSource, DispatcherQueue, item, AppInstance);
                }
            }
            else if (np.navParameter is List<ListedItem> items)
            {
                BaseProperties = new CombinedProperties(ViewModel, np.tokenSource, DispatcherQueue, items, AppInstance);
            }
            else if (np.navParameter is DriveItem drive)
            {
                BaseProperties = new DriveProperties(ViewModel, drive, AppInstance);
            }

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (BaseProperties != null && BaseProperties.TokenSource != null)
            {
                //BaseProperties.TokenSource.Cancel();
            }

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Tries to save changed properties to file.
        /// </summary>
        /// <returns>Returns true if properties have been saved successfully.</returns>
        public abstract Task<bool> SaveChangesAsync(ListedItem item);

        public abstract void Dispose();
    }
}