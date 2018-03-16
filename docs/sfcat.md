# Using sfcat - walkthrough
sfcat is a sample command-line tool to interact with Catalog Service. The following walkthrough presents you the steps to provision and bind to an Azure Storage account.

# Register a broker
First, use:
```bash
sfcat get broker
```
to check your registered OSB API endpoints. If you don't have one set up yet, you can use:
```bash
sfcat create broker --name <broker name of your choice> --url <OSB endpoint> --user <user name> --password <password>

for example:

sfcat create broker --name azure-broker --url http://35.203.182.162:80 --user username --password password

```
to register one.

Once a broker is registered, you can get its supported service types by:
```bash
sfcat get service-class (or sfcat get sc)
```
For example, after you connect to an Azure OSB API endpoint, you should see the following list:

```bash
0346088a-d4b2-4478-aa32-f18e295ec1d9      azure-rediscache
2e2fc314-37b6-4587-8127-8f9ee8b33fea      azure-storage
451d5d19-4575-4d4a-9474-116f705ecc95      azure-aci
6330de6f-a561-43ea-a15e-b99f44d183e6      azure-cosmos-document-db
6dc44338-2f13-4bc5-9247-5b1b3c5462d3      azure-servicebus
7bade660-32f1-4fd7-b9e6-d416d975170b      azure-eventhub
8797a079-5346-4e84-8018-b7d5ea5c0e3a      azure-cosmos-mongo-db
997b8372-8dac-40ac-ae65-758b4a5075a5      azure-mysqldb
b43b4bba-5741-4d98-a10b-17dc5cee0175      azure-postgresqldb
c54902aa-3027-4c5c-8e96-5b3d3b452f7f      azuresearch
d90c881e-c9bb-4e07-a87b-fcfe87e03276      azure-keyvault
fb9bc99e-0aa9-11e6-8a8a-000d3a002ed5      azure-sqldb
```

# Create an Azure Storage account
To provision a brokered service instance such as Azure Storage, you need to prepare a parameter file that supplies necessary parameters to provision the resource. Different resources require different parameter files. We are working with OSB to make this process more discoverable and automated. At the meanwhile, you need to manually create such a parameter file as a JSON file. For instance, to create a new Azure Storage account, you need the following parameter file:

```json
{
  "service_id": "2e2fc314-37b6-4587-8127-8f9ee8b33fea",
  "plan_id": "6ddf6b41-fb60-4b70-af99-8ecc4896b3cf",
  "parameters": {
    "resourceGroup": "<your Azure resource group>",
    "storageAccountName": "<storage account name, this parameter is not used>",
    "location": "<Azure location, such as eastus>",
    "accountType": "<Storage account type, such as Standard_LRS>"
  }
}
```
> **NOTE**: You can find this file at *sampleFiles\azure-broker\storage.json* under *sfcat*.

To create a storage account, use:

```bash
sfcat create service-instance --file <path to your parameter file> --id <id of your choice>

for example:

sfcat create service-instance --file sampleFiles\azure-broker\storage.json --id cat01
``` 
You can use:
```bash
sfcat get service-instance (or sfcat get si)
```
to list all provisioned service instances.

# Create a binding
Once a resource is provisioned, you can start creating bindings to it. Similarly, you need to provide a parameter file to create a binding, for example:

```json
{
  "service_id": "2e2fc314-37b6-4587-8127-8f9ee8b33fea",
  "plan_id": "6ddf6b41-fb60-4b70-af99-8ecc4896b3cf" 
}
```
> **NOTE**: You can find this file at *sampleFiles\azure-broker\binding.json* under *sfcat*.

To create a binding, use:

```bash
sfcat create binding --file <path to your parameter file> --instance-id <service instnace id> --id <id of your choice>

for example:

sfcat create binding --file sampleFiles\binding.json --instance-id cat01 --id dog01
```

Once the binding is created, you can use:
```bash
sfcat get binding (or sfcat get bd)
```
To get binding details, such as access key to the storage account.
