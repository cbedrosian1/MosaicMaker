using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.Model.Image;
using GroupGMosaicMaker.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupGMosaicMaker.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        #region Data members

        /// <summary>
        ///     The application height
        /// </summary>
        public const int ApplicationHeight = 840;

        /// <summary>
        ///     The application width
        /// </summary>
        public const int ApplicationWidth = 1080;
        

        private readonly StreamFileLoader fileLoader;
        private readonly StreamFolderLoader folderLoader;

        private string chosenFileType;
        private List<string> validFileTypes;
        

        #endregion

        #region Constructors

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size {Width = ApplicationWidth, Height = ApplicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(ApplicationWidth, ApplicationHeight));
            this.chosenFileType = string.Empty;
            this.fileLoader = new StreamFileLoader();
            this.folderLoader = new StreamFolderLoader();
            this.validFileTypes = new List<string> {
                ".jpg",
                ".png",
                ".bmp"
            };
        }

        #endregion

        #region Methods

        private async void loadFile_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                this.chosenFileType = file.FileType;
                var stream = await this.fileLoader.LoadFile(file);
                await ((MainPageViewModel) DataContext).LoadImageSource(stream);
            }
        }

        private async Task<StorageFile> selectSaveImageFile()
        {
            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };
            this.validFileTypes = this.generateFileTypeChoices();
            foreach (var current in this.validFileTypes)
            {
                fileSavePicker.FileTypeChoices.Add(current.Substring(1).ToUpperInvariant() + " files",
                    new List<string> {current});
            }

            var file = await fileSavePicker.PickSaveFileAsync();
            return file;
        }

        private List<string> generateFileTypeChoices()
        {
            var fileTypesForSaving = new List<string> {
                this.chosenFileType
            };
            foreach (var current in this.validFileTypes)
            {
                if (!fileTypesForSaving.Contains(current))
                {
                    fileTypesForSaving.Add(current);
                }
            }

            return fileTypesForSaving;
        }

        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFile = await this.selectSaveImageFile();
            if (saveFile != null)
            {
                await ((MainPageViewModel) DataContext).WriteDataAsync(saveFile);
            }
        }

        private async void loadPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await this.selectPaletteFolderAsync();

            if (folder != null)
            {
                var files = await this.folderLoader.LoadFolder(folder);

                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                await ((MainPageViewModel) DataContext).GeneratePalette(files);
            }
        }

        private async Task<StorageFolder> selectPaletteFolderAsync()
        {
            var folderPicker = new FolderPicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            return folder;
        }

        private void stretchButtonIsSelected(object sender, RoutedEventArgs e)
        {
            this.sourceScrollView.ChangeView(0, this.sourceScrollView.VerticalOffset, 1.0f);
            this.mosaicScrollView.ChangeView(0, this.mosaicScrollView.VerticalOffset, 1.0f);
        }

        #endregion


        //TODO lots of duplicate code here but not really b/c its just a file picker so IDK
        private async void  addImageToPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var stream = await this.fileLoader.LoadFile(file);
                await ((MainPageViewModel) DataContext).AddImageToPalette(stream);
            }
        }

        private void useSelectedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            
                ((MainPageViewModel)DataContext).UpdateSelectedPalette(this.gridView.SelectedItems);

        }

        private void deleteSelectedItemsButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedImages = new ObservableCollection<PaletteImageGenerator>();
            foreach (var current in this.gridView.SelectedItems)
            {
                selectedImages.Add((PaletteImageGenerator)current);
            }
            ((MainPageViewModel)DataContext).DeleteSelectedImages(selectedImages);
        }
        


        private void GridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            ((MainPageViewModel) DataContext).IsUsingSelectedImages = false;
        }

        private void BlackWhiteToggle_OnToggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch blackWhiteToggle)
            {
                ((MainPageViewModel)DataContext).IsBlackWhiteToggled = blackWhiteToggle.IsOn;
                ((MainPageViewModel)DataContext).UpdateMosaicImage();
            }
        }

        private void gridSwitchToggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch gridToggleSwitch)
            {
                ((MainPageViewModel)DataContext).IsGridToggled = gridToggleSwitch.IsOn;
                ((MainPageViewModel)DataContext).UpdateDisplayedImage();
            }
        }
    }
}