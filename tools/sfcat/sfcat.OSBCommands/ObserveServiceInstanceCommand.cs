using Newtonsoft.Json;
using OSB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sfcat.Commands;
using sfcat.OSBCommands;

namespace sfcat
{
    internal class ObserveServiceInstanceCommand: EntityCommand
    {
        public ObserveServiceInstanceCommand(string entityType, string entityKey = "", Dictionary<string, string> switches = null, ISchemaChecker checker = null) 
            : base(entityType, entityKey, switches, checker, new LastOperationFormatter())
        {
        }

        protected override Intention RunCommand()
        {         
            while (true)
            {

                try
                {
                    var response = CatalogServiceClient.CatalogServiceClient.QueryServiceInstanceProvisionStatusAsync(mySwitches["CatalogServiceEndpoint"], mEntityKey).Result;
                    List<LastOperation> operations = new List<LastOperation>();
                    try
                    {
                        operations.Add(JsonConvert.DeserializeObject<LastOperation>(response));
                    }
                    catch (Exception exp) //TODO: catch deserialization exception only
                    {
                        Notify(new ExceptionConclusion(exp));
                    }
                    if (operations != null)
                    {
                        Notify(new MultiOutputConclusion(mFormatter != null ? mFormatter.GenerateOutputStrings<LastOperation>(operations) : null,
                                operations != null ? operations.Cast<object>() : null));
                        
                        foreach (var operation in operations)
                        {
                            if (operation.State == "succeeded")
                                return new MultiOutputConclusion(mFormatter != null ? mFormatter.GenerateOutputStrings<LastOperation>(operations) : null,
                                operations != null ? operations.Cast<object>() : null);
                        }
                    }
                }
                catch (InvalidOperationException exp)
                {
                    return new ExceptionConclusion(exp);
                }
                Thread.Sleep(2000);
            }            
        }
    }
}
