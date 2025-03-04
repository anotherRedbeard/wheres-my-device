targetScope = 'subscription'

@description('Provide the id of the subscription.')
param subscriptionId string = 'subscriptionId'

@description('Provide the name of the resource group.')
param rgName string = 'rgName'

@description('Provide the location of all the resources.')
param location string = 'location'

param alternateLocation string = 'centralus'

@description('The name of the event hub namespace.')
param eventHubNS string = 'eventHubNS'

@description('The name of the eventhub instance.')
param eventHubName string = 'eventHubName'

@description('Name of the keyvault.')
param keyVaultName string = '<apimKeyVaultName>'

@description('Provide the name of the app service plan.')
param aspName string = 'App Service Plan name'

@allowed([
  'EP1'
  'EP2'
  'EP3'
])
@description('The name of the app service plan sku.')
param aspSku string = 'EP1'

@description('Name of the log analytics workspace.')
param lawName string = '<lawName>'

@description('Name of the app insights resource.')
param appInsightsName string = '<appInsightsName>'

@description('The name of the function app.')
param functionAppName string = '<functionAppName>'

@description('The name of the storage account.')
param storageAccountName string = '<storageAccountName>'

@description('The name of the Cosmos DB account.')
param cosmosDbAccountName string = '<cosmosDbAccountName>'

@description('The name of the Cosmos DB container.')
param cosmosContainerName string = '<cosmosContainerName>'

@description('The name of the Cosmos DB database.')
param cosmosIotDBName string = '<cosmosIotDBName>'

@description('The name of the Static Web App.')
param staticWebAppName string = '<staticWebAppName>'

//create resource group
module resourceGroupResource 'br/public:avm/res/resources/resource-group:0.3.0' = {
  name: 'createResourceGroup'
  scope: subscription(subscriptionId)
  params: {
    name: rgName
    location: location
  }
}

//event hub resource
module namespace 'br/public:avm/res/event-hub/namespace:0.4.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'namespaceDeployment'
  params: {
    // Required parameters
    name: eventHubNS
    // Non-required parameters
    location: location
    managedIdentities: {
      systemAssigned: true
    }
    requireInfrastructureEncryption: true
    skuName: 'Basic'
    authorizationRules: [
      {
        name: 'RootManageSharedAccessKey'
        rights: [
          'Listen'
          'Manage'
          'Send'
        ]
      }
      {
        name: 'SendAccessKey'
        rights: [
          'Send'
        ]
      }
      {
        name: 'ListenAccessKey'
        rights: [
          'Listen'
        ]
      }
    ]
    disableLocalAuth: false
    eventhubs: [
      {
        authorizationRules: [
          {
            name: 'RootManageSharedAccessKey'
            rights: [
              'Listen'
              'Manage'
              'Send'
            ]
          }
          {
            name: 'SendAccessKey'
            rights: [
              'Send'
            ]
          }
          {
            name: 'ListenAccessKey'
            rights: [
              'Listen'
            ]
          }
        ]
        name: eventHubName
        messageRetentionInDays: 1
        retentionDescriptionCleanupPolicy: 'Delete'
        retentionDescriptionRetentionTimeInHours: 3
        partitionCount: 2
        status: 'Active'
      }
    ]
    networkRuleSets: {
      defaultAction: 'Allow'
      publicNetworkAccess: 'Enabled'
      trustedServiceAccessEnabled: false
    }
    roleAssignments: [
    ]
  }
}

// Send authorization rule
resource sendAuthorizationRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-05-01-preview' existing = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: '${eventHubNS}/${eventHubName}/SendAccessKey'
}

// Listen authorization rule
resource listenAuthorizationRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-05-01-preview' existing = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: '${eventHubNS}/${eventHubName}/ListenAccessKey'
}

//key vault resource
module vault 'br/public:avm/res/key-vault/vault:0.7.1' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'vaultDeployment'
  params: {
    // Required parameters
    name: keyVaultName
    // Non-required parameters
    enablePurgeProtection: false
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
    location: location
    roleAssignments: [
    ]
    secrets: [
      {
        contentType: 'Id'
        name: 'favoritePerson'
        value: '3'
      }
      {
        contentType: 'Id'
        name: 'eventHubSendToken'
        value: listKeys(sendAuthorizationRule.id, '2021-06-01-preview').primaryConnectionString
      }
      {
        contentType: 'Id'
        name: 'eventHubListenToken'
        value: listKeys(listenAuthorizationRule.id, '2021-06-01-preview').primaryConnectionString
      }
    ]
  }
}

//app service plan
module serverfarm 'br/public:avm/res/web/serverfarm:0.2.2' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'serverfarmDeployment'
  params: {
    // Required parameters
    name: aspName
    skuCapacity: 1
    skuName: aspSku
    // Non-required parameters
    kind: 'Elastic'
    maximumElasticWorkerCount: 2
    reserved: true
    location: location 
    perSiteScaling: false
    zoneRedundant: false
  }
}

//log analytics workspace resource
module workspace 'br/public:avm/res/operational-insights/workspace:0.5.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'workspaceDeployment'
  params: {
    // Required parameters
    name: lawName
    // Non-required parameters
    location: location
  }
}

//app insights resource
module component 'br/public:avm/res/insights/component:0.4.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'componentDeployment'
  params: {
    // Required parameters
    name: appInsightsName
    workspaceResourceId: workspace.outputs.resourceId
    // Non-required parameters
    location: location
  }
}

// ✅ Storage Account for Azure Functions
module storage 'br/public:avm/res/storage/storage-account:0.8.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'storageDeployment'
  params: {
    name: storageAccountName
    location: location
    skuName: 'Standard_LRS'
  }
}

// ✅ Cosmos DB (for storing location history)
module cosmosDb 'br/public:avm/res/document-db/database-account:0.11.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'cosmosDbDeployment'
  params: {
    name: cosmosDbAccountName
    location: alternateLocation
    defaultConsistencyLevel: 'Session'
    enableFreeTier: false
    enableMultipleWriteLocations: false
    networkRestrictions: {
      publicNetworkAccess: 'Enabled'
    }
    capabilitiesToAdd: [
      'EnableServerless'
    ]
    sqlDatabases: [
      {
        name: cosmosIotDBName
        containers: [
          {
            name: cosmosContainerName
            paths: [
              '/deviceId'
            ]
          }
        ]
      }
    ]
  }
}


// ✅ Azure Function App (For Processing + API Layer)
module functionApp 'br/public:avm/res/web/site:0.15.0' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'functionAppDeployment'
  params: {
    kind: 'functionapp'
    name: functionAppName
    serverFarmResourceId: serverfarm.outputs.resourceId
    appInsightResourceId: component.outputs.resourceId
    storageAccountResourceId: storage.outputs.resourceId
    storageAccountUseIdentityAuthentication: false
    location: location
    managedIdentities: {
      systemAssigned: true
    }
    appSettingsKeyValuePairs: {
      EVENT_HUB_CONNECTION: listKeys(sendAuthorizationRule.id, '2021-06-01-preview').primaryConnectionString
      COSMOS_DB_CONNECTION: cosmosDb.outputs.endpoint 
      FUNCTIONS_EXTENSION_VERSION: '~4'
      FUNCTIONS_WORKER_RUNTIME: 'dotnet-isolated'
      AzureFunctionJobHost__logging__logLevel__default: 'Warning'
    }
    siteConfig: {
      nmberOfWorkers: 1
      linuxFxVersion: 'DOTNET-ISOLATED|8.0'
      alwaysOn: false
      minimumElasticInstanceCount: 1
    }
  }
}

// ✅ Azure Static Web App (For React Frontend)
module staticWebApp 'br/public:avm/res/web/static-site:0.8.2' = {
  scope: resourceGroup(rgName)
  dependsOn: [ resourceGroupResource ]
  name: 'staticWebAppDeployment'
  params: {
    name: staticWebAppName
    location: alternateLocation
    appSettings: { 
      API_URL: 'https://${functionAppName}.azurewebsites.net'
    }
  }
}
