using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SkJukeBox_Demo.Models
{
    class AlbumDirectory : ISupportInitialize
    {
        private string _albumNo;
        public string AlbumNo
        {
            get { return _albumNo; }
            set
            {
                _albumNo = value;
            }
        }

        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set
            {
                _artistName = value;
            }
        }

        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            set
            {
                _albumName = value;
            }
        }

        private string _DirectoryPath;
        public string DirectoryPath
        {
            get { return _DirectoryPath; }
            set
            {
                _DirectoryPath = value;
                Refresh();
            }
        }

        private bool _ExpandFolders = false;
        public bool ExpandFolders
        {
            get { return _ExpandFolders; }
            set
            {
                _ExpandFolders = value;
                Refresh();
            }
        }

        private string _DefaultAlbumArt;
        public string DefaultAlbumArt
        {
            get { return _DefaultAlbumArt; }
            set
            {
                _DefaultAlbumArt = value;
                ////Refresh();
            }
        }

        private ObservableCollection<Song> _songCollection;
        public ObservableCollection<Song> SongCollection
        {
            get { return _songCollection; }
            set
            {
                _songCollection = value;
            }
        }
        public AlbumDirectory()
        {
            this.SongCollection = new ObservableCollection<Song>();
            DirectoryPath = Directory.GetCurrentDirectory();
        }

        public AlbumDirectory(string directoryPath)
        {
            this.SongCollection = new ObservableCollection<Song>();
            DirectoryPath = directoryPath;
        }

        private void Refresh()
        {
            if (_IsInitializing) return;

            this.SongCollection.Clear();
            AddDirectory(_DirectoryPath, ExpandFolders);
        }

        private void AddDirectory(string Path, bool IsRecursive)
        {
            if (Directory.Exists(Path))
            {
                DirectoryInfo d = new DirectoryInfo(Path);

                if (ExpandFolders)
                {
                    // look for directories
                    foreach (DirectoryInfo subd in d.GetDirectories())
                    {
                        AddDirectory(subd.FullName, IsRecursive);
                    }
                }

                // look for files
                foreach (FileInfo f in d.GetFiles("*.mp3"))
                {
                    Song id3 = new Song(f.FullName, DefaultAlbumArt);
                    if (!string.IsNullOrEmpty(id3.Album))
                    {
                        this.AlbumName = id3.Album;
                    }

                    if (!string.IsNullOrEmpty(id3.Artist))
                    {
                        this.ArtistName = id3.Artist;
                    }

                    if (!string.IsNullOrEmpty(id3.AlbumArt))
                    {
                        this.DefaultAlbumArt = id3.AlbumArt;
                    }
                    this.SongCollection.Add(id3);
                }
            }
        }

        #region ISupportInitialize Members

        private bool _IsInitializing = false;

        public void BeginInit()
        {
            _IsInitializing = true;
        }

        public void EndInit()
        {
            _IsInitializing = false;
            Refresh();
        }

        #endregion

    }
}
