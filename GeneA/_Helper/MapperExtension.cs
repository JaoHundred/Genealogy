using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class MapperExtension
    {
        public static Gender ToGenderTypes(this GenderEnum.Gender genderEnum)
        {
            return new Gender 
            { 
                GenderEnum = genderEnum, 
                Name = DynamicTranslate.Translate(genderEnum.ToString()),
            };
        }
    }
}
