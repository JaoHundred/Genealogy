using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GeneA.Views;
using LiteDB;
using Model.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeneA.Services
{
    public class ImportExportService
    {
        public ImportExportService(MainView mainView, IRepository<Person> personRepository)
        {
            _personRepository = personRepository;
            _topLevel = TopLevel.GetTopLevel(mainView)!;
            _mainView = mainView;
        }

        private readonly IRepository<Person> _personRepository;
        private readonly TopLevel _topLevel;
        private readonly MainView _mainView;

        public async Task Import()
        {
            //var files = await _topLevel!.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            //{
            //    AllowMultiple = false,
            //    FileTypeFilter = new List<FilePickerFileType>
            //    {
            //        FilePickerFileTypes.TextPlain,
            //    },
            //});

            //if (files.Count == 0)
            //    return;
        }

        public async Task Export()
        {
            var date = DateTime.Now;

            var file = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                ShowOverwritePrompt = true,
                SuggestedFileName = $"Gene export {date.Day}-{date.Month}-{date.Year}  {date.Hour}-{date.Minute}-{date.Second}",
                DefaultExtension = "json",
            });

            if (file == null)
                return;

            var listToExport = _personRepository.FindAll().ToList();
            string json = System.Text.Json.JsonSerializer.Serialize(listToExport, listToExport.GetType());

            //TODO: test this and make sure it also works in android
            await File.WriteAllTextAsync(json, file.Path.LocalPath);
        }
    }
}
