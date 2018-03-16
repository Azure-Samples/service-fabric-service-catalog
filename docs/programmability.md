# Calling Catalog Service from your code
Catalog Service is a REST-ful service. You can make direct HTTP calls if you like. In addition, Catalog Service also defines an actor service at **fabric:/SFServiceCatalog/ServiceBrokerActorService**, through which you can provision and bind to brokered resources.

Please see **samples\WebWithBrokeredServiceActor** example to see how a web application provisions and binds to an Azure Storage account through Catalog Service. 

> **NOTE:** Current API requires callers to pass in parameter files, which could be cumbersome to use. We are looking at improving this by auto-generating parameters.

Essentially, your application needs to call the two methods defined by the broker actor:

* **Connect** provisions a resource
* **GetBindingCredential** binds to a resource and retrieves the credential to connect to the resource

Both methods are idempotent, so you are free to repeatedly call them without side effects.