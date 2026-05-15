using './main.bicep'

param environment = readEnvironmentVariable('AZURE_ENV_NAME', 'demo')
param location = readEnvironmentVariable('AZURE_LOCATION', 'eastus2')
param projectName = 'linux-vm-cosmosdb'
param principalId = readEnvironmentVariable('AZURE_PRINCIPAL_ID', '')
param sshPublicKey = readEnvironmentVariable('AZURE_VM_SSH_PUBLIC_KEY', '')
