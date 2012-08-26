using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SkJukeBox_Demo.Models
{
    [DataContract(IsReference = true)]
    public class Album
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
        public bool ExpandFolders { get; set; }

        [DataMember]
        public string DefaultAlbumArt { get; set; }
    }
}
