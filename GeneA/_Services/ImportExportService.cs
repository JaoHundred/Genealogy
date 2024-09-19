﻿using Avalonia.Controls;
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
using System.Text.RegularExpressions;
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
            var storageFiles = await _topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new FilePickerFileType[]
                 {
                    new("Zip Files")
                    {
                        Patterns = new[]{"*.zip"},
                        MimeTypes = new[] { "application/zip" }
                    }
                 },
            });

            if (!storageFiles.Any() || !HasFileInsideZip(storageFiles.First().Path.LocalPath, "people.json"))
                return false;

            using (var zipFile = ZipFile.OpenRead(storageFiles.First().Path.LocalPath))
            {
                foreach (var entry in zipFile.Entries)
                {
                    if (!entry.FullName.EndsWith(".json"))
                        continue;

                    using (var stream = entry.Open())
                    {
                        switch (entry.Name)
                        {
                            case "nationality.json":

                                var nationalities = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Nationality>>(stream);

                                if (nationalities != null)
                                {
                                    foreach (var nationality in nationalities)
                                    {
                                        ImportUpdateNationality(nationality);
                                    }
                                }

                                break;

                            case "people.json":

                                var people = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<Person>>(stream);

                                if (people != null)
                                {
                                    foreach (var person in people)
                                    {
                                        await ImportUpdatePerson(person);
                                    }
                                }

                                break;
                            default:
                                break;
                        }

                    }
                }
            }

            return true;
        }

        private void ImportUpdateNationality(Nationality nationality)
        {
            var existingNationality = _nationalityRepository.FindById(nationality.Id);

            //you can only add or remove nationalities
            if(existingNationality == null)
            {
                _nationalityRepository.Upsert(nationality);
            }
        }

        private async Task ImportUpdatePerson(Person person)
        {
            var existingPerson = _personRepository.FindById(person.Id);

            if (existingPerson != null)
            {
                //TODO: test importing with another device
                if (existingPerson.UpdatedDate < person.UpdatedDate)
                {
                    await _documentFileRepository.DeleteBatchAsync(existingPerson.DocumentFiles);

                    _personRepository.Update(person);

                    foreach (var document in person.DocumentFiles)
                    {
                        _documentFileRepository.Upsert(document);
                    }

                    //import, follow the rules from https://github.com/users/JaoHundred/projects/1/views/1?pane=issue&itemId=45536938
                }
            }
            else
            {
                _personRepository.Upsert(person);

                foreach (var document in person.DocumentFiles)
                {
                    _documentFileRepository.Upsert(document);
                }
            }
        }

        public async Task<bool> Export()
        {
            var date = DateTime.Now;
            string fileNameDate = Regex.Replace(date.ToString("dd/MM/yyyy HH:mm:ss"), "[^0-9a-zA-Z]+", "-");

            var file = await _topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                ShowOverwritePrompt = true,
                SuggestedFileName = $"Gene export {fileNameDate}",
                DefaultExtension = "zip",
            });

            if (file == null)
                return false;

            var people = _personRepository.FindAll();

            if (!people.Any())
                return false;

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

            if (stringBuilderPeople.Length > 0)
                stringBuilderPeople.Length--; // remove last ","
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

        //TODO: importing and exporting are not working in android, take a look in what could be, my bet is if has something to do with
        //permissions to files
        private void ZipFiles(string tempSourcePath, string targetPath)
        {
            ZipFile.CreateFromDirectory(tempSourcePath, targetPath, CompressionLevel.Optimal, includeBaseDirectory: false);

            if (Directory.Exists(tempSourcePath))
                Directory.Delete(tempSourcePath, recursive: true);
        }

        private bool HasFileInsideZip(string zipPath, string fileName)
        {
            using (var zip = ZipFile.OpenRead(zipPath))
            {
                return zip.Entries.Any(p => p.Name == fileName);
            }
        }
    }
}
