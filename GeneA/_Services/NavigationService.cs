using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using GeneA._Helper;
using GeneA.Interfaces;
using GeneA.ViewModels;
using GeneA.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Services;

public class NavigationService
{
    public NavigationService(MainView mainView)
    {
        _stack = new List<Control>();
        _mainView = mainView;
    }

    private readonly MainView _mainView;

    private List<Control> _stack;

    public async Task GoBackAsync(bool needToReload = true, bool needToReloadTitle = true, object? param = null)
    {
        if (_stack.LastOrDefault() is IPopupView)
        {
            await RunInUIThread(() =>
            {
                var children = _mainView.FullViewGrid.Children;

                _stack.RemoveAt(_stack.Count - 1);
                children.RemoveAt(children.Count - 1);
            });
        }

        else if (_stack.LastOrDefault() is HomeView)//already in HomeView
            return;

        // in any menuView except HomeView
        else if (_stack.LastOrDefault() is IMenuView menuView && menuView is not HomeView)
        {
            await RunInUIThread(() =>
            {
                _stack.Clear();
                _mainView.ContentGrid.Children.Clear();

                _stack.Add(GetViewFromViewModel<HomeViewModel>());
                _mainView.ContentGrid.Children.Add(_stack.LastOrDefault()!);
            });
        }

        //in any view
        else
            await RunInUIThread(() =>
            {
                _stack.RemoveAt(_stack.Count - 1);

                _mainView.ContentGrid.Children.Clear();

                var view = _stack.ElementAt(_stack.Count - 1);

                _mainView.ContentGrid.Children.Add(view);
            });

        if (needToReload)
        {
            await RunInUIThread(() =>
            {
                var vm = (_stack.LastOrDefault()?.DataContext as ViewModelBase);
                vm!.Param = param;
                vm!.LoadAction?.Invoke();//reload the previous view
            });
        }

        if (needToReloadTitle)
        {
           await LoadTitle();
        }

        await RunInUIThread(() =>
        {
            if (_mainView.DataContext is MainViewModel vm)
                vm.CanGoback = _stack.Count > 1;
        });
    }



    public async Task GoToAsync<T>(object? param = null) where T : ViewModelBase
    {
        UserControl view = await RunInUIThread(() =>
        {
            UserControl view = param == null
                ? GetViewFromViewModel<T>()
                : GetViewFromViewModel<T>(param);

            return view;
        });

        var lastView = _stack.LastOrDefault();

        if (lastView != null && lastView.GetType() == view.GetType())//trying to navigate to the same place
            return;

        else if (view is IMenuView)//navigate to any bottom menu
        {
            await RunInUIThread(() =>
            {


                _stack.Clear();

                _mainView.ContentGrid.Children.Clear();

                _stack.Add(view);
                _mainView.ContentGrid.Children.Add(view);
            });
        }

        else//navigate further in a bottom menu
        {
            await RunInUIThread(() =>
            {
                _stack.Add(view);

                _mainView.ContentGrid.Children.Clear();
                _mainView.ContentGrid.Children.Add(view);
            });
        }

        await LoadTitle();

        await RunInUIThread(() =>
        {
            if (_mainView.DataContext is MainViewModel vm)
                vm.CanGoback = _stack.Count > 1;
        });
    }

    public async Task<T> PopUpAsync<T>(object? param = null) where T : ViewModelBase
    {
        UserControl view = await RunInUIThread(() =>
        {
            UserControl view = param == null
                ? GetViewFromViewModel<T>()
                : GetViewFromViewModel<T>(param);

            return view;
        });

        ViewModelBase vm = await RunInUIThread(() =>
        {
            _stack.Add(view);
            _mainView.FullViewGrid.Children.Add(view);

            return (ViewModelBase)view.DataContext!;
        });

        return (T)vm;
    }

    private UserControl GetViewFromViewModel<T>(object? param = null) where T : ViewModelBase
    {
        DisposeLastEvents();

        ViewModelBase viewModel = App.ServiceProvider?.GetService<T>()!;
        viewModel.Param = param;
        viewModel.LoadAction?.Invoke();


        string viewModelTypeName = viewModel.GetType().FullName!;
        string viewTypeName = viewModelTypeName.Replace("ViewModel", "View");

        Type viewType = Type.GetType(viewTypeName)!;


        var view = (UserControl)App.ServiceProvider?.GetService(viewType)!;
        view.DataContext = viewModel;

        return view;
    }

    private void DisposeLastEvents()
    {
        foreach (var item in _stack)
        {
            if (item.DataContext is IDisposable disposable)
                disposable.Dispose();
        }
    }

    private async Task LoadTitle()
    {
        await RunInUIThread(() =>
        {
            if (_mainView.DataContext is MainViewModel mainVM)
            {
                if (_stack.LastOrDefault()?.DataContext is ViewModelBase navigatedVM)
                {
                    string title = string.Empty;
                    switch (navigatedVM)
                    {
                        case HomeViewModel:
                            title = DynamicTranslate.Translate(MessageConsts.Home);
                            break;

                        case PersonListingViewModel:
                            title = DynamicTranslate.Translate(MessageConsts.PeopleList);
                            break;

                        case PersonViewModel:
                            title = DynamicTranslate.Translate(MessageConsts.CreateOrEdit);
                            break;

                        case SettingsViewModel:
                            title = DynamicTranslate.Translate(MessageConsts.Settings);
                            break;

                        default: break;
                    }

                    mainVM.Title = title;
                }
            }
        });

    }

    private async Task<ViewModelBase> RunInUIThread(Func<ViewModelBase> action)
    {
        return await Dispatcher.UIThread.InvokeAsync(action);
    }

    private async Task<UserControl> RunInUIThread(Func<UserControl> action)
    {
        return await Dispatcher.UIThread.InvokeAsync(action);
    }

    private async Task RunInUIThread(Action action)
    {
        await Dispatcher.UIThread.InvokeAsync(action);
    }
}
