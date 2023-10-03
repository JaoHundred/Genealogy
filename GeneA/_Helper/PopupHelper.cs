using GeneA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class PopupHelper
    {
        public static async Task<ConfirmationPopupViewModel> ConfigurePopUpProperties(
            this Task< ConfirmationPopupViewModel> viewModel,string title,string message, Action confirmAction, Action cancelAction)
        {
            var vm = await viewModel;

            vm.Title = title;
            vm.Message = message;
            vm.ConfirmAction = confirmAction;
            vm.CancelAction = cancelAction;

            return vm;
        }
    }
}
