using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SkJukeBox_Demo.Models
{
    [DataContract(IsReference = true)]
    public class Albums
    {
        public Albums()
        {
            this.AlbumDirectories = new List<AlbumDirectory>();
        }

        [DataMember]
        public List<AlbumDirectory> AlbumDirectories { get; set; }

        [DataMember]
        public List<Track> TrackList { get; set; }
    }
}
