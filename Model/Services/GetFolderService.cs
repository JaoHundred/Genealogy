using Microsoft.Maui.Controls.PlatformConfiguration;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services
{
    public class GetFolderService : IGetFolderService
    {
        public string GetApplicationDirectory()
        {
            string fullDirectory = string.Empty;
            if (DeviceInfo.Platform == DevicePlatform.Android)
                fullDirectory = FileSystem.Current.AppDataDirectory;

            return fullDirectory;
        }
    }
}
