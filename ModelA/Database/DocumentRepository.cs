using LiteDB;
using Model.Database;
using Model.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelA.Database
{
    public class DocumentRepository : Repository<DocumentFile>
    {
        public DocumentRepository(LiteDBConfiguration configuration, IGetFolderService getFolderService) : base(configuration)
        {
            _liteStorage = _configuration.LiteDB!.FileStorage;
            _getFolderService = getFolderService;
        }

        private ILiteStorage<string> _liteStorage;
        private readonly IGetFolderService _getFolderService;

        public override void Delete(DocumentFile entity)
        {
            _liteStorage.Delete(entity.Id.ToString());
            base.Delete(entity);
        }

        public override async Task DeleteBatchAsync(IEnumerable<DocumentFile> entities)
        {
            await Task.Run(() =>
            {
                foreach (var entity in entities)
                    Delete(entity);
            });
        }

        public override DocumentFile Upsert(DocumentFile entity)
        {
            DocumentFile document = base.Upsert(entity);

            using (var stream = new FileStream(Uri.UnescapeDataString(entity.OriginalPath), FileMode.Open, FileAccess.Read))
            {
                _liteStorage.Upload(document.Id.ToString(), document.FileName, stream);
            }

            return document;
        }

        public string DownloadToTemporaryFolder(DocumentFile entity, long personId)
        {
            string tempFolder = _getFolderService.GetFolderTemporaryFolderDirectory();
            string fullPath = Path.Combine(Uri.UnescapeDataString(tempFolder), "Temp", "GeneDoc", personId.ToString());

            if(!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);

            fullPath = Path.Combine(fullPath, entity.FileName);

            try
            {
                using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                     _liteStorage.Download(entity.Id.ToString(), stream);
                }
            }
            catch (Exception ex)
            {
            }

            return fullPath;
        }
    }
}