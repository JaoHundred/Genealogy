using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper;

public static class StaticList
{

    public static Gender[] FillGenders()
    {
        return new Gender[] 
        {
            new Gender{ GenderEnum = GenderEnum.Gender.Male, Name = Lang.Resource.Male},
            new Gender{ GenderEnum = GenderEnum.Gender.Female, Name = Lang.Resource.Female},
        };
    }
}

public class Gender
{
    public string Name { get; set; } = string.Empty;
    public GenderEnum.Gender GenderEnum { get; set; }
}