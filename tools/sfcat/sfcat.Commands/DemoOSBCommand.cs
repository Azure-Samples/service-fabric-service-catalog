using Newtonsoft.Json;
using OSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace sfcat
{
    internal class DemoOSBCommand: Command
    {
        protected override Intention RunCommand()
        {
            string osbEndpoint = "http://104.45.224.242";
            string user = "haishi";
            string password = "P$ssword!";

            //FancyConsole.WriteLine("Demoing OSB API command", ConsoleColor.Cyan, ConsoleColor.Black, 1, 1, true, '=');
            //FancyConsole.WriteLine("This demo calls the meta-azure-service-broker endpoint at " + osbEndpoint, paddingBottom: 1);

            ////v2/catalog

            //FancyConsole.WriteLine("Calling v2/catalog...", ConsoleColor.Yellow, paddingBottom:1, borderBottom:'-');
            //var services = OSBClient.CatalogAsync(osbEndpoint, user, password).Result;
            //FancyConsole.WriteLine("List of registered services", ConsoleColor.White);
            //foreach (var service in services)
            //    Console.WriteLine(service.Name);

            ////provision
            //var storageService = services.First(s => s.Name == "azure-storage");
            //string id = "osb" + Guid.NewGuid().ToString("N").Substring(0, 8);
            //FancyConsole.WriteLine("Calling v2/service_instances/" + id + "...", ConsoleColor.Yellow, paddingTop:1, paddingBottom: 1, borderBottom: '-');
            //var response = OSBClient.ProvisionAsync(osbEndpoint, user, password, storageService.Id, storageService.Plans[0].Id, id, "").Result;
            //FancyConsole.WriteLine("OSB API returned status code: " + response.StatusCode);
            //FancyConsole.WriteLine("OSB API returned status message: " + response.ReasonPhrase);



            ////waiting for provisioning status
            //FancyConsole.WriteLine("Polling for provisioning status ...", ConsoleColor.Yellow, paddingTop:1, paddingBottom:1);
            //bool shouldContinue = false;
            //while (!shouldContinue)
            //{
            //    FancyConsole.WriteLine("Polling...", ConsoleColor.Yellow);
            //    var res = OSBClient.QueryServiceInstanceProvisionStatusAsync(osbEndpoint, user, password, id, "provisioning").Result;
            //    if (res.IsSuccessStatusCode)
            //        shouldContinue = true;
            //    else
            //    {
            //        FancyConsole.WriteLine("        Server returns: " + res.StatusCode + ":" + res.ReasonPhrase);
            //        Thread.Sleep(2000);
            //    }
            //}

            ////bind
            //string bindingId = Guid.NewGuid().ToString("N").Substring(0, 8);
            //FancyConsole.WriteLine("Calling v2/service_instances/" + id + "/service_binding/" + bindingId + "...", ConsoleColor.Yellow, paddingTop:1, paddingBottom: 1, borderBottom: '-');
            //var bindResponse = OSBClient.BindAsync(osbEndpoint, user, password, id, storageService.Id, storageService.Plans[0].Id, bindingId).Result;
            //FancyConsole.WriteLine("OSB API returned binding object: " + JsonConvert.SerializeObject(bindResponse, Formatting.Indented));

            ////unbind
            //FancyConsole.WriteLine("Calling DELETE v2/service_instances/" + id + "/service_binding/" + bindingId + "...", ConsoleColor.Yellow, paddingTop: 1, paddingBottom: 1, borderBottom: '-');
            //OSBClient.UnbindAsync(osbEndpoint, user, password, id, storageService.Id, storageService.Plans[0].Id, bindingId).Wait();
            //FancyConsole.WriteLine("Unbound");

            return null;
        }
    }
}
