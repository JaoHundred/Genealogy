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
        _mainView = mainView;
    }

    private readonly MainView _mainView;

    public async Task GoBackAsync()
    {
        if (_mainView.ContentGrid.Children.LastOrDefault() is HomeView)//already in HomeView
            return;

        // in any menuView except HomeView
        else if (_mainView.ContentGrid.Children.LastOrDefault() is IMenuView menuView && menuView is not HomeView)
        {
            await RunInUIThread(() =>
            {
                _mainView.ContentGrid.Children.Clear();
                _mainView.ContentGrid.Children.Add(GetViewFromViewModel<HomeViewModel>());
            });
        }

        //in any view
        else
            await RunInUIThread(() =>
            {
                _mainView.ContentGrid.Children.RemoveAt(_mainView.ContentGrid.Children.Count - 1);
            });
    }

    public async Task GoToAsync<T>() where T : ViewModelBase
    {
        UserControl view = GetViewFromViewModel<T>();

        var lastView = _mainView.ContentGrid.Children.LastOrDefault();

        if (lastView != null && lastView.GetType() == view.GetType())//trying to navigate to the same place
            return;

        else if (view is IMenuView)//navigate to any bottom menu
        {
            await RunInUIThread(() =>
            {
                _mainView.ContentGrid.Children.Clear();
                _mainView.ContentGrid.Children.Add(view);
            });
        }

        else//navigate further in a bottom menu
        {
            await RunInUIThread(() =>
            {
                _mainView.ContentGrid.Children.Add(view);
            });
        }
    }

    private UserControl GetViewFromViewModel<T>() where T : ViewModelBase
    {
        var viewModel = App.ServiceProvider?.GetService<T>();
        string viewModelTypeName = viewModel!.GetType().FullName!;
        string viewTypeName = viewModelTypeName.Replace("ViewModel", "View");

        Type viewType = Type.GetType(viewTypeName)!;

        var view = (UserControl)App.ServiceProvider?.GetService(viewType)!;
        return view;
    }

    private async Task RunInUIThread(Action action)
    {
        await Dispatcher.UIThread.InvokeAsync(action);
    }
}
