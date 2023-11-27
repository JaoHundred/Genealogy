using LiteDB;
using Model.Database;
using Model.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
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

            _directoryPath = Path.Combine(_getFolderService.GetApplicationDirectory(), "GeneDocs");

            if (!Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);
        }

        private ILiteStorage<string> _liteStorage;
        private readonly IGetFolderService _getFolderService;

        private string _directoryPath;

        public override void Delete(DocumentFile entity)
        {
            _liteStorage.Delete(entity.Id.ToString());
            base.Delete(entity);
        }

        public override DocumentFile Upsert(DocumentFile entity)
        {
            if (_liteStorage.Exists(entity.Id.ToString()))
                _liteStorage.Delete(entity.Id.ToString());

            string fullPath = Path.Combine(_directoryPath, entity.FileName);


            //TODO: this only send broken files to the directory, search for examples in litedb on how
            //to work with files

            _liteStorage.Upload(entity.Id.ToString(), fullPath);



            return base.Upsert(entity);
        }

        //TODO:upload and download methods here
    }
}