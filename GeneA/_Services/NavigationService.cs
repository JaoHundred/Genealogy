using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
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
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _mainView.ContentGrid.Children.RemoveAt(_mainView.ContentGrid.Children.Count-1);
        });
    }

    public async Task GoToAsync<T>() where T : ViewModelBase
    {
        var viewModel = App.ServiceProvider?.GetService<T>();
        var view = GetViewFromViewModel(viewModel!);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _mainView.ContentGrid.Children.Add(view);
        });
    }

    private UserControl GetViewFromViewModel(ViewModelBase viewModel)
    {
        string viewModelTypeName = viewModel.GetType().FullName!;
        string viewTypeName = viewModelTypeName.Replace("ViewModel", "View");

        Type viewType = Type.GetType(viewTypeName)!;

        var view = App.ServiceProvider?.GetService(viewType);

        return (UserControl)view!;
    }
}
