using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkJukeBox_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int index = 0;
        private int count = 3;
        private ObservableCollection<string> tempStrings;
        ObservableCollection<string> strings = new ObservableCollection<string>() { "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", };
        public MainWindow()
        {
            InitializeComponent();
            tempStrings = new ObservableCollection<string>(strings.Take(count));
            this.MusicListBox.ItemsSource = tempStrings;
        }

        private void Pre_Click(object sender, RoutedEventArgs e)
        {
            if (index != 0)
            {
                index = index - 3;
                tempStrings = new ObservableCollection<string>(strings.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (index < this.strings.Count)
            {
                index = index + 3;
                tempStrings = new ObservableCollection<string>(strings.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }
    }
}
