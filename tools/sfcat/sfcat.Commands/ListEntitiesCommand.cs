using OSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json.Linq;
using sfcat.Commands;

namespace sfcat
{
    public class ListEntitiesCommand<T>: EntityCommand
    {
        public ListEntitiesCommand(string entityType, string entityKey = "", Dictionary<string, string> switches = null, ISchemaChecker checker = null, IEntityFormatter formatter = null) 
            : base(entityType, entityKey, switches, checker, formatter)
        {
        }

        protected override Intention RunCommand()
        {
            try
            {
                var results = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<T>(mySwitches["CatalogServiceEndpoint"], mEntityType, mEntityKey).Result;
                return new MultiOutputConclusion(mFormatter!=null? mFormatter.GenerateOutputStrings<T>(results): null, 
                        results!=null?results.Cast<object>():null);
            }
            catch (Exception exp)
            {
                return new ExceptionConclusion(exp);
            }
        }
        public  void RunAA()
        {

            //try
            //{
            //    var results = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<T>(mySwitches["CatalogServiceEndpoint"], mEntityType, mEntityKey).Result;
            //    return new MultiOutputConclusion<T>(results);
            //}
            //catch (Exception exp)
            //{
            //    return new ExceptionConclusion(exp);
            //}

            //string entitiesEndpoint = string.Format("{0}/api/entities", mySwitches["CatalogServiceEndpoint"]);
            //switch (mEntityType)
            //{
            //    case "bk":
            //    case "broker":
            //    case "brokers":
            //        try
            //        {
            //            var brokers = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<Broker>(entitiesEndpoint, "broker", mEntityKey).Result;
            //            if (brokers != null)
            //            {
            //                List<string> output = new List<string>();
            //                output.Add(string.Format("#w{0,-40}  URL", "NAME"));
            //                foreach (var broker in brokers)
            //                    output.Add(string.Format("#a{0,-40}  {1}", broker.Name, broker.Url));
            //                return new MultiOutputConclusion(output, brokers);
            //            }
            //        }
            //        catch (InvalidOperationException exp)
            //        {
            //            return new ExceptionConclusion(exp);
            //        }
            //        break;
            //    case "sc":
            //    case "service-class":
            //    case "service-classes":
            //        try
            //        {
            //            var services = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<Service>(entitiesEndpoint, "service-class", mEntityKey).Result;
            //            if (services != null)
            //            {
            //                if (string.IsNullOrEmpty(mEntityKey))
            //                {
            //                    List<string> output = new List<string>();
            //                    output.Add(string.Format("#w{0,-40}  NAME", "ID"));
            //                    foreach (var service in services)
            //                        output.Add(string.Format("#a{0,-40}  {1}", service.Id, service.Name));
            //                    return new MultiOutputConclusion(output, services);
            //                }
            //                else
            //                {
            //                    List<string> output = new List<string>();
            //                    foreach (var service in services)
            //                    {
            //                        output.Add(string.Format("#w{0,-40}  #a{1}", "Id", service.Id));
            //                        output.Add(string.Format("#w{0,-40}  #a{1}", "Name", service.Name));
            //                        if (service.Plans != null && service.Plans.Count > 0)
            //                        {
            //                            output.Add(string.Format("#w{0}", "Plans"));
            //                            foreach(var plan in service.Plans)
            //                            {
            //                                output.Add(string.Format("          #w{0,-12}  #a{1}", "Id", plan.Id));
            //                                output.Add(string.Format("          #w{0,-12}  #a{1}", "Description", plan.Description));
            //                                output.Add("");
            //                            }
            //                        }
            //                    }
            //                    return new MultiOutputConclusion(output);
            //                }
            //            }
            //        }
            //        catch (InvalidOperationException exp)
            //        {
            //            return new ExceptionConclusion(exp);
            //        }
            //        break;
            //    case "si":
            //    case "service-instance":
            //    case "service-instances":
            //        try
            //        {
            //            var instances = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<ServiceInstance>(entitiesEndpoint, "service-instance", mEntityKey).Result;
            //            if (instances != null)
            //            {
            //                List<string> output = new List<string>();
            //                output.Add(string.Format("#w{0,-40}  {1,-40}  PLAN ID", "ID", "SERVICE ID"));
            //                foreach (var instance in instances)
            //                    output.Add(string.Format("#a{0,-40}  {1,-40}  {2}", instance.InstanceId, instance.ServiceId, instance.PlanId));
            //                return new MultiOutputConclusion(output);
            //            }
            //        }
            //        catch (InvalidOperationException exp)
            //        {
            //            return new ExceptionConclusion(exp);
            //        }
            //        break;
            //    case "bd":
            //    case "binding":
            //    case "bindings":
            //        try
            //        {
            //            var bindings = CatalogServiceClient.CatalogServiceClient.GetEntitiesAsync<BindingwithResult>(entitiesEndpoint, "binding", mEntityKey).Result;
            //            if (bindings != null)
            //            {
            //                List<string> output = new List<string>();
            //                output.Add(string.Format("#w{0,-40}  CREDENTIALS", "ID"));
            //                foreach (var binding in bindings)
            //                {
            //                    if (binding.Result == null)
            //                        output.Add(string.Format("#a{0,-40}  {1}", binding.Binding.BindingId, "#rBinding not found!"));
            //                    else if (binding.Result.Credentials == "null")
            //                        output.Add(string.Format("#a{0,-40}  {1}", binding.Binding.BindingId, "#rCredentials not found!"));
            //                    else
            //                    {
            //                        var credential = JObject.Parse(binding.Result.Credentials);
            //                        bool first = true;
            //                        foreach (var property in credential.Properties())
            //                        {
            //                            if (first)
            //                            {
            //                                output.Add(string.Format("#a{0,-40}  {1}: {2}", binding.Binding.BindingId, property.Name, property.Value));
            //                                first = false;
            //                            }
            //                            else
            //                                output.Add(string.Format("#a{0,-40}  {1}: {2}", "", property.Name, property.Value));
            //                        }
            //                    }
            //                }
            //                return new MultiOutputConclusion(output);
            //            }
            //        }
            //        catch (InvalidOperationException exp)
            //        {
            //            return new ExceptionConclusion(exp);
            //        }
            //        break;
            //}
            //return null;
        }
    }
}
