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

        _stack = new List<UserControl>();
    }

    private List<UserControl> _stack;
    private readonly MainView _mainView;

    public async Task GoBackAsync()
    {
        _stack.RemoveAt(_stack.Count - 1);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _mainView.Content = _stack.Last();
        });
    }

    public async Task GoToAsync<T>() where T : ViewModelBase
    {
        var viewModel = App.ServiceProvider?.GetService<T>();
        var view = GetViewFromViewModel(viewModel!);

        _stack.Add(view);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _mainView.Content = view;
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
