using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GeneA.Views;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Services
{
    public class FileService
    {
        public FileService(MainView mainView)
        {
            _topLevel = TopLevel.GetTopLevel(mainView)!;
        }

        private TopLevel _topLevel;

        public async Task<IList<string>?> OpenFilePickerAsync()
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

            var pathList = new List<string>();

            foreach (var file in files)
            {
                pathList.Add(file.Path.AbsolutePath);
            }

            return pathList;
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public IEnumerable<DocumentFile> GetDocuments(IEnumerable<string> paths)
        {
            return paths.Select(p => new DocumentFile 
            {
                FileExtension = GetFileExtension(p),
                FileName = GetFileName(p),
                OriginalPath = p,
            });
        }
    }
}
