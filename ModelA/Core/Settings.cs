using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelA.Core;

public class Settings : IDbEntity
{
    public Guid Id { get; set; }

    public int ColorTheme { get; set; }
    public DateTime CreatedDate { get; set; }
}
