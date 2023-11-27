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
            string fullDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return fullDirectory;
        }

        public string GetAppStorage()
        {
            string fullDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return fullDirectory;
        }
    }
}
