using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WallBox;

namespace WallboxWpfLoader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int workT = 0;
        int AT = 0;
        
        bool Loading = false;
        int currentPage = 1;
        List<int> pages = new List<int>();
        ObservableCollection<WallBox.DataModel.ImageModel> imageModels = new ObservableCollection<WallBox.DataModel.ImageModel>();
        public MainWindow()
        {
            InitializeComponent();
            SaveImages.Visibility = Visibility.Collapsed;
            FinishExitButton.Visibility = Visibility.Collapsed;

            this.PreviewListView.ItemsSource = imageModels;
            this.CategoriesListBox.Items.Add(new WallBox.DataModel.CategoryModel()
            {
                Title = "Все",
                Url = ""

            });
            LoadCategories();
        }
        public void LoadCategories()
        {
            var TaskAwaiter = WallBoxApi.GetCategoriesAsync().GetAwaiter();
            TaskAwaiter.OnCompleted(() =>
            {
                var data = TaskAwaiter.GetResult();
                foreach (WallBox.DataModel.CategoryModel categoryModel in data)
                    CategoriesListBox.Items.Add(categoryModel);




            });
        }
        public void LoadNexPage()
        {
           
            if ((currentPage + 1) < pages.Last())
            {
                currentPage += 1;
                var PreViewAwaiter = WallBoxApi.GetCategoryPageData(((WallBox.DataModel.CategoryModel)CategoriesListBox.SelectedItem).Url, currentPage).GetAwaiter();
                PreViewAwaiter.OnCompleted(() =>
                {
                    var data = PreViewAwaiter.GetResult();
                    pages = data.Item2;

                    foreach (var previewModel in data.Item1)
                        imageModels.Add(previewModel);

                    if (AllCheckbox.IsChecked == true)
                        PreviewListView.SelectAll();

                    Loading = false;

                });
            }
        }


        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentPage = 1;
            imageModels.Clear();
            GC.Collect(0, GCCollectionMode.Optimized);
            GC.Collect(1, GCCollectionMode.Optimized);
            GC.Collect(2, GCCollectionMode.Optimized);
            GC.Collect(3, GCCollectionMode.Optimized);
            var Lbox = sender as System.Windows.Controls.ListBox;

            var PreViewAwaiter = WallBox.WallBoxApi.GetCategoryPageData(((WallBox.DataModel.CategoryModel)Lbox.SelectedItem).Url, 1).GetAwaiter();
            PreViewAwaiter.OnCompleted(() =>
            {
                var data = PreViewAwaiter.GetResult();
                pages = data.Item2;
                foreach (var previewModel in data.Item1)
                    imageModels.Add(previewModel);

                if (AllCheckbox.IsChecked == true)
                    PreviewListView.SelectAll();



            });
        }

        private void FinishExitButton_Click(object sender, RoutedEventArgs e)
        {
            AllCheckbox.IsChecked = false;
            ThreadPool.SetMaxThreads(workT, AT);
            DownloadProgressTextBlock.Text = "";
            DownloadProgressBar.Value = 0;
            DownloadProgressBar.Maximum = 0;
            PreviewListView.SelectedItems.Clear();
            FinishExitButton.Visibility = Visibility.Collapsed;
            DownloadPanel.Visibility = Visibility.Collapsed;
        }

        private void AllCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            PreviewListView.SelectAll();
        }

        private void PreviewListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PreviewListView.SelectedItems.Count > 0)
            {
                SaveImages.Visibility = Visibility.Visible;
            }
            else SaveImages.Visibility = Visibility.Collapsed;
            

        }

        private async void SaveImages_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            if (folderBrowserDialog.ShowDialog()==System.Windows.Forms.DialogResult.OK)
            {
                DownloadPanel.Visibility = Visibility.Visible;


                List<WallBox.DataModel.ImageModel> imageObjects = new List<WallBox.DataModel.ImageModel>();
              await  this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
              {

                  foreach (WallBox.DataModel.ImageModel k in PreviewListView.SelectedItems)
                  {
                      imageObjects.Add(k);
                  }
                  DownloadProgressBar.Maximum = imageObjects.Count;
                  DownloadProgressTextBlock.Text = "Загруженно " + DownloadProgressBar.Value + " из " + DownloadProgressBar.Maximum;
              }));

                foreach (WallBox.DataModel.ImageModel k in imageObjects)
                {
                    Debug.WriteLine("starts");
                    ThreadPool.GetMaxThreads(out workT, out AT);
                    ThreadPool.SetMinThreads(1, 1);
                    ThreadPool.SetMaxThreads(10, 10);
                    ThreadPool.QueueUserWorkItem(
                            new WaitCallback(delegate (object state)
                            {
                                sAsync(folderBrowserDialog.SelectedPath, k);
                            }), null);
                }
            }
        }

        private async Task sAsync(string folder, WallBox.DataModel.ImageModel imageobject)
        {
            try
            {
                var ImageUrl = await WallBoxApi.GetImageUrlAsync(imageobject);
                Debug.WriteLine(ImageUrl);
                var sp = ImageUrl.Split('/');
               
                new WebClient().DownloadFile(ImageUrl, folder+"\\"+sp[sp.Length - 2] + sp[sp.Length - 1]);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            await this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {

                DownloadProgressBar.Value++;
                DownloadProgressTextBlock.Text = "Загруженно " + DownloadProgressBar.Value + " из " + DownloadProgressBar.Maximum;
            }));
            await Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background,new Action(() =>
            {
                if (DownloadProgressBar.Value == DownloadProgressBar.Maximum)
                {
                    GC.Collect(0, GCCollectionMode.Optimized);
                    GC.Collect(1, GCCollectionMode.Optimized);
                    GC.Collect(2, GCCollectionMode.Optimized);
                    GC.Collect(3, GCCollectionMode.Optimized);

                    DownloadProgressTextBlock.Text = "Загрузка завершена";
                    FinishExitButton.Visibility = Visibility.Visible;


                }
            }));
        }


        private void PreviewListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {

                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    
                    if (!Loading)
                        LoadNexPage();

                }
            }
        }

        private void BitmapImage_DownloadCompleted(object sender, EventArgs e)
        {

        }

        private void VirtualizingWrapPanel_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalChange > 0)
            {
                
                if (e.VerticalOffset + e.ViewportHeight == e.ExtentHeight)
                {
                    if (!Loading)
                    {
                        Loading = true;
                        LoadNexPage();
                    }

                }
            }
        }

        private void AllCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            PreviewListView.UnselectAll();
        }
    }
}
