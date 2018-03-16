using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public class EntityCommand: Command
    {
        protected string mEntityType { get; set; }
        protected string mEntityKey { get; set; }
        protected ISchemaChecker mChecker;
        protected IEntityFormatter mFormatter;
        public EntityCommand(string entityType, string entityKey = "", Dictionary<string,string> switches = null, ISchemaChecker checker = null, IEntityFormatter formatter = null)
            :base(switches)
        {
            mEntityType = entityType;
            mEntityKey = entityKey;
            mChecker = checker;
            mFormatter = formatter;
        }
        public virtual string BuildPayload()
        {
            return null;
        }
        public virtual Conclusion CheckPayload(string payload)
        {
            return null;
        }
    }
}
