using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace GeneA.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    /// <summary>
    /// a way to passing runtime parameter to the View Model
    /// </summary>
    public object? Param { get; set; }

    public  Action? LoadAction;
}
