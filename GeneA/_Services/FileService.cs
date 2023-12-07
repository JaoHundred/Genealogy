using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
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
    }
}
