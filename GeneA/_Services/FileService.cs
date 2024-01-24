using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using AvaloniaGraphControl;
using GeneA.Views;
using ModelA.Core;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using FilePickerFileType = Avalonia.Platform.Storage.FilePickerFileType;
using SixLabors.ImageSharp.Processing;
using Avalonia.Threading;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Microsoft.Msagl.Core.ProjectionSolver;
using System.Threading;

namespace GeneA._Services
{
    public class FileService
    {
        public FileService(MainView mainView)
        {
            _mainView = mainView;
            _topLevel = TopLevel.GetTopLevel(mainView)!;
        }

        private MainView _mainView;
        private TopLevel _topLevel;

        public async Task<IList<DocumentFile>?> OpenFilePickerAsync()
        {
            var files = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new List<FilePickerFileType>
                {
                    FilePickerFileTypes.ImageJpg,
                    FilePickerFileTypes.ImagePng,
                    FilePickerFileTypes.Pdf,
                },
            });

            if (files.Count == 0)
                return null;

            var docs = new List<DocumentFile>();
            foreach (var file in files)
            {
                Stream stream = await file.OpenReadAsync();

                using (var meStream = new MemoryStream())
                {
                    stream.CopyTo(meStream);

                    docs.Add(new DocumentFile
                    {
                        FileExtension = Path.GetExtension(file.Name),
                        FileName = file.Name,
                        DocumentBytes = meStream.ToArray(),
                    });
                }

                await stream.DisposeAsync();
            }

            return docs;
        }

        public async Task OpenFileInDefaultAppAsync(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = path,
                    UseShellExecute = true
                });
            }
            else //android
            {
                await Xamarin.Essentials.Launcher.OpenAsync(new OpenFileRequest
                {
                    File = new ReadOnlyFile(path),
                });
            }
        }

        public async Task SaveGraphImageAsync(string fileName)
        {
            var file = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                ShowOverwritePrompt = true,
                SuggestedFileName = fileName,
                DefaultExtension = "png",
                FileTypeChoices = new List<FilePickerFileType>
                {
                    FilePickerFileTypes.ImagePng,
                },
            });

            if (file == null)
                return;

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                SaveBitmapToStream(file);
            });
        }

        private void SaveBitmapToStream(IStorageFile file)
        {
            // Ensure that the graphPanel is rendered before capturing its content
            if (_mainView._graphPanelInstance == null)
                return;

            //TODO:graph is not going complete to png image

            var resolution = new Size(3840, 2160); //4k resolution

            double panelWidth = _mainView._graphPanelInstance.Bounds.Width;
            double panelHeight = _mainView._graphPanelInstance.Bounds.Height;
            double scaleX = resolution.Width / panelWidth;
            double scaleY = resolution.Height / panelHeight;

            //_mainView._graphPanelInstance.Background = Brushes.WhiteSmoke;

            // image desired resolution
            var renderTarget = new RenderTargetBitmap(new PixelSize((int)resolution.Width, (int)resolution.Height)
                , new Vector(128, 128));

            double moveToX = resolution.Width / 2 - panelWidth / 2;
            double moveToY = resolution.Height / 2 - panelHeight / 2;

            _mainView._graphPanelInstance.RenderTransform = new TransformGroup()
            {
                Children = new Transforms()
                {
                    //TODO: why scalling the graph cut its contents?
                    //TODO: try to disable temporarily panzoom and scrollviewer
                    //new ScaleTransform(scaleX, scaleY),
                    new TranslateTransform(moveToX, moveToY),
                }
            };

            renderTarget.Render(_mainView._graphPanelInstance);

            using (var fileStream = new FileStream(file.Path.LocalPath, FileMode.Create))
            {
                renderTarget.Save(fileStream);
            }

            //_mainView._graphPanelInstance.Background = Brushes.Transparent;
        }
    }
}
