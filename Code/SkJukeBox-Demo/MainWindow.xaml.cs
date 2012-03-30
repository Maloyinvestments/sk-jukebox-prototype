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
        private int count = 10;
        private ObservableCollection<string> tempStrings;
        ObservableCollection<string> strings = new ObservableCollection<string>() { "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", "aaa", "vvv", "ccc", "aaa1", "vvv1", "ccc1", "aaa2", "vvv2", "ccc2", "aaa3", "vvv3", "ccc3", };

        private int keyCount = 0;
        private string inputKey;
        public MainWindow()
        {
            InitializeComponent();
            tempStrings = new ObservableCollection<string>(strings.Take(count));
            this.MusicListBox.ItemsSource = tempStrings;
        }

        private void CloseContextMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CloseExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D1 || e.Key == Key.D2 || e.Key == Key.D3 || e.Key == Key.D4 || e.Key == Key.D5 || e.Key == Key.D6 || e.Key == Key.D7 || e.Key == Key.D8 || e.Key == Key.D9 || e.Key == Key.D0)
            {
                this.keyCount++;
                inputKey += e.Key;
                inputKey = inputKey.Replace("D", string.Empty);
                if (this.keyCount == 5)
                {
                    MessageBox.Show(this.inputKey);
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
                tempStrings = new ObservableCollection<string>(strings.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }

        private void Next_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (index < this.strings.Count)
            {
                index = index + count;
                tempStrings = new ObservableCollection<string>(strings.Skip(index).Take(count));
                this.MusicListBox.ItemsSource = tempStrings;
            }
        }
    }
}
