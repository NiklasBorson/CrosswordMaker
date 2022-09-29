using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CrosswordMaker
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool m_isSelectMode = false;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void NewPuzzleButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            m_isSelectMode = !m_isSelectMode;
            PuzzleListView.SelectionMode = m_isSelectMode ? ListViewSelectionMode.Multiple : ListViewSelectionMode.Single;
            PuzzleListView.IsItemClickEnabled = !m_isSelectMode;
            DeleteButton.Visibility = m_isSelectMode ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PuzzleListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var fileInfo = e.ClickedItem as FileInfo;
            // TODO
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
    }
}
