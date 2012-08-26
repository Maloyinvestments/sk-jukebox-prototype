using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Ookii.Dialogs.Wpf;
using SkJukeBox_Demo.Models;
using SkJukeBox_Demo.Utility;
using MenuItem = System.Windows.Controls.MenuItem;
using MessageBox = System.Windows.MessageBox;
using Track = SkJukeBox_Demo.Models.Track;

namespace SkJukeBox_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int index = 0;
        private int count = 12;
        private ObservableCollection<AlbumDirectory> topAlbumList;
        private int keyCount6Digit = 0;
        private string inputKey6Digit;

        private string albumDirectory;
        private ObservableCollection<AlbumDirectory> albumCollection;

        private string currentGenre;
        private ObservableCollection<Track> trackList = new ObservableCollection<Track>();
        private Track currentTrack;
        private Timer _timer = new Timer();
        private int tick = 0;
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += new System.EventHandler(MainWindow_Closed);
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
            this.RatingAlbum.Visibility = System.Windows.Visibility.Visible;
            this.RatingAlbum.ValueChanged += new RoutedPropertyChangedEventHandler<double?>(RatingAlbum_ValueChanged);
            this.MyMediaElement.Volume = 0.5;
            this.VolumeTextBlock.Text = "Volume 50%";
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(_timer_Tick);

        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if (tick == 2)
            {
                popup1.IsOpen = false;
                tick = 0;
                _timer.Stop();
            }
            tick++;
        }

        void RatingAlbum_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (this.PlayGrid.DataContext != null)
            {
                (this.PlayGrid.DataContext as AlbumDirectory).Rating = e.NewValue;
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            albumCollection = new ObservableCollection<AlbumDirectory>();
            string recentAlbumsFilePath = Directory.GetCurrentDirectory() + @"\Albums.xml";
            if (File.Exists(recentAlbumsFilePath))
            {
                var albums = Serializer<Albums>.DeserializeFromFile(recentAlbumsFilePath);

                this.albumCollection = new ObservableCollection<AlbumDirectory>(albums.AlbumDirectories);
                if (albums.TrackList != null && albums.TrackList.Count > 0)
                {
                    this.trackList = new ObservableCollection<Track>(albums.TrackList);
                    this.currentTrack = this.trackList[0];
                    var albumD =
                        this.albumCollection.SingleOrDefault(
                            x => x != null && x.IsNotNull && x.AlbumNo == StringHelper.Left(this.currentTrack.TrackNo, 4));

                    this.NavigateToSelectedAlbumAndSong(albumD);
                    this.BindingToAlbumSelected(albumD);
                }
                else
                {
                    this.MusicListBox.ItemsSource = albumCollection.Take(12);
                }

                this.TrackListBox.ItemsSource = this.trackList;
                this.AddToCollectionTopAlbumByGenre("Pop");
            }
        }

        private void AddToCollectionTopAlbumByGenre(string genreName)
        {
            if (this.currentGenre != genreName)
            {
                this.currentGenre = genreName;
                this.GenreTextBlock.Text = genreName;
                this.TopTextBlock.Text = "Top 10 " + genreName;
                var popAlbumCollection =
                    this.albumCollection.Where(x => x != null && x.Genre == genreName).OrderByDescending(x => x.Rating).
                        ToList();
                topAlbumList = new ObservableCollection<AlbumDirectory>(popAlbumCollection);
                for (int i = 0; i < popAlbumCollection.Count; i++)
                {
                    var directory = popAlbumCollection[i];
                    if (directory != null)
                    {
                        directory.Rank = i + 1;
                    }
                }

                int countNotNull = this.albumCollection.Where(x => x != null).Count();
                if (countNotNull > 0)
                {
                    int random = (new Random()).Next(countNotNull);
                    var collection = new ObservableCollection<AlbumDirectory>() { this.albumCollection[random] };
                    this.TopAlbumListBox.ItemsSource = collection;
                    this.TopAlbumListBox.SelectedItem = this.TopAlbumListBox.Items[0];
                }

                this.TopListBox.ItemsSource = topAlbumList;
            }
        }

        void MainWindow_Closed(object sender, System.EventArgs e)
        {
            this.SaveToXml();
        }

        void SaveToXml()
        {
            string recentAlbumsFilePath = Directory.GetCurrentDirectory() + @"\Albums.xml";
            Albums albums = new Albums();
            albums.AlbumDirectories = this.albumCollection.ToList();
            //albums.AlbumDirectories = new List<Album>() { new Album() { AlbumName = "aaaa" }, new Album() { AlbumName = "bbbbb" } };
            albums.TrackList = new List<Track>(this.trackList.ToList());
            Serializer<Albums>.SerializeToFile(
                albums,
                recentAlbumsFilePath);
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

        private void BackspaceExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.DetailAlbumGrid.Visibility == System.Windows.Visibility.Visible)
            {
                this.DetailAlbumGrid.Visibility = System.Windows.Visibility.Collapsed;
                this.ListAlbumGrid.Visibility = System.Windows.Visibility.Visible;
                this.InputTextBox.Text = null;
                this.inputKey6Digit = string.Empty;
                this.keyCount6Digit = 0;
            }
            else
            {
                if (string.IsNullOrEmpty(this.InputTextBox.Text))
                {
                    ////this.MusicListBox.SelectedItem = null;
                    ////this.PlayGrid.DataContext = null;
                    ////this.SongListBox.ItemsSource = new ObservableCollection<Song>();
                    this.NextExecuted(null, null);
                }
                else
                {
                    this.InputTextBox.Text = null;
                    this.inputKey6Digit = string.Empty;
                    this.keyCount6Digit = 0;
                }
            }
            e.Handled = true;
        }

        private void SelectExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (inputKey6Digit != null && inputKey6Digit.Length == 6)
            {
                this.SongListBox_MouseDoubleClick(null, null);
            }

            e.Handled = true;
        }

        private void NextExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.currentTrack != null && this.trackList.Count > 1)
            {
                this.MyMediaElement.Stop();
                this.trackList.Remove(this.currentTrack);
                if (this.MusicListBox.SelectedItem != null)
                {
                    var albumD = this.albumCollection.SingleOrDefault(
                        x => x != null && x.IsNotNull && x.AlbumNo == StringHelper.Left(this.currentTrack.TrackNo, 4));
                    ;
                    ////albumD.IsPlaying = false;
                    this.MusicListBox.Items.Refresh();
                }

                if (this.trackList.Count > 0)
                {
                    this.currentTrack = this.trackList[0];
                    this.MusicListBox.SelectedItem =
                        this.albumCollection.SingleOrDefault(

                        x => x != null && x.IsNotNull && x.AlbumNo == StringHelper.Left(this.currentTrack.TrackNo, 4));

                    //this.SongListBox.SelectedItem =
                    //    (this.MusicListBox.SelectedItem as AlbumDirectory).SongCollection.SingleOrDefault(
                    //        x => x.Track == Int32.Parse(StringHelper.Right(this.currentTrack.TrackNo, 2)));
                    var song = (this.MusicListBox.SelectedItem as AlbumDirectory).SongCollection.SingleOrDefault(
                            x => x.Track == Int32.Parse(StringHelper.Right(this.currentTrack.TrackNo, 2)));
                    this.MyMediaElement.Source = new Uri(song.FileName);
                    this.PlayingSongTextBlock.Text = song.Title;
                    this.MyMediaElement.Play();
                    ////foreach (var directory in this.albumCollection)
                    ////{
                    ////    if (directory != null)
                    ////    {
                    ////        directory.IsPlaying = false;
                    ////    }
                    ////}

                    ////(this.MusicListBox.SelectedItem as AlbumDirectory).IsPlaying = true;
                    this.MusicListBox.Items.Refresh();
                    this.NavigateToSelectedAlbum(this.MusicListBox.SelectedItem as AlbumDirectory);
                    this.BindingToAlbumSelected(this.MusicListBox.SelectedItem as AlbumDirectory);
                }
            }

            if (e != null)
            {
                e.Handled = true;
            }
        }

        private void IncreaseVolumeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.MyMediaElement.Volume <= 0.9)
            {
                this.MyMediaElement.Volume += 0.1;
                this.VolumeTextBlock.Text = string.Format("Volume {0}0%", this.MyMediaElement.Volume * 10);
            }
            e.Handled = true;
        }

        private void DecreaseVolumeExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.MyMediaElement.Volume >= 0.2)
            {
                this.MyMediaElement.Volume -= 0.1;
                this.VolumeTextBlock.Text = string.Format("Volume {0}0%", this.MyMediaElement.Volume * 10);
            }
            else if (this.MyMediaElement.Volume >= 0.1 && this.MyMediaElement.Volume <= 0.2)
            {
                this.MyMediaElement.Volume = 0.0;
                this.VolumeTextBlock.Text = string.Format("Volume 0%");
            }

            e.Handled = true;
        }

        private void NextAlbumExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Next_MouseDown(null, null);

            e.Handled = true;
        }

        private void PreviousAlbumExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Pre_MouseDown(null, null);

            e.Handled = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.D0)
            {
                this.keyCount6Digit++;
                if (this.keyCount6Digit % 6 == 1)
                {
                    this.keyCount6Digit = 1;
                    this.inputKey6Digit = string.Empty;
                }

                this.inputKey6Digit += e.Key;
                inputKey6Digit = inputKey6Digit.Replace("D", string.Empty);
                this.InputTextBox.Text = inputKey6Digit;
                if (this.keyCount6Digit == 4)
                {
                    //System.Windows.MessageBox.Show(this.inputKey6Digit);
                    var albumDirectory = this.albumCollection.SingleOrDefault(x => x != null && x.AlbumNo == inputKey6Digit);
                    if (albumDirectory != null)
                    {
                        this.MusicListBox.SelectedItem = albumDirectory;
                        this.DetailAlbumGrid.Visibility = System.Windows.Visibility.Visible;
                        this.ListAlbumGrid.Visibility = System.Windows.Visibility.Collapsed;
                        this.NavigateToSelectedAlbum(albumDirectory);
                        this.SongListBox.ItemsSource = albumDirectory.SongCollection;
                    }
                }

                if (this.keyCount6Digit == 6)
                {
                    //System.Windows.MessageBox.Show(this.inputKey6Digit);
                    if (this.MusicListBox.SelectedItem != null)
                    {
                        int track = int.Parse(this.inputKey6Digit.Substring(4, 2));
                        this.SongListBox.SelectedItem =
                            (this.MusicListBox.SelectedItem as AlbumDirectory).SongCollection.SingleOrDefault(
                                x => x.Track == track);
                        this.SongListBox.ScrollIntoView(this.SongListBox.SelectedItem);
                    }
                    ////this.keyCount6Digit = 0;
                    ////this.inputKey6Digit = string.Empty;
                }
            }
            else if (e.Key == Key.Right)
            {
                this.NextExecuted(null, null);
            }
            else if (e.Key == Key.G)
            {
                this.MainContextMenu.IsOpen = true;
            }
        }

        private void Pre_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DetailAlbumGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                if (index != 0)
                {
                    index = index - count;
                    this.MusicListBox.ItemsSource = this.albumCollection.Skip(index).Take(count);
                }
            }
        }

        private void Next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.DetailAlbumGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                if (index < this.albumCollection.Count)
                {
                    index = index + count;
                    var directories = this.albumCollection.Skip(index).Take(count);
                    var i = directories.Where(x => x != null).ToList().Count;
                    if (i > 0)
                    {
                        this.MusicListBox.ItemsSource = directories;
                    }
                    else
                    {
                        index = index - count;
                    }
                }
            }
        }

        private void OpenAlbumDirectory_Click(object sender, RoutedEventArgs e)
        {
            this.OpenAlbumDirectoryMethod();
        }

        private void OpenAlbumDirectoryMethod()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            folderBrowserDialog.Description = @"Please select a root folder.";
            folderBrowserDialog.UseDescriptionForTitle = true;
            if ((bool)folderBrowserDialog.ShowDialog())
            {
                this.albumDirectory = folderBrowserDialog.SelectedPath;
                int numAlbums = 0;
                DirectoryInfo d = new DirectoryInfo(this.albumDirectory);
                foreach (DirectoryInfo subd in d.GetDirectories())
                {
                    var attributes = subd.Attributes;
                    AlbumDirectory album = new AlbumDirectory(subd.FullName);
                    if (album.SongCollection.Count > 0 && subd.FullName.Contains("-") && string.IsNullOrEmpty(subd.Extension))
                    {
                        numAlbums++;
                        album.DefaultAlbumArt = subd.FullName + @"\portada.jpg";
                        album.AlbumName = subd.FullName.Substring(subd.FullName.IndexOf("-") + 1);
                        var artistAndAlbumName = subd.FullName.Substring(subd.FullName.LastIndexOf("\\")).Replace("\\", string.Empty);
                        album.ArtistName = artistAndAlbumName.Replace(album.AlbumName, string.Empty).Replace("-", string.Empty);
                        album.SongCollection = new ObservableCollection<Song>(album.SongCollection.OrderBy(x => x.Track));
                        this.AddAlbummToCollection(album);
                    }
                    ////else
                    ////{
                    ////    MessageBox.Show(string.Format("Cannot find any music file on directory {0} or this is a occult folder.", subd.FullName), "SK Jukebox WPF",
                    ////                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    ////}
                }

                this.SaveToXml();
                MessageBox.Show(string.Format("Found {0} albums.", numAlbums), "SK Jukebox WPF",
                                       MessageBoxButton.OK, MessageBoxImage.Warning);
                this.MusicListBox.ItemsSource = this.albumCollection.Take(count);
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
                        albumDirectory.AlbumNo = string.Format("{0:0000}", i + 1);
                        albumCollection.Insert(currentIndex, albumDirectory);
                        break;
                    }

                    currentIndex++;
                }
            }
        }

        private void GenreChangeClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            switch (menuItem.Header.ToString())
            {
                case "Pop":
                    {
                        this.SetGenreForSelectedAlbum("Pop");
                    }
                    break;
                case "Rock":
                    {
                        this.SetGenreForSelectedAlbum("Rock");
                    }
                    break;
                case "RnB":
                    {
                        this.SetGenreForSelectedAlbum("RnB");
                    }
                    break;
                case "HipHop":
                    {
                        this.SetGenreForSelectedAlbum("HipHop");
                    }
                    break;
            }
        }

        private void SetGenreForSelectedAlbum(string genre)
        {
            if (this.MusicListBox.SelectedItem != null)
            {
                (this.MusicListBox.SelectedItem as AlbumDirectory).Genre = genre;
                ////var albumDirectory = this.albumCollection.SingleOrDefault(
                ////       x => x != null && x.IsNotNull && x.AlbumName == (this.MusicListBox.SelectedItem as AlbumDirectory).AlbumName);
                ////albumDirectory.Genre = genre;
                this.MusicListBox.Items.Refresh();
            }
        }

        private void SongListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var song = this.SongListBox.SelectedItem as Song;
            var album = this.MusicListBox.SelectedItem as AlbumDirectory;
            string index;
            if (song.Track > 0 && song.Track < 10)
            {
                index = album.AlbumNo + "0" + song.Track.ToString();
            }
            else
            {
                index = album.AlbumNo + "0" + song.Track.ToString();
            }


            // Ad all track by 00 track
            if (index == (album.AlbumNo + "00"))
            {
                if (this.trackList.Count == 0)
                {
                    var song1 = album.SongCollection.SingleOrDefault(x => x.Track == 1);
                    this.MyMediaElement.Source = new Uri(song1.FileName);
                    this.PlayingSongTextBlock.Text = song1.Title;
                    this.MyMediaElement.Play();
                    ////(this.MusicListBox.SelectedItem as AlbumDirectory).IsPlaying = true;
                    this.MusicListBox.Items.Refresh();
                    this.currentTrack = new Track()
                                            {
                                                TrackNo = album.AlbumNo + "01",
                                                Album = (this.MusicListBox.SelectedItem as AlbumDirectory).AlbumName,
                                                Artist = (this.MusicListBox.SelectedItem as AlbumDirectory).ArtistName,
                                                SongName = song1.Title
                                            };
                }

                foreach (var song1 in album.SongCollection)
                {
                    if (song1.Track != 0)
                    {
                        var track1 = new Track()
                        {
                            Album = (this.MusicListBox.SelectedItem as AlbumDirectory).AlbumName,
                            Artist = (this.MusicListBox.SelectedItem as AlbumDirectory).ArtistName,
                            SongName = song1.Title
                        };
                        string num;
                        if (song.Track > 0 && song.Track < 10)
                        {
                            num = album.AlbumNo + "0" + song1.Track.ToString();
                        }
                        else
                        {
                            num = album.AlbumNo + "0" + song1.Track.ToString();
                        }

                        track1.TrackNo = num;
                        this.trackList.Add(track1);
                    }
                }

                PopupTextBlock.Text = string.Format("Add {0} Tracks to TrackList completed", album.SongCollection.Count - 1);
            }
            else
            {
                if (this.trackList.Count == 0)
                {
                    this.MyMediaElement.Source = new Uri(song.FileName);
                    this.PlayingSongTextBlock.Text = song.Title;
                    this.MyMediaElement.Play();
                    ////foreach (var directory in this.albumCollection)
                    ////{
                    ////    if (directory != null)
                    ////    {
                    ////        directory.IsPlaying = false;
                    ////    }
                    ////}

                    ////(this.MusicListBox.SelectedItem as AlbumDirectory).IsPlaying = true;
                    this.MusicListBox.Items.Refresh();
                    this.currentTrack = new Track()
                                            {
                                                TrackNo = index,
                                                Album = (this.MusicListBox.SelectedItem as AlbumDirectory).AlbumName,
                                                Artist = (this.MusicListBox.SelectedItem as AlbumDirectory).ArtistName,
                                                SongName = song.Title
                                            };
                }

                this.trackList.Add(new Track()
                    {
                        TrackNo = index,
                        Album = (this.MusicListBox.SelectedItem as AlbumDirectory).AlbumName,
                        Artist = (this.MusicListBox.SelectedItem as AlbumDirectory).ArtistName,
                        SongName = song.Title
                    });
                PopupTextBlock.Text = string.Format("Add {0} to TrackList completed", index);
            }

            if (this.PlayGrid.DataContext == null)
            {
                this.BindingToAlbumSelected(album);
            }

            if (popup1.IsOpen == false)
            {
                popup1.IsOpen = true;
                _timer.Start();
            }

            this.TrackListBox.ItemsSource = this.trackList;
        }

        private void MyMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.trackList.Remove(this.currentTrack);
            ////if (this.MusicListBox.SelectedItem != null)
            ////{
            ////    var albumD = this.albumCollection.SingleOrDefault(
            ////            x => x != null && x.AlbumNo == StringHelper.Left(this.currentTrack, 4)); ;
            ////    albumD.IsPlaying = false;
            ////    this.MusicListBox.Items.Refresh();
            ////}

            if (this.trackList.Count > 0)
            {
                this.currentTrack = this.trackList[0];
                var albumDirectory =
                        this.albumCollection.SingleOrDefault(
                            x => x != null && x.IsNotNull && x.AlbumNo == StringHelper.Left(this.currentTrack.TrackNo, 4));
                var song = albumDirectory.SongCollection.SingleOrDefault(
                                x => x.Track == Int32.Parse(StringHelper.Right(this.currentTrack.TrackNo, 2)));
                this.MyMediaElement.Source = new Uri(song.FileName);
                this.PlayingSongTextBlock.Text = song.Title;
                this.MyMediaElement.Play();
                ////foreach (var directory in this.albumCollection)
                ////{
                ////    if (directory != null)
                ////    {
                ////        directory.IsPlaying = false;
                ////    }
                ////}

                ////(this.Mu5sicListBox.SelectedItem as AlbumDirectory).IsPlaying = true;
                ////this.MusicListBox.Items.Refresh();
                this.NavigateToSelectedAlbum(albumDirectory);
                this.BindingToAlbumSelected(albumDirectory);

            }
        }

        private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.MyMediaElement.Position = TimeSpan.FromSeconds(e.NewValue);
        }

        private void NextContextMenu_Click(object sender, RoutedEventArgs e)
        {
            this.NextExecuted(null, null);
        }

        private void MusicListBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Right)
            {
                this.NextExecuted(null, null);
                e.Handled = true;
            }

        }

        private void ListBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                case Key.Down:
                    e.Handled = true;
                    break;

                case Key.Left:
                    {
                        this.Pre_MouseDown(null, null);
                        e.Handled = true;
                    }
                    break;

                case Key.Right:
                    {
                        this.Next_MouseDown(null, null);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private void NavigateToSelectedAlbum(AlbumDirectory albumDirectory)
        {
            if (albumDirectory != null)
            {
                int index = this.albumCollection.IndexOf(albumDirectory);
                int mul = index / this.count;
                if (mul == 0)
                {
                    this.MusicListBox.ItemsSource = this.albumCollection.Take(this.count);
                }
                else
                {
                    this.MusicListBox.ItemsSource = this.albumCollection.Skip(mul * this.count).Take(this.count);
                }

                ////this.BindingToAlbumSelected(albumDirectory);
                ////if (this.MusicListBox.SelectedItem == null)
                ////{
                ////this.MusicListBox.SelectedItem = albumDirectory;
                ////}
            }
        }

        private void NavigateToSelectedAlbumAndSong(AlbumDirectory albumDirectory)
        {
            if (albumDirectory != null)
            {
                int index = this.albumCollection.IndexOf(albumDirectory);
                int mul = index / this.count;
                if (mul == 0)
                {
                    this.MusicListBox.ItemsSource = this.albumCollection.Take(this.count);
                }
                else
                {
                    this.MusicListBox.ItemsSource = this.albumCollection.Skip(mul * this.count).Take(this.count);
                }

                ////this.BindingToAlbumSelected(albumDirectory);
                ////if (this.MusicListBox.SelectedItem == null)
                ////{
                ////this.MusicListBox.SelectedItem = albumDirectory;
                ////}

                var song = albumDirectory.SongCollection.SingleOrDefault(
                                 x => x.Track == Int32.Parse(StringHelper.Right(this.currentTrack.TrackNo, 2)));
                this.MyMediaElement.Source = new Uri(song.FileName);
                this.PlayingSongTextBlock.Text = song.Title;
                this.MyMediaElement.Play();
                ////foreach (var directory in this.albumCollection)
                ////{
                ////    if (directory != null)
                ////    {
                ////        directory.IsPlaying = false;
                ////    }
                ////}

                ////(this.MusicListBox.SelectedItem as AlbumDirectory).IsPlaying = true;
                this.MusicListBox.Items.Refresh();
            }
        }

        private void BindingToAlbumSelected(AlbumDirectory albumDirectory)
        {
            this.PlayGrid.DataContext = albumDirectory;
            this.RatingAlbum.Value = albumDirectory.Rating;
            this.SongListBox.ItemsSource = albumDirectory.SongCollection;
            if (!string.IsNullOrEmpty(albumDirectory.Genre))
            {
                this.AddToCollectionTopAlbumByGenre(albumDirectory.Genre);
            }
        }
    }
}
