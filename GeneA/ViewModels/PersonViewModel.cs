using Avalonia.Controls.Notifications;
using Avalonia.Media;
using Avalonia.Notification;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA._Helper;
using GeneA._Services;
using Microsoft.CodeAnalysis.CSharp;
using ModelA.Core;
using Model.Interfaces;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModelA.Database;
using System.Xml.XPath;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using GeneA.ViewModelItems;

namespace GeneA.ViewModels;

public partial class PersonViewModel : ViewModelBase
{
    //TODO: create a control to make the user choice how many iterations are desired on familytree, observable property and all
    //the way to a working solution already exists, just bind Generations property to increment control in view


    public PersonViewModel(IRepository<Person> personRepository, DocumentRepository documentRepository,
        NavigationService navigation, FileService fileService, MainViewModel mainViewModel)
    {
        _personRepository = personRepository;
        _documentRepository = documentRepository;
        _navigation = navigation;
        _fileService = fileService;
        _mainViewModel = mainViewModel;

        _documentList = new ObservableRangeCollection<DocumentFile>();

        LoadAction = () => { Load().SafeFireAndForget(); };
    }

    private readonly IRepository<Person> _personRepository;
    private readonly DocumentRepository _documentRepository;
    private readonly NavigationService _navigation;
    private readonly FileService _fileService;
    private readonly MainViewModel _mainViewModel;

    [ObservableProperty]
    private PersonItemViewModel? _person;

    public IEnumerable<Gender> Genders { get; set; } = StaticList.FillGenders();

    [ObservableProperty]
    private Gender? _selectedGender;

    [ObservableProperty]
    private Person? _selectedFather;

    [ObservableProperty]
    private Person? _selectedMother;

    [ObservableProperty]
    private List<Person>? _fatherList;

    [ObservableProperty]
    private List<Person>? _motherList;

    [ObservableProperty]
    private ObservableRangeCollection<DocumentFile> _documentList;

    private async Task Load()
    {
        await Task.Run(() =>
        {
            if (Param == null)
            {
                Person = new PersonItemViewModel();
            }
            else
            {
                var people = _personRepository.FindAll();

                var fatherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male).ToList();
                var motherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female).ToList();

                Person = _personRepository.FindById((long)Param).ToPersonItemViewModel();

                //constraints to dont show offsprings in father/mother list of father/mother
                fatherList = fatherList.ExceptBy(Person.Offsprings.Select(p => p.Id), q => q.Id).ToList();
                motherList = motherList.ExceptBy(Person.Offsprings.Select(p => p.Id), q => q.Id).ToList();

                SelectedGender = Genders.FirstOrDefault(p => p.GenderEnum == Person.Gender)!;
                SelectedFather = fatherList.FirstOrDefault(p => p.Id == Person.Father?.Id);
                SelectedMother = motherList.FirstOrDefault(p => p.Id == Person.Mother?.Id);

                if (Person.Gender == ModelA.Enums.GenderEnum.Gender.Male)
                    fatherList.RemoveAll(p => p.Id == Person.Id);
                else
                    motherList.RemoveAll(p => p.Id == Person.Id);

                FatherList = fatherList;
                MotherList = motherList;

                var documents = _documentRepository.FindAll();
                var personDocuments = new List<DocumentFile>();

                foreach (var document in documents)
                {
                    if (Person.DocumentFiles.Any(p => p.Id == document.Id))
                    {
                        personDocuments.Add(document);
                    }
                }

                Person.DocumentFiles = personDocuments;


                Dispatcher.UIThread.Invoke(() =>
                {
                    DocumentList.ReplaceRange(Person.DocumentFiles);
                });
            }
        });
    }

    [RelayCommand]
    private async Task Save()
    {
        await Task.Run(async () =>
        {
            //TODO: add validations here

            SaveToDB();

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mainViewModel.NotificationManager.CreateMessage()
                .Animates(true)
                .HasMessage(DynamicTranslate.Translate(MessageConsts.SavedWithSuccess))
                .Dismiss().WithDelay(TimeSpan.FromSeconds(2))
                .Queue();
            });

            await _navigation.GoBackAsync();
        });
    }

    [RelayCommand]
    private async Task Delete()
    {
        await Task.Run(async () =>
        {
            string title = DynamicTranslate.Translate(MessageConsts.ConfirmationDialogTitle);
            string message = DynamicTranslate.Translate(MessageConsts.Confirmation);

            Action confirm = async () =>
            {
                await _documentRepository.DeleteBatchAsync(Person!.DocumentFiles);

                _personRepository.Delete(Person!);
                //close popup and dont reload PersonViewModel
                await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
                await _navigation.GoBackAsync();//back to HomeView
            };
            Action cancel = async () =>
            {
                await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
            };

            await _navigation.PopUpAsync<ConfirmationPopupViewModel>()
            .ConfigurePopUpProperties(title, message, confirm, cancel);
        });
    }

    [RelayCommand]
    private async Task OpenOffSprings()
    {
        await _navigation.PopUpAsync<OffspringsSelectionPopupViewModel>(Person).ConfigurePopUpProperties
            (
             confirmAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             cancelAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             }
            );
    }

    [RelayCommand]
    private async Task OpenSpouses()
    {
        await _navigation.PopUpAsync<SpouseSelectionPopupViewModel>(Person).ConfigurePopUpProperties
            (
             confirmAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             cancelAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             }
            );
    }

    [RelayCommand]
    private async Task OpenNationality()
    {
        await _navigation.PopUpAsync<NationalitySelectionPopupViewModel>(Person).ConfigurePopUpProperties
            (
             confirmAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             cancelAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             }

            );
    }

    [RelayCommand]
    private async Task AddFile()
    {
        var docs = await _fileService.OpenFilePickerAsync();

        if (docs == null)
            return;

        if (Person!.DocumentFiles.Count > 0)
        {
            foreach (var doc in docs)
            {
                var personDoc = Person.DocumentFiles.FirstOrDefault(p => p.FileName == doc.FileName);

                if (personDoc is not null)
                {
                    personDoc.UpdateDate = DateTime.Now;
                    personDoc.DocumentBytes = doc.DocumentBytes;
                }
                else
                    Person.DocumentFiles.Add(doc);
            }
        }
        else
            Person!.DocumentFiles = docs.ToList();

        Dispatcher.UIThread.Invoke(() =>
        {
            DocumentList.ReplaceRange(Person!.DocumentFiles);
        });

        SaveToDB();
    }

    [RelayCommand]
    private async Task OpenFile(DocumentFile file)
    {
        string path = _documentRepository.DownloadToTemporaryFolder(file, Person!.Id);

        if (!string.IsNullOrEmpty(path))
        {
            await _fileService.OpenFileInDefaultAppAsync(path);
        }
    }

    [RelayCommand]
    private async Task DeleteFile(DocumentFile documentFile)
    {
        await _navigation.PopUpAsync<ConfirmationPopupViewModel>(documentFile).ConfigurePopUpProperties
            (
             confirmAction: async () =>
             {
                 Dispatcher.UIThread.Invoke(() =>
                 {
                     DocumentList.Remove(documentFile);
                 });

                 if (documentFile.Id > 0)// its not a temporary documentFile(user didnt save it yet)
                 {
                     _documentRepository.Delete(documentFile);
                     Person!.DocumentFiles.Remove(documentFile);
                 }

                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             cancelAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             title: DynamicTranslate.Translate(MessageConsts.ConfirmationDialogTitle),
             message: DynamicTranslate.Translate(MessageConsts.Confirmation)
            );
    }

    [RelayCommand]
    private void Spin(string direction)
    {
        Person!.Generations = direction switch
        {
            "Increase" when Person!.Generations < 10 => Person!.Generations + 1,
            "Decrease" when Person!.Generations > 4 => Person!.Generations - 1,
            _ => Person!.Generations
        };
    }

    [RelayCommand]
    private async Task OpenFamilyTree()
    {
        await _navigation.GoToAsync<FamilyTreeViewModel>(Person);
    }

    public async Task<IEnumerable<object>> FatherStartsWithAsync(string str, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var list = FatherList!.Where(p => p.Name.ToLower().StartsWith(str.ToLower()));
            return list;
        }, token);
    }

    public async Task<IEnumerable<object>> MotherStartsWithAsync(string str, CancellationToken token)
    {
        return await Task.Run(() =>
        {
            var list = MotherList!.Where(p => p.Name.ToLower().StartsWith(str.ToLower()));
            return list;
        }, token);
    }

    private void SaveToDB()
    {
        if (SelectedGender != null)
            Person!.Gender = SelectedGender.GenderEnum;

        if (SelectedMother != null)
            Person!.Mother = SelectedMother;

        if (SelectedFather != null)
            Person!.Father = SelectedFather;


        foreach (var document in DocumentList)
        {
            if (document.DocumentBytes == null)
                continue;

            _documentRepository.Upsert(document);
        }

        _personRepository.Upsert(Person!);
    }
}
