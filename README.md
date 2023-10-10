# SwizlyPeasy.Clusters
Library for seamless integration of service discovery providers (Consul / Eureka) into yarp.

## Introduction
Yarp allows automated and transparent route and cluster configuration updates. This is something I've been able to verify in the SwizlyPeasy.Gateway project. So here I'm trying to generalize what has been implemented for consul in the SwizlyPeasy.Gateway project, so that other discovery providers can be used. 

Finally, I'd like to offer a set of extension methods for configuring and integrating a discovery provider service. In this way, cluster configuration can be updated automatically according to the information provided by the discovery provider.

## Usage
There are three things to do in order to use this functionality: 
- First, create a class that implements ```IServiceDiscoveryProviderClient```, 
- use the ```LoadFromServiceDiscoveryProvider<T>``` extension method with T corresponding to your ```IServiceDiscoveryProviderClient``` implementation,
- add the service discovery configuration in appsettings.json (check the dto ```SwizlyPeasyConfig```)
- and finally provide a default "routes.config.json" file in the executable folder or somewhere else.
