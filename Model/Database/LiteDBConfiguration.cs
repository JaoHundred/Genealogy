using LiteDB;
using Model.Core;
using Model.Interfaces;
using Model.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Database
{
    public class LiteDBConfiguration
    {
        public const string LiteDataName = "data";
        public const string LiteErrordbLogName = "errorLog";

        public LiteDBConfiguration(IGetFolderService getFolder)
        {
            _getFolder = getFolder;
            StartLiteDB();
            StartErrorLogLiteDB();
        }

        private readonly IGetFolderService _getFolder;
        public LiteDatabase LiteDB { get; private set; }
        public LiteDatabase LiteErrorLogDB { get; private set; }

        public string GetLiteDBPath(string databaseName)
        {
            string applicationFolder = _getFolder.GetApplicationDirectory();
            return Path.Combine(applicationFolder, databaseName);
        }

        private void StartLiteDB()
        {
            BsonMapper bsonMapper = BsonMapper.Global;
            bsonMapper.Entity<Person>().Id(p => p.Id);
            bsonMapper.Entity<Nacionality>().Id(p => p.Id);

            string completePath = $"Filename={GetLiteDBPath(LiteDataName)}";
            LiteDB = new LiteDatabase(completePath, bsonMapper);

            LiteDB.Checkpoint();
        }

        private void StartErrorLogLiteDB()
        {
            BsonMapper bsonMapper = BsonMapper.Global;
            bsonMapper.Entity<ErrorLog>().Id(errorLog => errorLog.Id);

            string completePath = $"Filename={GetLiteDBPath(LiteErrordbLogName)}";
            LiteErrorLogDB = new LiteDatabase(completePath, bsonMapper);

            LiteErrorLogDB.Checkpoint();
        }
    }
}
