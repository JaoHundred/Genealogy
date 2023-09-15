using CommunityToolkit.Mvvm.ComponentModel;

namespace GeneA.ViewModels;

public class ViewModelBase : ObservableObject
{
    /// <summary>
    /// a way to passing runtime parameter to the View Model
    /// </summary>
    public object? Param { get; set; }
}
