using Avalonia;
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
    public NavigationService()
    {
        _stack = new List<ViewModelBase>();
    }

    private List<ViewModelBase> _stack;

    //TODO: search for some way to access mainwindow content property
   
    public void GoBack()
    {
        //var curr = App.li
    }

    public void GoTo<T>() where T : ViewModelBase
    {
        throw new NotImplementedException();
    }
}
