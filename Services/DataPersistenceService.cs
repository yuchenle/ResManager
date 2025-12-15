using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RestoManager.Models;

namespace RestoManager.Services
{
    public class DataPersistenceService
    {
        private readonly string _dishesFilePath;

        public DataPersistenceService()
        {
            // Store data in AppData\Local\RestoManager
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appDataPath, "RestoManager");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            _dishesFilePath = Path.Combine(appFolder, "dishes.json");
        }
    }
}
