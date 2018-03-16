using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class DeleteEntityCommand : EntityCommand
    {
        public DeleteEntityCommand(string entityType, Dictionary<string, string> switches = null) : base(entityType, "", switches)
        {
        }

        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!mySwitches.ContainsKey("id") || string.IsNullOrEmpty(mySwitches["id"]))
            {
                if (intention != null)
                    return intention;
                else
                    return new HelpConclusion { Commands = new List<Command> { new DeleteHelpCommand() } };
            }
            return base.CheckSwitches(intention);
        }

        protected override Intention RunCommand()
        {
            try
            {
                CatalogServiceClient.CatalogServiceClient.DeleteEntityAsync(mySwitches["CatalogServiceEndpoint"], mEntityType, mySwitches["id"]).Wait();
                return new SuccessConclusion("Entity deleted.");
            }
            catch (Exception exp)
            {
                return new ExceptionConclusion(exp);
            }
        }        
    }
}
