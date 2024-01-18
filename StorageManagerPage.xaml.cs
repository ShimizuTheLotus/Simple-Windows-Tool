using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SimpleWindowsTool
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StorageManagerPage : Page
    {
        //variables
        List<string> driveLetters = new List<string>();
        bool clearingLogs = false;

        public StorageManagerPage()
        {
            this.InitializeComponent();

            GetDriveLetter();
            InitializeUI();
        }

        private void GetDriveLetter()
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.io.driveinfo.getdrives?view=net-8.0
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            driveLetters.Clear();
            foreach (DriveInfo drive in allDrives) 
            {
                driveLetters.Add(drive.Name);
            }

        }

        private void InitializeUI()
        {
            ChooseDiskComboBox.Items.Clear();
            foreach (string drive in driveLetters) 
            {
                ChooseDiskComboBox.Items.Add(drive);
            }
            try
            {
                ChooseDiskComboBox.SelectedIndex = 0;
            }
            catch { }
        }

        private async void ClearLogsButton_Click(object sender, RoutedEventArgs e)
        {
            if (!clearingLogs)
            {
                clearingLogs = true;
                ClearLogsButton.Content = "Cancel";
                ChooseDiskComboBox.IsEnabled = false;
                ClearLogsProgressRing.IsActive = true;
                ClearLogsFinishedTick.Visibility = Visibility.Collapsed;

                string directoryPath = ChooseDiskComboBox.SelectedItem.ToString();
                if(File.Exists("E:\\New Text Document.log")){
                    StorageFile storageFile = await StorageFile.GetFileFromPathAsync("E:\\New Text Document.log");
                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                try
                {
                    StorageFolder storageFolder = await StorageFolder.GetFolderFromPathAsync(directoryPath);
                    IReadOnlyList<StorageFolder> folderList = await storageFolder.GetFoldersAsync();
                    IReadOnlyList<StorageFile> storageFiles = await storageFolder.GetFilesAsync();
                    foreach (StorageFile storageFile in storageFiles)
                    {
                        if (Path.GetExtension(storageFile.Path).ToLower() == ".log")
                        {
                            if (clearingLogs)
                            {
                                try
                                {
                                    await storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                }
                                catch { }
                            }
                        }
                    }
                    foreach (StorageFolder folder in folderList)
                    {
                        string[] fileNames = Directory.GetFiles(folder.Path, "*", SearchOption.AllDirectories);
                        foreach (string file in fileNames)
                        {
                            if (File.Exists(file))
                            {
                                if (Path.GetExtension(file).ToLower() == ".log")
                                {
                                    StorageFile targetFile = await StorageFile.GetFileFromPathAsync(file);

                                    if (clearingLogs)
                                    {
                                        try
                                        {
                                            await targetFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                                        }
                                        catch { }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch 
                {
                }
                clearingLogs = false;
                ClearLogsButton.Content = "Clear Logs";
                ChooseDiskComboBox.IsEnabled = true;
                ClearLogsProgressRing.IsActive = false;
                ClearLogsFinishedTick.Visibility = Visibility.Visible;
            }
            else
            {
                clearingLogs = false;
                ClearLogsButton.Content = "Clear Logs";
                ChooseDiskComboBox.IsEnabled = true;
                ClearLogsProgressRing.IsActive = false;
            }
        }

        private void ChooseDiskComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearLogsFinishedTick.Visibility = Visibility.Collapsed;
        }
    }
}
