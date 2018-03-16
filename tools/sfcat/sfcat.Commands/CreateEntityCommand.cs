using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public class CreateEntityCommand : EntityCommand
    {
        public CreateEntityCommand(string entityType, Dictionary<string, string> switches = null, ISchemaChecker checker = null) 
            : base(entityType, "", switches, checker)
        {
        }

        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!mySwitches.ContainsKey("id") || string.IsNullOrEmpty(mySwitches["id"]))
            {
                if (intention != null)
                    return intention;
                else
                    throw new InvalidOperationException("The 'id' field is missing from switches.");
            }
            return base.CheckSwitches(intention);
        }
        protected override Intention RunCommand()
        {            
            try
            {
                string content = BuildPayload();
                Intention intention = CheckPayload(content);
                if (intention != null)
                    return intention;
                CatalogServiceClient.CatalogServiceClient.PutEntityAsync(mySwitches["CatalogServiceEndpoint"], mEntityType, mySwitches["id"], content).Wait();                
                return new SuccessConclusion("Entity created.");
            }
            catch (Exception exp)
            {
                return new ExceptionConclusion(exp);
            }            
        }
    }
}
