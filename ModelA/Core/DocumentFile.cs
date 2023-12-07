using LiteDB;
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
        public DocumentFile()
        {
            CreateDate = DateTime.Now;
        }

        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;

        public DateTime CreateDate { get; }
        public DateTime? UpdateDate { get; set; }

        [BsonIgnore]
        public byte[]? DocumentBytes { get; set; }
    }
}
