using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services
{
    public class GetFolderService : IGetFolderService
    {
        public string GetApplicationDirectory()
        {
            string fullDirectory = string.Empty;

            fullDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return fullDirectory;
        }

        public string GetTemporaryFolderDirectory()
        {

            string fullDirectory = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fullDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            else
            {
                fullDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }

            return Path.Combine(Uri.UnescapeDataString(fullDirectory), "Temp" , "GeneDoc");
        }
    }
}
