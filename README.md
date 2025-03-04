# Where's My Husband Now?
This is a repo that we will be using to track devices on a map. It will contain everything from the IaC to the apps/apis needed to run.

## Prerequisites

- You will need to use and existing or create a new service principal with an appropriate role to add new resources and a federated identity credential. I followed [this document](https://learn.microsoft.com/en-us/azure/developer/github/connect-from-azure-openid-connect) to set mine up, but here are the commands I used
  - Create the new service principal

    ```# Bash script
      az ad sp create-for-rbac --name myServicePrincipalName1 --role Contributor --scopes /subscriptions/<your-subscription-id>/resourceGroups/<your-resource-group>
    ```

- Configure a federated identity for the service principal
  - Here is the command to create the new service principal

    ```# Bash script
      az ad app federated-credential create --id <your-service-principal-id> --parameters fed-credential.json
    ```
