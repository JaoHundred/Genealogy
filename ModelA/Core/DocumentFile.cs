using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ModelA.Core
{
    public class DocumentFile : IDbEntity
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{FileName}.{FileExtension}";
        }
    }
}
