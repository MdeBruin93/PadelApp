@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource sqlServerAdminManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' = {
  name: take('azuresql-admin-${uniqueString(resourceGroup().id)}', 63)
  location: location
}

resource azuresql 'Microsoft.Sql/servers@2023-08-01' = {
  name: take('azuresql-${uniqueString(resourceGroup().id)}', 63)
  location: location
  properties: {
    administrators: {
      administratorType: 'ActiveDirectory'
      login: sqlServerAdminManagedIdentity.name
      sid: sqlServerAdminManagedIdentity.properties.principalId
      tenantId: subscription().tenantId
      azureADOnlyAuthentication: true
    }
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    version: '12.0'
  }
  tags: {
    'aspire-resource-name': 'azuresql'
  }
}

resource sqlFirewallRule_AllowAllAzureIps 'Microsoft.Sql/servers/firewallRules@2023-08-01' = {
  name: 'AllowAllAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
  parent: azuresql
}

resource database 'Microsoft.Sql/servers/databases@2023-08-01' = {
  name: 'database'
  location: location
  properties: {
    freeLimitExhaustionBehavior: 'BillOverUsage'
    useFreeLimit: true
  }
  sku: {
    name: 'GP_S_Gen5_2'
  }
  parent: azuresql
}

output sqlServerFqdn string = azuresql.properties.fullyQualifiedDomainName

output name string = azuresql.name

output sqlServerAdminName string = sqlServerAdminManagedIdentity.name