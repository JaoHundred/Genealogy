﻿using Avalonia.Controls.Notifications;
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

namespace GeneA.ViewModels;

public partial class PersonViewModel : ViewModelBase
{
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
    private Person? _person;

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
                Person = new Person();
            }
            else
            {
                var people = _personRepository.FindAll();

                var fatherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Male).ToList();
                var motherList = people.Where(p => p.Gender == ModelA.Enums.GenderEnum.Gender.Female).ToList();

                Person = _personRepository.FindById((long)Param);

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
        var documents = await _fileService.OpenFilePickerAsync();

        if (documents == null)
            return;

        if (Person!.DocumentFiles.Count > 0)
        {
            foreach (var document in documents)
            {
                var personDocument = Person.DocumentFiles.Find(p => p.FileName == document.FileName);

                if(personDocument != null)
                {
                    personDocument.FileName = document.FileName;
                    personDocument.FileExtension = document.FileExtension;
                    personDocument.UpdateDate = DateTime.Now;
                }
                else
                    Person.DocumentFiles.Add(document);
            }
        }
        else
            Person!.DocumentFiles = documents.ToList();

        Dispatcher.UIThread.Invoke(() =>
        {
            DocumentList.ReplaceRange(Person!.DocumentFiles);
        });
    }

    [RelayCommand]
    private void OpenFile()
    {
        //TODO: open selected file in default app which can read this file extension
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

                 _documentRepository.Delete(documentFile);

                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             },
             cancelAction: async () =>
             {
                 await _navigation.GoBackAsync(needToReload: false, needToReloadTitle: false);
             }
            );
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
            _documentRepository.Upsert(document);
        }
        //TODO: save to filestorage

        _personRepository.Upsert(Person!);
    }
}
