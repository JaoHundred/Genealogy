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

        public override DocumentFile Upsert(DocumentFile entity)
        {
            //if (_liteStorage.Exists(entity.Id.ToString()))
            //    _liteStorage.Delete(entity.Id.ToString());

            //TODO: update only selected files

            using (var stream = new FileStream(Uri.UnescapeDataString(entity.OriginalPath), FileMode.Open, FileAccess.Read))
            {
                var bla = _liteStorage.Upload(entity.Id.ToString(), entity.FileName, stream);
            }

            return base.Upsert(entity);
        }

        //TODO: download method here, see for a temporary folder and open the downloaded file in an default app

    }
}