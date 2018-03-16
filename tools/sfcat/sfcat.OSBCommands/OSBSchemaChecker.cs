using Newtonsoft.Json;
using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public abstract class OSBSchemaChecker : ISchemaChecker
    {
        private static volatile List<Service> mServices;
        private static object padLock = new object();
        private static string mServiceEndpoint;
        protected static List<Service> Services
        {
            get
            {
                if (mServices == null)
                {
                    lock (padLock)
                    {
                        if (mServices == null)
                        {
                            ListEntitiesCommand<Service> command = new ListEntitiesCommand<Service>("service-class");
                            command.InjectSwitch("CatalogServiceEndpoint", mServiceEndpoint);
                            var conclusion = command.Run();
                            mServices = ((MultiOutputConclusion)conclusion).GetObjectList<Service>();
                        }
                    }
                }
                return mServices;
            }
        }
        public OSBSchemaChecker(string serviceEndpoint)
        {
            mServiceEndpoint = serviceEndpoint;
        }
        public abstract bool IsValid(string payload);        
    }
}
