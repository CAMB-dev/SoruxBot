﻿using Sorux.Framework.Bot.Core.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorux.Framework.Bot.Core.Kernel.Service
{
    public class Global
    {
        private IPluginsStoragePermanentAble? _pluginsStoragePermanentAble;
        private IPluginsStorageWebSupport? _pluginsStorageWebSupport;
        private static Global global = null;
        public IMessageQueue messageQueue { get; init; }
        public ILogger Logger { get; init; }
        public IPluginsStorage pluginsStorage { get; init; }
        public IPluginsStoragePermanentAble? pluginsStoragePermanentAble 
        {
            get { return _pluginsStoragePermanentAble; } 
        }

        public IPluginsStorageWebSupport? pluginsStorageWebSupport
        {
            get { return _pluginsStorageWebSupport; }
        }

        public Global(ILogger logger, IMessageQueue messageQueue,IPluginsStorage pluginsStorage)
        {
            this.messageQueue = messageQueue;
            this.Logger = logger;
            this.pluginsStorage = pluginsStorage;
            if (global != null)
                throw new Exception("Cannot Register two Global instances.");
        }
        public static Global GetGlobal() => global;

        public Global AddPluginsStoragePerment(IPluginsStoragePermanentAble pluginsStoragePermanentAble)
        {
            _pluginsStoragePermanentAble = pluginsStoragePermanentAble;
            return global;
        }
        public Global AddPluginsStorageWebSupport(IPluginsStorageWebSupport pluginsStorageWebSupport)
        {
            _pluginsStorageWebSupport = pluginsStorageWebSupport;
            return global;
        }
    }
}
