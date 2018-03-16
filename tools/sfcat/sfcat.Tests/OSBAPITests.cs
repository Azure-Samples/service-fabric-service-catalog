using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using OSB;
using sfcat.OSBCommands;
using System.Threading;
using System.Linq;

namespace sfcat.Tests
{
    [TestClass]
    public class OSBAPITests
    {
        static string ServiceInstanceID;
        static string BindingID;
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            ServiceInstanceID = Guid.NewGuid().ToString("D");
            BindingID = Guid.NewGuid().ToString("D");
        }
        [TestMethod]
        public void CreateBrokerWihoutServer()
        {
            CreateEntityCommand command = new CreateBrokerCommand(new Dictionary<string, string>
            {
                {"name" , "mock" },
                {"url", "http://localhost:3000" },
                {"user", "username" },
                {"password", "password" }
            });
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8888");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(ExceptionConclusion));
        }
        [TestMethod]
        public void CreateBroker()
        {
            CreateEntityCommand command = new CreateBrokerCommand(new Dictionary<string, string>
            {
                {"name" , "mock" },
                {"url", "http://localhost:3000" },
                {"user", "username" },
                {"password", "password" }
            });
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));
        }
        [TestMethod]
        public void GetBrokers()
        {
            ListEntitiesCommand<Broker> command = new ListEntitiesCommand<Broker>("broker");
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(MultiOutputConclusion));
            var brokers = ((MultiOutputConclusion)conclusion).GetObjectList<Broker>();
            Assert.IsNotNull(brokers);
            Assert.IsTrue(brokers.Count == 1);
            Assert.AreEqual(brokers[0].Url, "http://localhost:3000");
        }
        [TestMethod]
        public void Catalog()
        {
            ListEntitiesCommand<Service> command = new ListEntitiesCommand<Service>("service-class");
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(MultiOutputConclusion));
            var services = ((MultiOutputConclusion)conclusion).GetObjectList<Service>();
            Assert.IsNotNull(services);
            Assert.AreEqual(1, services.Count);
            Assert.AreEqual("acb56d7c-XXXX-XXXX-XXXX-feb140a59a66", services[0].Id);
        }       
        [TestMethod]
        public void CreateServiceInstanceBadSchema()
        {
            CreateServiceInstanceCommand command = new CreateServiceInstanceCommand(new Dictionary<string, string>
            {
                {"id", ServiceInstanceID },
                {"f", @"TestFiles\fakeservice-badschema.json" }
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(FailedConclusion));
        }
        [TestMethod]
        public void CreateServiceInstance()
        {
            CreateServiceInstanceCommand command = new CreateServiceInstanceCommand(new Dictionary<string, string>
            {
                {"id", ServiceInstanceID },
                {"f", @"TestFiles\fakeservice.json" }
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));
        }
        [TestMethod]
        public void CreateServiceInstanceNoID()
        {
            CreateServiceInstanceCommand command = new CreateServiceInstanceCommand(new Dictionary<string, string>
            {
                {"f", @"TestFiles\fakeservice.json" }
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(HelpConclusion));
        }
        [TestMethod]
        public void GetLastOperation()
        {
            AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
            ObserveServiceInstanceCommand command = new ObserveServiceInstanceCommand("service-instance", ServiceInstanceID);
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            command.OnIntermediateConclusion += (sender, e) =>
            {
                Assert.IsTrue(e.Conclusion is MultiOutputConclusion);
                var con = (MultiOutputConclusion)e.Conclusion;
                var operations = con.GetObjectList<LastOperation>();
                Assert.IsNotNull(operations);
                Assert.IsTrue(operations.Count >= 1);
                _autoResetEvent.Set();
            };
            command.Run();
            _autoResetEvent.WaitOne(5000);
        }

        [TestMethod]
        public void GetServiceInstances()
        {
            ListEntitiesCommand<ServiceInstance> command = new ListEntitiesCommand<ServiceInstance>("service-instance");
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(MultiOutputConclusion));
            var services = ((MultiOutputConclusion)conclusion).GetObjectList<ServiceInstance>();
            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count>=1);
            Assert.IsNotNull(services.FirstOrDefault(s => s.ServiceId == "acb56d7c-XXXX-XXXX-XXXX-feb140a59a66"));
            Assert.IsNotNull(services.FirstOrDefault(s => s.PlanId == "d3031751-XXXX-XXXX-XXXX-a42377d3320e"));
            Assert.IsNotNull(services.FirstOrDefault(s => s.InstanceId == ServiceInstanceID));
        }
        [TestMethod]
        public void UpdateServiceInstance()
        {
            UpdateServiceInstanceCommand command = new UpdateServiceInstanceCommand(new Dictionary<string, string>
            {
                {"id", ServiceInstanceID },
                {"f", @"TestFiles\fakeservice-newplan.json" }
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));            

            ListEntitiesCommand<ServiceInstance> readCommand = new ListEntitiesCommand<ServiceInstance>("service-instance");
            readCommand.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var readConclusion = readCommand.Run();
            Assert.IsInstanceOfType(readConclusion, typeof(MultiOutputConclusion));
            var services = ((MultiOutputConclusion)readConclusion).GetObjectList<ServiceInstance>();
            Assert.IsNotNull(services);
            Assert.IsTrue(services.Count >= 1);
            Assert.IsNotNull(services.FirstOrDefault(s => s.ServiceId == "acb56d7c-XXXX-XXXX-XXXX-feb140a59a66"));
            Assert.IsNotNull(services.FirstOrDefault(s => s.PlanId == "0f4008b5-XXXX-XXXX-XXXX-dace631cd648"));
            Assert.IsNotNull(services.FirstOrDefault(s => s.InstanceId == ServiceInstanceID));
        }

        [TestMethod]
        public void CreateBinding()
        {
            CreateBindingCommand command = new CreateBindingCommand(new Dictionary<string, string>
            {
                {"instance-id", ServiceInstanceID},
                {"id", BindingID },
                {"f", @"TestFiles\fakeservice-binding.json" }
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));
        }
        [TestMethod]
        public void GetBinding()
        {
            ListEntitiesCommand<BindingwithResult> command = new ListEntitiesCommand<BindingwithResult>("binding");
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(MultiOutputConclusion));
            var bindings = ((MultiOutputConclusion)conclusion).GetObjectList<BindingwithResult>();
            Assert.IsNotNull(bindings);
            Assert.IsTrue(bindings.Count >= 1);
            Assert.IsNotNull(bindings.FirstOrDefault(s => s.Binding.ServiceId == "acb56d7c-XXXX-XXXX-XXXX-feb140a59a66"));
            Assert.IsNotNull(bindings.FirstOrDefault(s => s.Binding.BindingId == BindingID));
        }
        [TestMethod]
        public void Unbind()
        {
            DeleteBindingCommand command = new DeleteBindingCommand();
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            command.InjectSwitch("id", BindingID);
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));

            ListEntitiesCommand<BindingwithResult> readCommand = new ListEntitiesCommand<BindingwithResult>("binding");
            readCommand.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var readConclusion = readCommand.Run();
            Assert.IsInstanceOfType(readConclusion, typeof(MultiOutputConclusion));
            var bindings = ((MultiOutputConclusion)readConclusion).GetObjectList<BindingwithResult>();
            Assert.IsNotNull(bindings);
            Assert.IsNull(bindings.FirstOrDefault(s => s.Binding.BindingId == BindingID));
        }
        [TestMethod]
        public void Deprovisioning()
        {
            DeleteServiceInstanceCommand command = new DeleteServiceInstanceCommand();
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            command.InjectSwitch("id", ServiceInstanceID);
            var conclusion = command.Run();
            Assert.IsInstanceOfType(conclusion, typeof(SuccessConclusion));

            ListEntitiesCommand<ServiceInstance> readCommand = new ListEntitiesCommand<ServiceInstance>("service-instance");
            readCommand.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var readConclusion = readCommand.Run();
            Assert.IsInstanceOfType(readConclusion, typeof(MultiOutputConclusion));
            var services = ((MultiOutputConclusion)readConclusion).GetObjectList<ServiceInstance>();
            Assert.IsNotNull(services);
            Assert.IsNull(services.FirstOrDefault(s => s.InstanceId == ServiceInstanceID));
        }
    }
}
