targetScope = 'resourceGroup'

param name string = uniqueString('keyvault_', subscription().subscriptionId)

resource keyVault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: name
  location: resourceGroup().location
  properties: {
    enabledForDeployment: false
    enabledForTemplateDeployment: false
    enabledForDiskEncryption: false
    tenantId: tenant().tenantId
    accessPolicies:[
      {
        objectId: guid(resourceGroup().name)
        applicationId: guid(resourceGroup().name)
        tenantId: tenant().tenantId
        permissions: {
          secrets:[
            'list'
            'get'
          ]
        }
      }
    ]
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}

