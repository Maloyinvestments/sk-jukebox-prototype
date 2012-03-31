using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;
using SkJukeBox_Demo.Models;

namespace SkJukeBox_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int index = 0;
        private int count = 12;
        private ObservableCollection<AlbumDirectory> tempStrings;
        //ObservableCollection<string> strings = new ObservableCollection<string>() { "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", };

        private int keyCount = 0;
        private string inputKey;

        private string albumDirectory;
        private ObservableCollection<AlbumDirectory> albumCollection;
        public MainWindow()
        {
            InitializeComponent(); 
            albumCollection = new ObservableCollection<AlbumDirectory>();
            //tempStrings = new ObservableCollection<string>(strings.Take(count));
            //this.MusicListBox.ItemsSource = tempStrings;
        }

        private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        private void OpenAlbumExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.OpenAlbumDirectoryMethod();
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.D0)
            {
                this.keyCount++;
                inputKey += e.Key;
                inputKey = inputKey.Replace("D", string.Empty);
                if (this.keyCount == 5)
                {
                    System.Windows.MessageBox.Show(this.inputKey);
                    this.keyCount = 0;
                    this.inputKey = string.Empty;
                }
            }
        }

        private void Pre_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (index != 0)
            {
                index = index - count;
                tempStrings = new ObservableCollection<AlbumDirectory>(this.albumCollection.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }

        private void Next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (index < this.albumCollection.Count)
            {
                index = index + count;
                tempStrings = new ObservableCollection<AlbumDirectory>(this.albumCollection.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }

        private void OpenAlbumDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.OpenAlbumDirectoryMethod();
        }

        private void OpenAlbumDirectoryMethod()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Description = @"Please select a folder.";
            folderBrowserDialog.UseDescriptionForTitle = true;
            ////if (this.Solution != null)
            ////{
            ////    if (!string.IsNullOrEmpty(this.ParentViewModel.CodeGenProject.DirectoryManager.ProjectLocation))
            ////    {
            ////        folderBrowserDialog.SelectedPath = this.ParentViewModel.CodeGenProject.DirectoryManager.ProjectLocation;
            ////    }
            ////}
            ////else
            ////{
            ////    this.Solution = new Solution();
            ////}
            if ((bool)folderBrowserDialog.ShowDialog())
            {
                this.albumDirectory = folderBrowserDialog.SelectedPath;
                AlbumDirectory album = new AlbumDirectory(this.albumDirectory);
                this.AddAlbummToCollection(album);
                this.MusicListBox.ItemsSource = this.albumCollection;
            }
        }

        private void AddAlbummToCollection(AlbumDirectory albumDirectory)
        {
            AlbumDirectory album = null;
            if (this.albumCollection.Count == 0)
            {
                albumDirectory.AlbumNo = "0001";
                this.albumCollection.Add(albumDirectory);
                do
                {
                    albumCollection.Add(album);
                } while (albumCollection.Count < 12);
            }
            else
            {
                int currentIndex = 0;
                for (int i = 0; i < this.albumCollection.Count; i++)
                {
                    var albumD = this.albumCollection[i];
                    if (albumD == null)
                    {
                        albumDirectory.AlbumNo = string.Format("000{0}", i + 1);
                        albumCollection.Insert(currentIndex, albumDirectory);
                        break;
                    }

                    this.index++;
                }
            }
        }
    }
}
