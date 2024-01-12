using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using AvaloniaGraphControl;
using GeneA.Views;
using ModelA.Core;
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


namespace GeneA._Services
{
    public class FileService
    {
        public FileService(MainView mainView, FamilyTreeView familyTreeView)
        {
            _topLevel = TopLevel.GetTopLevel(mainView)!;
            _familyTreeView = familyTreeView;
        }

        private TopLevel _topLevel;
        private readonly FamilyTreeView _familyTreeView;

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

        public async Task SaveFilePicker(string fileName)
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

            if(file == null)
                return;

            using (var stream = GetImageFileStream(file.Name))
            {
                //TODO: see how to save image to system, flush didnt work?
                await stream.FlushAsync();
            }
        }

        private FileStream GetImageFileStream(string path)
        {
            //TODO: graphPanel bounds is coming 0, its like the graph is not considered created, see what is happening

            // Create a bitmap from the graph control
            var bitmap = new RenderTargetBitmap(new PixelSize((int)_familyTreeView.graphPanel.Bounds.Width
                , (int)_familyTreeView.graphPanel.Bounds.Height)
                , new Vector(96, 96));
            using (var ctx = bitmap.CreateDrawingContext())
            {
                ctx.DrawRectangle(_familyTreeView.graphPanel.Background, null, new Rect(new Point(), bitmap.Size));
                _familyTreeView.graphPanel.Render(ctx);
            }

            // Create an image control with the bitmap as the source
            var image = new Image { Source = bitmap };

            return new FileStream($"{path}.png", FileMode.Create);
        }
    }
}
