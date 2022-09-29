using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CrosswordMaker
{
    public class FileInfo
    {
        public FileInfo(string fileName, string puzzleName)
        {
            FileName = fileName;
            PuzzleName = puzzleName;
        }

        public string FileName { get; }
        public string PuzzleName { get; }

        public override string ToString()
        {
            return PuzzleName;
        }
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        static ObservableCollection<FileInfo> m_puzzleList;
        static bool m_isPuzzleListDirty = false;

        static ClueMap m_clueMap;

        internal static ObservableCollection<FileInfo> PuzzleList
        {
            get
            {
                if (m_puzzleList == null)
                {
                    m_puzzleList = new ObservableCollection<FileInfo>();
                    InitializePuzzleListAsync();
                }
                return m_puzzleList;
            }
        }

        internal static ClueMap ClueMap
        {
            get
            {
                if (m_clueMap == null)
                {
                    m_clueMap = new ClueMap();
                    InitializeClueMapAsync();
                }
                return m_clueMap;
            }
        }

        private static async void InitializePuzzleListAsync()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var file = await folder.TryGetItemAsync("files.txt") as Windows.Storage.StorageFile;

            if (file != null)
            {
                using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        int i = line.IndexOf(';');
                        if (i > 0)
                        {
                            m_puzzleList.Add(new FileInfo(line.Substring(0, i), line.Substring(i + 1)));
                        }
                    }
                }
            }
            else
            {
                // TODO - remove this
                for (int i = 0; i < 100; i++)
                {
                    m_puzzleList.Add(new FileInfo("", $"Puzzle {i}"));
                }
            }

            m_puzzleList.CollectionChanged += PuzzleList_CollectionChanged;
        }

        private static void PuzzleList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            m_isPuzzleListDirty = true;
        }

        private static async Task SavePuzzleListAsync()
        {
            if (!m_isPuzzleListDirty)
                return;

            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("files.txt");

            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                foreach (var puzzle in m_puzzleList)
                {
                    await writer.WriteLineAsync($"{puzzle.FileName};{puzzle.PuzzleName}");
                }
            }

            m_isPuzzleListDirty = false;
            return;
        }

        private static async void InitializeClueMapAsync()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;

            var file = await folder.TryGetItemAsync("clues.txt") as Windows.Storage.StorageFile;

            if (file != null)
            {
                using (var reader = new StreamReader(await file.OpenStreamForReadAsync()))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        int i = line.IndexOf(';');
                        if (i > 0)
                        {
                            string word = line.Substring(0, i);
                            string clue = line.Substring(i + 1);
                            m_clueMap.AddClue(word, clue);
                        }
                    }
                }
            }

            m_clueMap.IsDirty = false;
        }

        private static async Task SaveClueMapAsync()
        {
            if (m_clueMap == null || !m_clueMap.IsDirty)
                return;

            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var file = await folder.CreateFileAsync("clues.txt");

            using (var writer = new StreamWriter(await file.OpenStreamForWriteAsync()))
            {
                foreach (var entry in m_clueMap)
                {
                    string word = entry.Key;

                    foreach (string clue in entry.Value.Split('\n'))
                    {
                        writer.WriteLine($"{word};{clue}");
                    }
                }
            }

            m_clueMap.IsDirty = false;
            return;
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await SavePuzzleListAsync();
            await SaveClueMapAsync();

            deferral.Complete();
        }
    }
}
