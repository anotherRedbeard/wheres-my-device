@description('Provide the location of all the resources.')
param location string = resourceGroup().location

@description('The name of the event hub namespace.')
param eventHubNS string = 'eventHubNS'

@description('The name of the eventhub instance.')
param eventHubName string = 'eventHubName'

@description('Name of the keyvault.')
param keyVaultName string = '<apimKeyVaultName>'

@description('Name of the log analytics workspace.')
param lawName string = '<lawName>'

@description('Name of the app insights resource.')
param appInsightsName string = '<appInsightsName>'

//event hub resource
module namespace 'br/public:avm/res/event-hub/namespace:0.4.0' = {
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
  name: '${eventHubNS}/${eventHubName}/SendAccessKey'
}

// Listen authorization rule
resource listenAuthorizationRule 'Microsoft.EventHub/namespaces/eventhubs/authorizationrules@2024-05-01-preview' existing = {
  name: '${eventHubNS}/${eventHubName}/ListenAccessKey'
}

//key vault resource
module vault 'br/public:avm/res/key-vault/vault:0.7.1' = {
  name: 'vaultDeployment'
  params: {
    // Required parameters
    name: keyVaultName
    // Non-required parameters
    enablePurgeProtection: false
    enableRbacAuthorization: true
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

//log analytics workspace resource
module workspace 'br/public:avm/res/operational-insights/workspace:0.5.0' = {
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
  name: 'componentDeployment'
  params: {
    // Required parameters
    name: appInsightsName
    workspaceResourceId: workspace.outputs.resourceId
    // Non-required parameters
    location: location
  }
}
