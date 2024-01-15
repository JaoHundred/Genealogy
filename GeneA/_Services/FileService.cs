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

namespace GeneA._Services
{
    public class FileService
    {
        public FileService(MainView mainView)
        {
            _topLevel = TopLevel.GetTopLevel(mainView)!;
        }

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

            if (file == null)
                return;

            //TODO: see how to save image to system, flush didnt work?
            await SaveBitmapToStream(file.Name);
        }

        private async Task SaveBitmapToStream(string path)
        {

            //TODO: dependency injection of FamilyTreeView may be picking up empty graphPanel while it doesnt exists and setting
            //its bounds to zero, see how I can access the familyTreeView to get the graphpanel with its correct bounds
            //dependency injection might be the culprit 

            // Ensure that the graphPanel is rendered before capturing its content
            var familytreeView = App.ServiceProvider!.GetService(typeof(FamilyTreeView)) as FamilyTreeView;

            if (familytreeView == null)
                return;

            if (familytreeView.graphPanel.IsVisible && familytreeView.Bounds.Width > 0)
            {
                // Access _familyTreeView.graphPanel.Bounds here

                Dispatcher.UIThread.Post(() =>
                {
                    familytreeView.graphPanel.Measure(familytreeView.graphPanel.Bounds.Size);
                    familytreeView.graphPanel.Arrange(familytreeView.graphPanel.Bounds);
                });
            }

            // Create a bitmap from the graphPanel
            var bitmap = new RenderTargetBitmap(new PixelSize((int)familytreeView.graphPanel.Bounds.Width,
                (int)familytreeView.graphPanel.Bounds.Height), new Vector(96, 96));

            using (var ctx = bitmap.CreateDrawingContext())
            {
                ctx.DrawRectangle(familytreeView.graphPanel.Background, null, new Rect(new Point(), bitmap.Size));
                familytreeView.graphPanel.Render(ctx);
            }

            // Convert Avalonia Bitmap to SixLabors.ImageSharp Image
            using (var memoryStream = new MemoryStream())
            {
                using (var graphStream = new FileStream($"{path}.png", FileMode.Create))
                {
                    bitmap.Save(memoryStream);
                    memoryStream.Position = 0;

                    // Use SixLabors.ImageSharp to resize, manipulate, or directly save the image
                    using (var image = SixLabors.ImageSharp.Image.Load(memoryStream))
                    {
                        // Example: Resize the image to a specific size
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Size = new SixLabors.ImageSharp.Size(800, 600),
                            Mode = ResizeMode.Max
                        }));

                        // Save the manipulated image to the provided stream
                        image.Save(graphStream, new PngEncoder());
                    }
                }
            }
        }

    }
}
