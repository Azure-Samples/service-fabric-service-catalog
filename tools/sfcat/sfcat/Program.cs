using CatalogServiceClient;
using OSB;
using sfcat.OSBCommands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace sfcat
{
    class Program
    {
        static ClientSettings mClientSettings;

        public static object Asssembly { get; private set; }

        static void Main(string[] args)
        {
            mClientSettings = CatalogServiceClient.CatalogServiceClient.GetClientSettings(ConfigurationManager.AppSettings["CatalogServiceEndpoint"]).Result;

            //args = new string[] { "delete", "bd", "-i", "hijk"};

            var intention = GetUserIntention(args);
            intention.OnIntermediateConclusion += Intention_IntermediateIntentionCreated;
            var conclusion = intention.Execute();
            if (conclusion != null)
                PrintConclusion("", conclusion);
        }

        private static void Intention_IntermediateIntentionCreated(object sender, ConclusionEventArgs e)
        {
            PrintConclusion("", e.Conclusion);
        }

        static void PrintConclusion(string prefix, Conclusion conclusion)
        {
            if (conclusion is MultiConclusions)
            {
                foreach (var c in ((MultiConclusions)conclusion).Conclusions)
                {
                    PrintConclusion(prefix + "    ", c);
                }
            }
            else if (conclusion is SuccessConclusion)
                FancyConsole.WriteLine("#g" + prefix + ((SuccessConclusion)conclusion).Message);
            else if (conclusion is ExceptionConclusion)
                FancyConsole.DumpException(prefix, ((ExceptionConclusion)conclusion).Exception);
            else if (conclusion is SingeOutputConclusion)
                FancyConsole.WriteLine(prefix + ((SingeOutputConclusion)conclusion).Output);
            else if (conclusion is MultiOutputConclusion)
            {
                var result = (MultiOutputConclusion)conclusion;
                var outputs = result.GetOutputStrings();
                if (outputs != null && outputs.Count > 0)
                {
                    foreach (var s in outputs)
                        FancyConsole.WriteLine(prefix + s);
                }                
            }
        }
        static Command CreateCommand(string typeName, string arg1, string arg2)
        {
            foreach(var k in mClientSettings.SupportedTypes.Keys)
            {
                if (k == typeName || Array.IndexOf(mClientSettings.SupportedTypes[k].Aliases, typeName) >= 0)
                {
                    var setting = mClientSettings.SupportedTypes[k];
                    Assembly assembly = Assembly.LoadFrom(setting.EntityAssembly);
                    Type type = typeof(ListEntitiesCommand<>).MakeGenericType(assembly.GetType(setting.EntityType));
                    Assembly formatterAssembly = Assembly.LoadFrom(setting.FormatterAssembly);
                    Type formatterType = formatterAssembly.GetType(setting.FormatterType);
                    var command = (Command)Activator.CreateInstance(type, new object[]{
                        k,
                        arg2,
                        null,
                        null,
                        Activator.CreateInstance(formatterType)
                    });
                    command.InjectSwitch("CatalogServiceEndpoint", ConfigurationManager.AppSettings["CatalogServiceEndpoint"]);
                    return command;
                }
            }
            return new HelpCommand();
        }
        static Intention GetUserIntention(string[] args)
        {
            Intention help = null;
            if (args == null || args.Length == 0)
            {
                return new Intention
                {
                    Commands = { new LogoCommand(), new HelpCommand() }
                };
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "get":
                        if (args.Length == 1)
                            return new Intention { Commands = { new ListSettingsCommand() } };
                        if (args.Length == 2)
                        {
                            switch (args[1].ToLower())
                            {
                                case "namespace":
                                case "ns":
                                    return new Intention { Commands = { new GetSettingCommand("Namespace") } };
                                case "cs":
                                    return new Intention { Commands = { new GetSettingCommand("CatalogServiceEndpoint") } };
                                default:
                                    return new Intention { Commands = { CreateCommand(args[1], args[1], null) } };                                   
                            }

                        }
                        else if (args.Length == 3)
                            return new Intention { Commands = { CreateCommand(args[1], args[1], args[2]) } };                            
                        else
                            return new Intention { Commands = { new HelpCommand() } };
                    case "update":
                        if (args.Length >= 2)
                        {
                            switch (args[1].ToLower())
                            {
                                case "si":
                                case "service-instance":
                                    var iSwitches = readSwitches(args, 2, out help);
                                    OSBSchemaChecker schemaChecker = new ServiceInstanceSchemaChecker(ConfigurationManager.AppSettings["CatalogServiceEndpoint"]);
                                    if (help != null)
                                        return help;
                                    else
                                        return new Intention { Commands = { new UpdateServiceInstanceCommand(iSwitches, schemaChecker) } };
                                default:
                                    return new Intention { Commands = { new UpdateServiceInstanceHelpCommand() } };
                            }
                        }
                        else
                            return new Intention { Commands = { new CreateHelpCommand() } };
                    case "create":
                        if (args.Length >= 2)
                        {
                            switch (args[1].ToLower())
                            {
                                case "bk":
                                case "broker":
                                    var rSwitches = readSwitches(args, 2, out help);
                                    if (help != null)
                                        return help;
                                    else
                                        return new Intention { Commands = { new CreateBrokerCommand(rSwitches) } };
                                case "si":
                                case "service-instance":
                                    var iSwitches = readSwitches(args, 2, out help);
                                    OSBSchemaChecker schemaChecker = new ServiceInstanceSchemaChecker(ConfigurationManager.AppSettings["CatalogServiceEndpoint"]);
                                    if (help != null)
                                        return help;
                                    else
                                        return new Intention { Commands = { new CreateServiceInstanceCommand(iSwitches, schemaChecker) } };
                                case "bd":
                                case "binding":
                                    var bSwitches = readSwitches(args, 2, out help);
                                    if (help != null)
                                        return help;
                                    else
                                        return new Intention { Commands = { new CreateBindingCommand(bSwitches) } };
                                default:
                                    return new Intention { Commands = { new CreateHelpCommand() } };
                            }
                        }
                        else
                            return new Intention { Commands = { new CreateHelpCommand() } };
                    case "delete":
                        if (args.Length == 4)
                        {
                            switch (args[1].ToLower())
                            {
                                case "si":
                                case "service-instance":
                                    var sSwitches = readSwitches(args, 2, out help);
                                    if (help != null)
                                        return help;
                                    else
                                        return new Intention { Commands = { new DeleteServiceInstanceCommand(sSwitches) } };
                                case "bd":
                                case "binding":
                                    var bSwitches = readSwitches(args, 2, out help);
                                    if (help != null)
                                        return help;
                                    else 
                                        return new Intention { Commands = {new DeleteBindingCommand(bSwitches) }};
                                default:
                                    return new Intention { Commands = { new DeleteHelpCommand() } };
                            }
                        }
                        else
                            return new Intention { Commands = { new DeleteHelpCommand() } };
                    case "watch":
                        if (args.Length == 3)
                        {
                            switch (args[1].ToLower())
                            {
                                case "si":
                                case "service-instance":
                                    var sSwitches = readSwitches(args, 3, out help);
                                    if (help != null)
                                        return help;
                                    else 
                                        return new Intention { Commands = { new ObserveServiceInstanceCommand("service-instance", args[2], sSwitches) } };
                                default:
                                    return new Intention { Commands = { new HelpCommand() } };
                            }
                        }
                        else
                            return new Intention { Commands = { new HelpCommand() } };
                    case "demo":
                        if (args.Length == 2)
                        {
                            switch (args[1].ToLower())
                            {
                                case "osb":
                                    return new Intention { Commands = { new DemoOSBCommand() } };
                                default:
                                    return new Intention { Commands = { new HelpCommand() } };
                            }
                        }
                        else
                            return new Intention { Commands = { new HelpCommand() } };
                    case "set":
                        if (args.Length == 3)
                        {
                            switch (args[1].ToLower())
                            {
                                case "ns":
                                case "namespace":
                                    return new Intention { Commands = { new SetSettingCommand("Namespace", args[2]) } };
                                case "cs":
                                    return new Intention { Commands = { new SetSettingCommand("CatalogServiceEndpoint", args[2]) } };
                                default:
                                    return new Intention { Commands = { new SetSettingCommand(args[1], args[2]) } };
                            }
                        }
                        else
                            return new Intention { Commands = { new SetHelpCommand() } };
                    default:
                        return new Intention { Commands = { new HelpCommand() } };
                }
            }
        }
        static Dictionary<string,string> readSwitches(string[] args, int index, out Intention intention)
        {
            intention = null;
            Dictionary<string, string> ret = new Dictionary<string, string>();
            string switchKey = "";
            for (int i = index; i < args.Length; i++)
            {
                if (string.IsNullOrEmpty(switchKey))
                {
                    if (args[i].StartsWith("--"))
                    {
                        switchKey = args[i].Substring(2);
                    }
                    else if (args[i].StartsWith("-"))
                    {
                        switchKey = args[i].Substring(1);
                    }
                    else
                    {
                        intention = new Intention { Commands = { new CreateHelpCommand() } };
                        return ret;
                    }
                }
                else
                {
                    ret[switchKey] = args[i];
                    switchKey = "";
                }
            }
            ret.Add("CatalogServiceEndpoint", ConfigurationManager.AppSettings["CatalogServiceEndpoint"]);
            return ret;
        }
    }
}
