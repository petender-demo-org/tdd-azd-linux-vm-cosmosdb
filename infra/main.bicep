targetScope = 'resourceGroup'

// ============================================================================
// Parameters
// ============================================================================

@description('Azure region for all resources')
param location string = 'eastus2'

@description('Environment name used in resource naming (e.g., demo, dev)')
@minLength(1)
@maxLength(20)
param environment string

@description('Project name for tagging')
param projectName string = 'linux-vm-cosmosdb'

@description('Principal ID of the deploying user. Azure Developer CLI populates this automatically.')
param principalId string = ''

@description('SSH public key for VM authentication. Provide via AZURE_VM_SSH_PUBLIC_KEY env var.')
@secure()
param sshPublicKey string

// ============================================================================
// Variables
// ============================================================================

var uniqueSuffix = uniqueString(resourceGroup().id)

var tags = {
  Environment: environment
  ManagedBy: 'Bicep'
  Project: projectName
  SecurityControl: 'Ignore'
}

// Resource naming — CAF conventions
var logAnalyticsName = 'log-${projectName}-${environment}'
var nsgName = 'nsg-compute-${environment}'
var vnetName = 'vnet-${projectName}-${environment}'
var publicIpName = 'pip-${projectName}-${environment}'
var vmName = 'vm-linuxvm-${environment}'
var cosmosName = 'cosmos-linuxvm-${environment}-${take(uniqueSuffix, 6)}'

// Cosmos DB Built-in Data Contributor role (data plane)
var cosmosDataContributorRoleId = '00000000-0000-0000-0000-000000000002'

// ============================================================================
// Tag the Resource Group (azd creates it without tags)
// ============================================================================

resource rgTags 'Microsoft.Resources/tags@2024-03-01' = {
  name: 'default'
  properties: { tags: tags }
}

// ============================================================================
// Module Deployments — Phase 1: Monitoring
// ============================================================================

module logAnalytics 'modules/log-analytics.bicep' = {
  name: 'log-analytics-deploy'
  params: {
    name: logAnalyticsName
    location: location
    tags: tags
  }
}

// ============================================================================
// Module Deployments — Phase 2: Networking
// ============================================================================

module networking 'modules/networking.bicep' = {
  name: 'networking-deploy'
  params: {
    nsgName: nsgName
    vnetName: vnetName
    location: location
    tags: tags
  }
}

// ============================================================================
// Module Deployments — Phase 3: Compute
// ============================================================================

module vm 'modules/virtual-machine.bicep' = {
  name: 'virtual-machine-deploy'
  params: {
    name: vmName
    location: location
    tags: tags
    vmSize: 'Standard_B2s'
    sshPublicKey: sshPublicKey
    subnetResourceId: networking.outputs.subnetResourceId
    nsgResourceId: networking.outputs.nsgResourceId
    publicIpName: publicIpName
  }
}

// ============================================================================
// Module Deployments — Phase 4: Data
// ============================================================================

module cosmosDb 'modules/cosmos-db.bicep' = {
  name: 'cosmos-db-deploy'
  params: {
    name: cosmosName
    location: location
    tags: tags
    logAnalyticsWorkspaceResourceId: logAnalytics.outputs.resourceId
  }
}

// ============================================================================
// RBAC — VM Managed Identity → Cosmos DB Data Contributor
// ============================================================================

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2024-05-15' existing = {
  name: cosmosName
}

resource vmCosmosRbac 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-05-15' = {
  parent: cosmosAccount
  name: guid(cosmosAccount.id, vmName, cosmosDataContributorRoleId)
  properties: {
    roleDefinitionId: '${cosmosAccount.id}/sqlRoleDefinitions/${cosmosDataContributorRoleId}'
    principalId: vm.outputs.principalId
    scope: cosmosAccount.id
  }
  dependsOn: [
    cosmosDb
  ]
}

// ============================================================================
// RBAC — Deployer → Cosmos DB Data Contributor (data plane access)
// ============================================================================

resource deployerCosmosRbac 'Microsoft.DocumentDB/databaseAccounts/sqlRoleAssignments@2024-05-15' = if (!empty(principalId)) {
  parent: cosmosAccount
  name: guid(cosmosAccount.id, principalId, cosmosDataContributorRoleId)
  properties: {
    roleDefinitionId: '${cosmosAccount.id}/sqlRoleDefinitions/${cosmosDataContributorRoleId}'
    principalId: principalId
    scope: cosmosAccount.id
  }
  dependsOn: [
    cosmosDb
  ]
}

// ============================================================================
// Outputs
// ============================================================================

@description('VM name')
output vmName string = vm.outputs.resourceName

@description('Cosmos DB account endpoint')
output cosmosEndpoint string = cosmosDb.outputs.endpoint

@description('Cosmos DB account name')
output cosmosAccountName string = cosmosDb.outputs.resourceName

@description('SSH connection command')
output sshCommand string = 'ssh azureuser@<public-ip>'
