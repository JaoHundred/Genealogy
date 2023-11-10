using ModelA.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public static class DynamicTranslate
    {
        public static string Translate(string word)
        {
            Lazy<ResourceManager> ResMgr = new(
                  () => new ResourceManager(typeof(Lang.Resource)));

            string translated = ResMgr.Value.GetString(word)!;
            return translated;
        }
    }
}
