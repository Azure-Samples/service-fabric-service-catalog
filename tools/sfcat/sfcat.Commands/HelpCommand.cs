using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class HelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
                new string[]
                { "  #gcreate#~        #aCreate a new entity.",
                  "  #gdelete#~        #aDelete an entity.",
                  "  #gget#~           #aList entities or get configuration settings.",
                  "  #gset#~           #aUpdate a configuration setting.",
                  "  #gupdate#~        #aUpdate an entity.",
                  "  #gwatch#~         #aWatch an entity.",
                });            
        }
    }
    internal class CreateHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
                new string[]
            { "",
              "  #gcreate#~ #a<entity type> [--file <file name>] [--<parm>, <name>]",
              "",
              "  #wSupported entity types: #abroker, service-class, service-instance, binding.",
              "",
              "  #aWhen you use the --file switch, you can override any properties in the file by adding a switch with the property name. For example, to override a some-setting property, add a --some-setting <new value> switch.",
              "",
              "  #cExample:",
              "",
              "  #wTo create a new Service Broker:",
              "        #acreate broker --name azure  --url http://your.osb.endpoint --user tom --password p@ssword!",
              "  #wTo create a new Binding:",
              "        #acreate binding --file my-binding.json --id my-binding"});
        }
    }
    internal class DeleteHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
                new string[]
            { "",
              "  #gdelete#~ #a<entity type> #c--id #a<entity id>",
              "",
              "  #wSupported entity types: #aservice-instance, binding.",              
              "",
              "  #cExample:",
              "",
              "  #wTo delete a Binding:",
              "        #adelete binding --id my_binding_id",
              "  #wTo delete a Service Instance:",
              "        #adelete binding --id my_service_instance_id"});
        }
    }
    internal class CreateBrokerHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
               new string[]
             {
                   "",
                   "  #gcreate broker #c--name #a<name> #c--url #a<url> #c--user #a<username> #c--password #a<passwrod>",
                   "",
                   "    #aor",
                   "",
                   "  #gcreate broker #c--file #a<filename>",
                   "",
                   "  #c--name      #aName of the Service Broker",
                   "  #c--url       #aService Broker's OSB API endpoint",
                   "  #c--user      #aUser name for OSB API authentication",
                   "  #c--password  #aPassword for OSB API authentication",
                   "  #c--file      #aBroker definition JSON file",
                   "",
                   "  #yExample:",
                   "",
                   "  #wTo create a new Service Broker:",
                   "        #acreate broker --name azure  --url http://your.osb.endpoint --user tom --password p@ssword!"});            
        }
    }
    internal class CreateServiceInstanceHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
              new string[]
            {
                "",
                "  #gcreate service-instance #c--file #a<filename> #c--id #a<instance id>",
                "",
                "  #c--file      #aService instance definition JSON file",
                "  #c--id        #aService instance id",
                "",
                "  #yExample:",
                "",
                "  #wTo create a new Service Instance:",
                "        #acreate service-instance --file /path/to/file --id my_unique_id"});
        }
    }
    internal class CreateBindingHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
              new string[]
            {
                "",
                "  #gcreate binding #c--file #a<filename> #c--instance-id #a<instance id> #c--id #a<binding id>",
                "",
                "  #c--file          #aBinding definition JSON file",
                "  #c--instance-id   #aService Instance id",
                "  #c--id            #aBinding id",
                "",
                "  #yExample:",
                "",
                "  #wTo create a new Binding:",
                "        #acreate binding --file /path/to/file --id my_unique_id --instance-id service_instance_id"});            
        }
    }

    internal class SetHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
              new string[]
            {
                "",
                "  #gset #a<config name> <config value>",
                "",
                "  #cExample:",
                "",
                "  #wTo update Namespace:",
                "        #aset Namespace my-ns"});            
        }
    }
}

