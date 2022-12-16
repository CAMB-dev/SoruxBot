﻿using Newtonsoft.Json;
using Sorux.Framework.Bot.Core.Kernel.Interface;
using Sorux.Framework.Bot.Core.Kernel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorux.Framework.Bot.Core.Kernel.DataStorage
{
    //Built-in storage implement.
    public class DsLocalStorage
    {
        static DsLocalStorage()
        {
            if (!new FileInfo(Directory.GetCurrentDirectory() + "\\parse.lock").Exists)
            {
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Config").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Logs").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Plugins").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Plugins\\Data").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Plugins\\Bin").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Plugins\\Config").Create();
                new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Lib").Create();
                new FileInfo(Directory.GetCurrentDirectory() + "\\parse.lock").Create();
                new FileInfo(Directory.GetCurrentDirectory() + "\\Config\\AppSettings.json").Create();
            }
        }

        public static AppSettings GetAppSettings() =>
            JsonConvert.DeserializeObject<AppSettings>(
                File.ReadAllText(Directory.GetCurrentDirectory() + "\\Config\\AppSettings.json"));

        public static string GetMessageQueuePath() 
            => Directory.GetCurrentDirectory() + "\\Config\\MessageQueue.sb";

        public static string GetPluginsDataDirectory()
            => Directory.GetCurrentDirectory() + "\\Plugins\\Data";
        public static string GetPluginsConfigDirectory()
            => Directory.GetCurrentDirectory() + "\\Plugins\\Config";
        public static string GetPluginsDirectory()
            => Directory.GetCurrentDirectory() + "\\Plugins";

        public static string GetStoragePath()
            => Directory.GetCurrentDirectory();

    }
}
