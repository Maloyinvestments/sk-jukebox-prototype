using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SkJukeBox_Demo.Models
{
    [DataContract(IsReference = true)]
    public class Song
    {
        public Song()
        {

        }

        public Song(string path, string defaultAlbumArt, int track)
        {
            HasTag = false;
            // make sure the file exists
            if (File.Exists(path))
            {
                try
                {
                    // set the FileName property
                    FileName = path;
                    Title = path.Substring(path.LastIndexOf("\\") + 1).Replace(".mp3", string.Empty);
                    // open the file
                    FileStream fs;
                    fs = new FileStream(path, FileMode.Open);

                    // read the tag
                    byte[] buffer = new byte[128];
                    fs.Seek(-128, SeekOrigin.End);
                    fs.Read(buffer, 0, 128);
                    fs.Close();

                    // convert the tag buffer into a string
                    UTF8Encoding encoder = new UTF8Encoding();
                    string tag = encoder.GetString(buffer);

                    // if there is a ID3 v1 tag, then read it (we can know by looking at the first 3 bytes for the word TAG)
                    if (tag.Substring(0, 3) == "TAG")
                    {
                        ////HasTag = true;

                        ////Title = RemoveWhiteSpace(tag.Substring(3, 30));
                        ////Artist = RemoveWhiteSpace(tag.Substring(33, 30));
                        ////Album = RemoveWhiteSpace(tag.Substring(63, 30));
                        ////Year = RemoveWhiteSpace(tag.Substring(93, 4));
                        ////Comment = RemoveWhiteSpace(tag.Substring(97, 28));

                        if (tag[125] == 0)
                            Track = (int)buffer[126];
                        else
                            Track = 0;
                    }
                    else
                    {
                        Track = track;
                    }

                    ////// look for album art
                    ////string[] artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt_*Large.jpg");

                    ////if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "AlbumArt*.jpg");
                    ////if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "Album*.jpg");
                    ////if (artPaths.Length == 0) artPaths = Directory.GetFiles(Path.GetDirectoryName(path), "*.jpg");

                    ////if (artPaths.Length > 0)
                    ////{
                    ////    AlbumArt = artPaths[0];
                    ////}
                    ////else
                    ////{
                    ////    AlbumArt = defaultAlbumArt;
                    ////}

                }
                catch { }


            }
        }

        private string RemoveWhiteSpace(string s)
        {
            string newstring = "";

            foreach (char c in s)
            {
                if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSeparator(c))
                {
                    newstring += c;
                }
            }

            return newstring.Trim();
        }

        private bool _HasTag;
        [DataMember]
        public bool HasTag
        {
            get { return _HasTag; }
            set { _HasTag = value; }
        }


        private string _FileName;
        [DataMember]
        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }

        ////private string _AlbumArtPath;
        ////[DataMember]
        ////public string AlbumArt
        ////{
        ////    get { return _AlbumArtPath; }
        ////    set { _AlbumArtPath = value; }
        ////}

        private string _Title;
        [DataMember]
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        ////private string _Artist;
        ////[DataMember]
        ////public string Artist
        ////{
        ////    get { return _Artist; }
        ////    set { _Artist = value; }
        ////}

        ////private string _Album;
        ////[DataMember]
        ////public string Album
        ////{
        ////    get { return _Album; }
        ////    set { _Album = value; }
        ////}

        ////private string _Year;
        ////[DataMember]
        ////public string Year
        ////{
        ////    get { return _Year; }
        ////    set { _Year = value; }
        ////}

        ////private string _Comment;
        ////[DataMember]
        ////public string Comment
        ////{
        ////    get { return _Comment; }
        ////    set { _Comment = value; }
        ////}

        private int _Track;
        [DataMember]
        public int Track
        {
            get { return _Track; }
            set { _Track = value; }
        }
    }
}
