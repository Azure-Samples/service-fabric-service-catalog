using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors.Client;
using BrokeredServiceActor.Interfaces;
using Microsoft.ServiceFabric.Actors;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var actorProxy = ActorProxy.Create<IBrokeredServiceActor>(new ActorId("wmll624"));
            var credentialDict = actorProxy.GetBindingCredential(new System.Threading.CancellationToken()).Result;
            string viewState = "";
            foreach(var tuple in credentialDict)
            {
                viewState += "<h2><b>" + tuple.Item1 + "</b><h2>";
                viewState += "<h3>" + tuple.Item2 + "</h3>";
            }
            ViewData["Message"] = viewState;
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
