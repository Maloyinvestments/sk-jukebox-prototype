using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SkJukeBox_Demo.Models
{
    [DataContract]
    public class AlbumDirectory : System.ComponentModel.INotifyPropertyChanged
    {
        [DataMember]
        public string AlbumNo { get; set; }

        [DataMember]
        public string ArtistName { get; set; }

        [DataMember]
        public string AlbumName { get; set; }

        [DataMember]
        public string DirectoryPath { get; set; }

        [DataMember]
        public string DefaultAlbumArt { get; set; }

        [DataMember]
        public ObservableCollection<Song> SongCollection { get; set; }

        [DataMember]
        public double? Rating { get; set; }

        [DataMember]
        public string Genre { get; set; }

        public int Rank { get; set; }

        ////private bool? isPlaying;
        ////public bool? IsPlaying
        ////{
        ////    get { return isPlaying; }
        ////    set
        ////    {
        ////        isPlaying = value;
        ////        NotifyPropertyChanged("IsPlaying");
        ////    }
        ////}

        [DataMember]
        public bool IsNotNull { get; set; }

        public AlbumDirectory()
        {
            this.SongCollection = new ObservableCollection<Song>();
            DirectoryPath = Directory.GetCurrentDirectory();
            this.Rating = 0;
            ////this.IsPlaying = null;
            this.IsNotNull = true;
            this.Refresh();
        }

        public AlbumDirectory(string directoryPath)
        {
            this.SongCollection = new ObservableCollection<Song>();
            DirectoryPath = directoryPath;
            this.Rating = 0;
            this.IsNotNull = true;
            ////this.IsPlaying = null;
            this.Refresh();
        }

        public AlbumDirectory(bool isNullAlbum)
        {
            ////this.IsPlaying = null;
            this.IsNotNull = false;
        }

        public void Refresh()
        {
            if (this.SongCollection == null)
            {
                this.SongCollection = new ObservableCollection<Song>();
            }
            else
            {
                this.SongCollection.Clear();
            }
            AddDirectory(DirectoryPath);
        }

        public void AddDirectory(string Path)
        {
            if (Directory.Exists(Path))
            {
                DirectoryInfo d = new DirectoryInfo(Path);

                ////if (ExpandFolders)
                ////{
                ////    // look for directories
                ////    foreach (DirectoryInfo subd in d.GetDirectories())
                ////    {
                ////        AddDirectory(subd.FullName, IsRecursive);
                ////    }
                ////}

                // look for files
                // Add defaul all songs album
                this.SongCollection.Add(new Song() { Title = "All tracks", Track = 0 });

                int track = 1;
                foreach (FileInfo f in d.GetFiles("*.mp3"))
                {
                    Song id3 = new Song(f.FullName, DefaultAlbumArt, track);

                    // For default soft
                    ////if (!string.IsNullOrEmpty(id3.Album))
                    ////{
                    ////    this.AlbumName = id3.Album;
                    ////}
                    ////if (!string.IsNullOrEmpty(id3.Artist))
                    ////{
                    ////    this.ArtistName = id3.Artist;
                    ////}
                    ////if (!string.IsNullOrEmpty(id3.AlbumArt))
                    ////{
                    ////    this.DefaultAlbumArt = id3.AlbumArt;
                    ////}
                    track++;
                    this.SongCollection.Add(id3);
                }
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
