using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.ViewModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GroupGMosaicMaker.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Data members

        /// <summary>
        ///     The application height
        /// </summary>
        public const int ApplicationHeight = 700;

        /// <summary>
        ///     The application width
        /// </summary>
        public const int ApplicationWidth = 1030;

        private StreamFileLoader fileLoader;
        private StreamFolderLoader folderLoader;

        private string chosenFileType;
        private List<string> validFileTypes;

        #endregion
        #region Constructors

        public MainPage()
        {
            InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size {Width = ApplicationWidth, Height = ApplicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(ApplicationWidth, ApplicationHeight));
            this.chosenFileType = string.Empty;
            this.fileLoader = new StreamFileLoader();
            this.folderLoader = new StreamFolderLoader();
            this.validFileTypes = new List<string>()
            {
                ".jpg",
                ".png",
                ".bmp"
            };

        }

        #endregion



        #region Methods

        private async void loadFile_Click(object sender, RoutedEventArgs e)
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
                this.chosenFileType = file.FileType;
                var stream = await this.fileLoader.LoadFile(file);
                await ((MainPageViewModel) this.DataContext).CreateImages(stream);
            }
        }

        private async Task<StorageFile> selectSaveImageFile()
        {
            var fileSavePicker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image",    
                
            };
            this.validFileTypes = generateFileTypeChoices();
            foreach (var current in this.validFileTypes)
            {
                fileSavePicker.FileTypeChoices.Add(current.Substring(1).ToUpperInvariant() + " files", new List<string>{current});
            }
            var file = await fileSavePicker.PickSaveFileAsync();
            return file;
        }

        private List<string> generateFileTypeChoices()
        {
            var fileTypesForSaving = new List<string>()
            {
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

        private async Task<BitmapImage> MakeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        private async void saveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFile = await selectSaveImageFile();

            if (saveFile != null) await ((MainPageViewModel) DataContext).WriteDataAsync(saveFile);
        }

        private void gridSwitchToggled(object sender, RoutedEventArgs e)
        {
            var gridToggle = sender as ToggleSwitch;
            if (gridToggle != null)
            {
                if (gridToggle.IsOn == true)
                {
                    ((MainPageViewModel) DataContext).DisplayedImage = ((MainPageViewModel) DataContext).GridImage;
                    ((MainPageViewModel) DataContext).IsGridToggled = true;
                }
                else
                {
                    ((MainPageViewModel) DataContext).DisplayedImage = ((MainPageViewModel) DataContext).OriginalImage;
                    ((MainPageViewModel) DataContext).IsGridToggled = false;
                }
            } 
         }

        private async void loadPaletteButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = await this.selectPaletteFolderAsync();


            if (folder != null)
            {
                var files = await this.folderLoader.LoadFolder(folder);

                Windows.Storage.AccessCache.StorageApplicationPermissions.
                    FutureAccessList.AddOrReplace("PickedFolderToken", folder);

                await ((MainPageViewModel) DataContext).GeneratePalette(files);
            }
        }

        private async Task<StorageFolder> selectPaletteFolderAsync()
        {
            var folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add("*");

            var folder = await folderPicker.PickSingleFolderAsync();
            return folder;
        }
         
        #endregion

       
    }
}