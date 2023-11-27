using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GeneA.Views;
using ModelA.Core;
using System;
using System.Collections.Generic;
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

            var documentList = new List<DocumentFile>();

            foreach (var file in files)
            {
                documentList.Add(new DocumentFile()
                {
                    FileName = file.Name,
                    FileExtension = file.Name.Split('.')[1],
                });
            }

            return documentList;
        }
    }
}
