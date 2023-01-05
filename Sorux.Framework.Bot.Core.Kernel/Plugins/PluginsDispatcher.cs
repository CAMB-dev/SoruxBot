﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sorux.Framework.Bot.Core.Interface.PluginsSDK.Attribute;
using Sorux.Framework.Bot.Core.Interface.PluginsSDK.Register;
using Sorux.Framework.Bot.Core.Interface.PluginsSDK.SDK.Basic;
using Sorux.Framework.Bot.Core.Kernel.Builder;
using Sorux.Framework.Bot.Core.Kernel.Interface;
using Sorux.Framework.Bot.Core.Kernel.Utils;

namespace Sorux.Framework.Bot.Core.Kernel.Plugins;
/// <summary>
/// 插件调度器，负责分配事件到具体的插件。
/// </summary>
public class PluginsDispatcher
{
    private BotContext _botContext;
    private ILoggerService _loggerService;
    private IPluginsStorage _pluginsStorage;
    private string _globalCommandPrefix;

    public PluginsDispatcher(BotContext botContext,ILoggerService loggerService,IPluginsStorage pluginsStorage,IConfiguration configuration)
    {
        this._botContext = botContext;
        this._loggerService = loggerService;
        this._pluginsStorage = pluginsStorage;
        IConfigurationSection section = configuration.GetRequiredSection("CommunicateTrigger");
        this._globalCommandPrefix = section["State"]!.Equals("True") ? section["TriggerChar"]! : "";
    }
    
    //插件按照触发条件可以分为选项式命令触发和事件触发
    //前者针对某个特定 EventType 的某个特定的语句触发某个特定的方法
    //后者针对某个通用的 EventType 进行触发
    private Dictionary<string, Delegate> _matchList = new Dictionary<string, Delegate>();
    /// <summary>
    /// 注册指令路由
    /// </summary>
    /// <param name="filepath"></param>
    /// <param name="name"></param>
    public void RegisterCommandRoute(string filepath,string name)
    {
        Assembly assembly = Assembly.LoadFile(filepath);
        Type[] types = assembly.GetExportedTypes();
        foreach (var className in types)
        {
            if (className.BaseType == typeof(BotController))
            { 
                _loggerService.Debug("CommandRoute","Controller is caught! For type ->" + className.Name);
                //缓存 Controller
                ConstructorInfo constructorInfo = className.GetConstructors()[0];
                ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
                List<object> objects = new List<object>();
                IServiceProvider serviceProvider = _botContext.ServiceProvider;
                foreach (var parameterInfo in parameterInfos)
                {
                    #region 匹配参数
                    if (parameterInfo.ParameterType == typeof(BotContext))
                    {
                        objects.Add(_botContext);
                    }else if (parameterInfo.ParameterType == typeof(ILoggerService))
                    {
                        objects.Add(_loggerService);
                    }else if (parameterInfo.ParameterType == typeof(IBasicAPI))
                    {
                        objects.Add(serviceProvider.GetRequiredService<IBasicAPI>());
                    }else if (parameterInfo.ParameterType == typeof(ILongMessageCommunicate))
                    {
                        objects.Add(serviceProvider.GetRequiredService<ILongMessageCommunicate>());
                    }else if (parameterInfo.ParameterType == typeof(IPluginsDataStorage))
                    {
                        objects.Add(serviceProvider.GetRequiredService<IPluginsDataStorage>());
                    }else if (parameterInfo.ParameterType == typeof(IPluginsStoragePermanentAble))
                    {
                        objects.Add(serviceProvider.GetRequiredService<IPluginsStoragePermanentAble>());
                    }
                    #endregion
                }
                _pluginsStorage.SetPluginInstance(name+ "." + className.Name, Activator.CreateInstance(className,objects.ToArray())!);
                MethodInfo[] methods = className.GetMethods();
                foreach (var methodInfo in methods)
                {
                    if (methodInfo.IsDefined(typeof(EventAttribute)))
                    {
                        var methodEventAttribute = methodInfo.GetCustomAttribute<EventAttribute>();
                        string commandTriggerType = methodEventAttribute!.EventType.ToString();
                        var methodEventCommand = methodInfo.GetCustomAttribute<CommandAttribute>();
                        string commandPrefix = methodEventCommand!.CommandPrefix switch
                           {
                               CommandAttribute.Prefix.None   => "",
                               CommandAttribute.Prefix.Single => _pluginsStorage.GetPluginInfor(name,"CommandPrefixContent"),
                               CommandAttribute.Prefix.Global => _globalCommandPrefix,
                               _                              => ""
                           };
                        foreach (var s in methodEventCommand!.Command)
                        {
                            //添加进入路由
                            //[Type]/<Prefix>[Command]
                            _matchList.Add(commandTriggerType + "/" + commandPrefix + s.Split(" ")[0],null);
                        }
                    }
                }
            }
        }
    }
}