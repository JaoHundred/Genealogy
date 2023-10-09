using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GeneA.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.ViewModels
{
    public partial class ConfirmationPopupViewModel : ViewModelBase, IpopupViewModel
    {
        public ConfirmationPopupViewModel()
        {

        }

        [ObservableProperty]
        private string? _title;
        
        [ObservableProperty]
        private string? _message;
        
        public Action? ConfirmAction { get; set; }
        
        public Action? CancelAction { get; set; }

        [RelayCommand]
        public void Confirm()
        {
            ConfirmAction?.Invoke();
        }

        [RelayCommand]
        public void Cancel()
        {
            CancelAction?.Invoke();
        }
    }
}
