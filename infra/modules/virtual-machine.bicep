@description('Name of the Virtual Machine')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

@description('VM size')
param vmSize string = 'Standard_B2s'

@description('Admin username for the VM')
param adminUsername string = 'azureuser'

@description('SSH public key for authentication')
@secure()
param sshPublicKey string

@description('Resource ID of the subnet to deploy the VM into')
param subnetResourceId string

@description('Resource ID of the NSG to associate with the VM NIC')
param nsgResourceId string

@description('Public IP name for the VM')
param publicIpName string

// ============================================================================
// Virtual Machine (AVM)
// ============================================================================

module vm 'br/public:avm/res/compute/virtual-machine:0.22.1' = {
  name: '${name}-deploy'
  params: {
    name: name
    location: location
    tags: tags
    vmSize: vmSize
    osType: 'Linux'
    availabilityZone: 1
    imageReference: {
      publisher: 'Canonical'
      offer: '0001-com-ubuntu-server-jammy'
      sku: '22_04-lts-gen2'
      version: 'latest'
    }
    osDisk: {
      diskSizeGB: 32
      managedDisk: {
        storageAccountType: 'StandardSSD_LRS'
      }
      caching: 'ReadWrite'
      createOption: 'FromImage'
    }
    adminUsername: adminUsername
    disablePasswordAuthentication: true
    publicKeys: [
      {
        keyData: sshPublicKey
        path: '/home/${adminUsername}/.ssh/authorized_keys'
      }
    ]
    nicConfigurations: [
      {
        nicSuffix: '-nic-01'
        enableAcceleratedNetworking: false
        ipConfigurations: [
          {
            name: 'ipconfig01'
            subnetResourceId: subnetResourceId
            pipConfiguration: {
              publicIpNameSuffix: '-pip'
              name: publicIpName
              publicIPAllocationMethod: 'Static'
              skuName: 'Standard'
              availabilityZones: [
                1
                2
                3
              ]
            }
          }
        ]
        networkSecurityGroupResourceId: nsgResourceId
      }
    ]
    managedIdentities: {
      systemAssigned: true
    }
  }
}

// ============================================================================
// Outputs
// ============================================================================

@description('Resource ID of the VM')
output resourceId string = vm.outputs.resourceId

@description('Name of the VM')
output resourceName string = vm.outputs.name

@description('Principal ID of the VM system-assigned managed identity')
output principalId string = vm.outputs.?systemAssignedMIPrincipalId ?? ''
