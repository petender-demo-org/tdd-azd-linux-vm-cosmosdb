@description('Name of the Network Security Group')
param nsgName string

@description('Name of the Virtual Network')
param vnetName string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

// ============================================================================
// Network Security Group (AVM)
// ============================================================================

module nsg 'br/public:avm/res/network/network-security-group:0.5.3' = {
  name: '${nsgName}-deploy'
  params: {
    name: nsgName
    location: location
    tags: tags
    securityRules: [
      {
        name: 'AllowSSH'
        properties: {
          priority: 100
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '22'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'AllowHTTP'
        properties: {
          priority: 200
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '80'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
      {
        name: 'AllowHTTP8080'
        properties: {
          priority: 210
          direction: 'Inbound'
          access: 'Allow'
          protocol: 'Tcp'
          sourcePortRange: '*'
          destinationPortRange: '8080'
          sourceAddressPrefix: '*'
          destinationAddressPrefix: '*'
        }
      }
    ]
  }
}

// ============================================================================
// Virtual Network (AVM)
// ============================================================================

module vnet 'br/public:avm/res/network/virtual-network:0.9.0' = {
  name: '${vnetName}-deploy'
  params: {
    name: vnetName
    location: location
    tags: tags
    addressPrefixes: [
      '10.0.0.0/16'
    ]
    subnets: [
      {
        name: 'snet-compute'
        addressPrefix: '10.0.1.0/24'
        networkSecurityGroupResourceId: nsg.outputs.resourceId
      }
    ]
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('Resource ID of the NSG')
output nsgResourceId string = nsg.outputs.resourceId

@description('Resource ID of the VNet')
output vnetResourceId string = vnet.outputs.resourceId

@description('Resource ID of the compute subnet')
output subnetResourceId string = vnet.outputs.subnetResourceIds[0]
