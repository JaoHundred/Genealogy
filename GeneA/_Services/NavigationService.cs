using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
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

    public async Task GoBackAsync()
    {
        if (_mainView.FullViewGrid.Children.LastOrDefault() is IPopup)
        {
            await RunInUIThread(() =>
            {
                var children = _mainView.FullViewGrid.Children;
                children.RemoveAt(children.Count - 1);
            });

            return;
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

                (view.DataContext as ViewModelBase)!.LoadAction?.Invoke();//reload the previous view

                _mainView.ContentGrid.Children.Add(view);
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
            _mainView.FullViewGrid.Children.Add(view);
            
            return (ViewModelBase)view.DataContext!;
        });

        return (T)vm;
    }

    private UserControl GetViewFromViewModel<T>(object? param = null) where T : ViewModelBase
    {
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
