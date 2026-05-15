@description('Name of the Log Analytics workspace')
param name string

@description('Azure region')
param location string

@description('Resource tags')
param tags object

module logAnalytics 'br/public:avm/res/operational-insights/workspace:0.15.1' = {
  name: '${name}-deploy'
  params: {
    name: name
    location: location
    tags: tags
    skuName: 'PerGB2018'
    dataRetention: 30
  }
}

@description('Resource ID of the Log Analytics workspace')
output resourceId string = logAnalytics.outputs.resourceId

@description('Name of the Log Analytics workspace')
output resourceName string = logAnalytics.outputs.name
