using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.ComponentModel;
using WPFSoundVisualizationLib;

namespace SkJukeBox_Demo.Utility
{
    class UpdatingMediaElement : MediaElement, System.ComponentModel.INotifyPropertyChanged
    {
        #region Constructor

        public UpdatingMediaElement()
        {
            CompositionTarget.Rendering += new EventHandler(CompositionTarget_Rendering);
            this.MediaOpened += new RoutedEventHandler(MP3MediaElement_MediaOpened);
        }

        #endregion

        #region Event Handlers

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Position");
        }

        void MP3MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("NaturalDuration");
            NotifyPropertyChanged("Position");
        }

        #endregion

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
