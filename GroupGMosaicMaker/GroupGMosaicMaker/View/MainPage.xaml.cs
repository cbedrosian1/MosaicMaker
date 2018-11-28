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
        public const int ApplicationWidth = 1200;

        #endregion

        #region Constructors

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size {Width = ApplicationWidth, Height = ApplicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(ApplicationWidth, ApplicationHeight));
        }

        #endregion

        #region Methods

        private async void loadFile_Click(object sender, RoutedEventArgs e)
        {
            var sourceImageFile = await this.selectSourceImageFile();

            if (sourceImageFile != null)
            {
                // TODO Consider moving this to DataTier to improve separation of concerns. It is small though, so not sure if it's even worth it.
                using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
                {
                    
                        await ((MainPageViewModel)DataContext).CreateImages(fileStream);
                    
                    


                }
            }
        }

        private async Task<StorageFile> selectSourceImageFile()
        {
            var openPicker = new FileOpenPicker {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            var file = await openPicker.PickSingleFileAsync();

            return file;
        }

        private async Task<StorageFile> selectSaveImageFile()
        {
            var fileSavePicker = new FileSavePicker {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = "image"
            };

            fileSavePicker.FileTypeChoices.Add("PNG files", new List<string> {".png"});

            var file = await fileSavePicker.PickSaveFileAsync();

            return file;
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
            var saveFile = await this.selectSaveImageFile();

            if (saveFile != null)
            { 
                await ((MainPageViewModel) DataContext).WriteDataAsync(saveFile);
            }
        }


        #endregion

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
    }
}