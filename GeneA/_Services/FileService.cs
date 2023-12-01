﻿using Avalonia.Controls;
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
            //TODO: see how to manage that in android and its quirks

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

            var pathList = new List<string>(files.Select(p => p.Path.AbsolutePath).ToList());

            return pathList;
        }

        public void OpenFileInDefaultApp(string path)
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
            else
            {

            }
            //TODO: process start will work only in windows for android it maybe should be in android project itself?
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