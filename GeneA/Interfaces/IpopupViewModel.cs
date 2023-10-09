using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA.Interfaces
{
    public interface IpopupViewModel
    {
        string? Title { get; set; }
        string? Message { get; set; }
        Action? ConfirmAction { get; set; }
        Action? CancelAction { get; set; }
    }
}
