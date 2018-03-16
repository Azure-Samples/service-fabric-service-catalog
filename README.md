![Build Status](https://haishinew.visualstudio.com/_apis/public/build/definitions/5d2cf77c-570e-4b7e-9361-b2bd291db7c7/1/badge)

# SFServiceCatalog
Service Fabric Service Catalog that implements Open Service Broker API (OSB API)

## Test Locally

### Prerequisites

* Visual Studio 2017

### Set up a OSB Service Broker server

You can use any OSB-API compliant Service Broker. There are several implementation on Internet:

* [Sample CloudFoundry servcie broker in Golang](https://github.com/cloudfoundry-samples/go_service_broker)

* [Azure service broker in Node.js](https://github.com/Azure/meta-azure-service-broker)

### Test Locally

1. Make sure your local Service Fabric cluster is up and running.
2. Open the **SFServiceCatalog\SFServiceCatalog.sln** solution.	
3. Modify **CatalogService\appsettings.json** to point to your OSB Service Broker server:

```json
    "ActorSwitches": {
        "OSBEntityCatalogActor": {
        "OSBEndpoint": "http://104.45.224.242",
        "OSBUser": "haishi",
        "OSBPassword":"P$ssword!"
      }
    }
```
4. Open the **tools\sfcat\sfcat.sln** solution and rebuild the client.
5. Press **F5** to launch the Catalog service. Or, you can publish the Service Fabric application to your local cluster.
6. Open a Command line prompt and use **tools\sfcat\sfcat\bin\debug\sfcat.exe** as your client to interact with the Catalog service.

The following is a list of commands that registers a new broker, gets service classes, creates a service instance and then creates a binding:

```
sfcat create broker --file sampleFiles\servicebroker.json --name 1

sfcat get service-classes

sfcat create service-instance --file sampleFiles\serviceinstance.json --storageAccountName <new Azure storage account name> --id <instance id>

sfcat watch service-instance <instance id>

sfcat create binding --file sampleFiles\binding.json --instance_id <instance id> --binding_id <binding id> --id <binding id again>
```

>**Pro tip:** all object types have shorthand notations: _si_ for service-instance, _bd_ for binding, _sc_ for service-class, and _bk_ for broker.



