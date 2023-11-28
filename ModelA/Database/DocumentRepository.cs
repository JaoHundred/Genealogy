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
        }

        private ILiteStorage<string> _liteStorage;

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

        //TODO: download method here, see for a temporary folder

    }
}