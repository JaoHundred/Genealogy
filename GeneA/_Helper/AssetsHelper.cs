using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper;

public static class AssetsHelper
{
    public static Stream Open(string filename)
    {
        return AssetLoader.Open(new Uri($"avares://GeneA/Assets/{filename}"));
    }
}
