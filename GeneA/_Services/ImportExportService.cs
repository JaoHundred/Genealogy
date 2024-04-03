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
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeneA.Services
{
    public class ImportExportService
    {
        public ImportExportService(MainView mainView, IRepository<Person> personRepository, DocumentRepository documentFileRepository,
            IRepository<Nationality> nationalityRepository, IGetFolderService getFolderService)
        {
            _personRepository = personRepository;
            _documentFileRepository = documentFileRepository;
            _nationalityRepository = nationalityRepository;
            _getFolderService = getFolderService;

            _topLevel = TopLevel.GetTopLevel(mainView)!;
            _mainView = mainView;
        }

        private readonly IRepository<Person> _personRepository;
        private readonly DocumentRepository _documentFileRepository;
        private readonly IRepository<Nationality> _nationalityRepository;
        private readonly IGetFolderService _getFolderService;
        private readonly TopLevel _topLevel;
        private readonly MainView _mainView;

        public async Task<bool> Import()
        {
            //TODO: open file dialog and get the user selected path to import
            //import, follow the rules from https://github.com/users/JaoHundred/projects/1/views/1?pane=issue&itemId=45536938

            //TODO: select zip file, desserialize the files inside it

            //TODO: proposition: read the json and check if user has documentFiles, add the new DocumentFiles and overwrite with the most recent
            //based on date if the file has the same name, save or update the files inside the litedb fileStorage

            return true;
        }

        public async Task<bool> Export()
        {
            var date = DateTime.Now;

            var file = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                ShowOverwritePrompt = true,
                SuggestedFileName = $"Gene export {date.Day}-{date.Month}-{date.Year}  {date.Hour}-{date.Minute}-{date.Second}",
                DefaultExtension = "zip",
            });

            if (file == null)
                return false;

            var people = _personRepository.FindAll();
            var nationalities = _nationalityRepository.FindAll();
            var documents = _documentFileRepository.FindAll();

            var stringBuilderPeople = new StringBuilder();

            foreach (var person in people)
            {
                foreach (var document in documents)
                {
                    var docIndex = person.DocumentFiles.FindIndex(p => p.Id == document.Id);

                    if (docIndex != -1)
                        person.DocumentFiles[docIndex].DocumentBytes = await _documentFileRepository.GetDocumentBytesAsync(document, person.Id);
                }
                
                stringBuilderPeople.Append(System.Text.Json.JsonSerializer.Serialize(person, person.GetType()));
                stringBuilderPeople.Append(",");
            }

            if(stringBuilderPeople.Length > 0)
                stringBuilderPeople.Length --; // remove last ","
            string correctPeopleJsonFormat = $"[{stringBuilderPeople}]";

            string nationalitiesJson = System.Text.Json.JsonSerializer.Serialize(nationalities, nationalities.GetType());

            string tempFilesPath = Path.Combine(_getFolderService.GetTemporaryFolderDirectory(), "TempFilesToZip");

            if (!Directory.Exists(tempFilesPath))
                Directory.CreateDirectory(tempFilesPath);

            string peopleJsonPath = Path.Combine(tempFilesPath, "people.json");
            string nationalityJsonPath = Path.Combine(tempFilesPath, "nationality.json");

            //TODO: test this and make sure it also works in android
            await File.WriteAllTextAsync(peopleJsonPath, correctPeopleJsonFormat);
            await File.WriteAllTextAsync(nationalityJsonPath, nationalitiesJson);

            ZipFiles(tempFilesPath, file.Path.LocalPath);

            return true;
        }

        private void ZipFiles(string tempSourcePath, string targetPath)
        {
            ZipFile.CreateFromDirectory(tempSourcePath, targetPath, CompressionLevel.Optimal, includeBaseDirectory: false);

            if (Directory.Exists(tempSourcePath))
                Directory.Delete(tempSourcePath, recursive: true);
        }

        private void UnzipFiles(string zipPath, string targetPath)
        {
            ZipFile.ExtractToDirectory(zipPath, targetPath, overwriteFiles: true);
        }
    }
}
