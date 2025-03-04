targetScope = 'resourceGroup'
param location string
param rgName string
param cosmosDbAccountName string

resource cosmosDbCheck 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'checkCosmosDbExists'
  location: location
  kind: 'AzureCLI'
  properties: {
    azCliVersion: '2.30.0'
    timeout: 'PT5M'
    retentionInterval: 'P1D'
    scriptContent: '''
      cosmosExists=$(az cosmosdb show --name ${cosmosDbAccountName} --resource-group ${rgName} --query "id" -o tsv || echo "notfound")
      if [ "$cosmosExists" = "notfound" ]; then
        echo "Cosmos DB does not exist"
        echo "{\"cosmosExists\": false}" > $AZ_SCRIPTS_OUTPUT_PATH
      else
        echo "Cosmos DB exists"
        echo "{\"cosmosExists\": true}" > $AZ_SCRIPTS_OUTPUT_PATH
      fi
    '''
  }
}

output cosmosDbExists bool = json(cosmosDbCheck.properties.outputs.cosmosExists).cosmosExists
