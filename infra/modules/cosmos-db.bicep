@description('Name of the Cosmos DB account')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('Resource ID of the Log Analytics workspace for diagnostics')
param logAnalyticsWorkspaceResourceId string

// ============================================================================
// Cosmos DB Account — NoSQL API, Serverless (AVM)
// ============================================================================

module cosmosAccount 'br/public:avm/res/document-db/database-account:0.19.0' = {
  name: '${name}-deploy'
  params: {
    name: name
    location: location
    tags: tags
    capabilitiesToAdd: [
      'EnableServerless'
    ]
    failoverLocations: [
      {
        locationName: location
        failoverPriority: 0
        isZoneRedundant: false
      }
    ]
    sqlDatabases: [
      {
        name: 'logistics-db'
        containers: [
          {
            name: 'shipments'
            paths: [
              '/customerId'
            ]
          }
          {
            name: 'vehicles'
            paths: [
              '/depotId'
            ]
          }
          {
            name: 'routes'
            paths: [
              '/regionId'
            ]
          }
        ]
      }
    ]
    disableLocalAuthentication: true
    minimumTlsVersion: 'Tls12'
    diagnosticSettings: [
      {
        workspaceResourceId: logAnalyticsWorkspaceResourceId
        logCategoriesAndGroups: [
          {
            categoryGroup: 'allLogs'
            enabled: true
          }
        ]
        metricCategories: [
          {
            category: 'AllMetrics'
            enabled: true
          }
        ]
      }
    ]
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('Resource ID of the Cosmos DB account')
output resourceId string = cosmosAccount.outputs.resourceId

@description('Name of the Cosmos DB account')
output resourceName string = cosmosAccount.outputs.name

@description('Cosmos DB account endpoint')
output endpoint string = cosmosAccount.outputs.endpoint
