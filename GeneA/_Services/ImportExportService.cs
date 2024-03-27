using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GeneA.Views;
using LiteDB;
using Model.Interfaces;
using ModelA.Core;
using ModelA.Database;
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
        public ImportExportService(MainView mainView, IRepository<Person> personRepository, DocumentRepository documentFileRepository)
        {
            _personRepository = personRepository;
            _documentFileRepository = documentFileRepository;

            _topLevel = TopLevel.GetTopLevel(mainView)!;
            _mainView = mainView;
        }

        private readonly IRepository<Person> _personRepository;
        private readonly DocumentRepository _documentFileRepository;
        private readonly TopLevel _topLevel;
        private readonly MainView _mainView;

        public async Task Import()
        {
            //TODO: select zip file, desserialize the files inside it

            //TODO: proposition: read the json and check if user has documentFiles, add the new DocumentFiles and overwrite with the most recent
            //based on date if the file has the same name, save or update the files inside the litedb fileStorage
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

            var people = _personRepository.FindAll().ToList();
            var documents = _documentFileRepository.FindAll().ToList();

            //TODO: export nationalites too in their own file, then zip both people.json and nationalities.json

            foreach (var person in people)
            {
                foreach (var document in documents)
                {
                    var docIndex = person.DocumentFiles.FindIndex(p => p.Id == document.Id);

                    if (docIndex != -1)
                        person.DocumentFiles[docIndex].DocumentBytes = _documentFileRepository.GetDocumentBytes(document, person.Id);
                }
            }

            string json = System.Text.Json.JsonSerializer.Serialize(people, people.GetType());

            //TODO: test this and make sure it also works in android
            await File.WriteAllTextAsync(file.Path.LocalPath, json);
        }
    }
}
