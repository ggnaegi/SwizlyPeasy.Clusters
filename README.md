# SwizlyPeasy.Clusters
Library for seamless integration of service discovery providers (Consul / Eureka) into yarp.

## Introduction
Yarp enables automated and transparent route and cluster configuration file updates. This is something I've been able to verify in the SwizlyPeasy.Gateway project. So here I'm trying to generalize what has been implemented for consul in the SwizlyPeasy.Gateway project, so that other discovery providers can be used. 

Finally, I'd like to offer a set of extension methods for configuring and integrating a discovery provider service. In this way, cluster configuration can be updated automatically according to the information provided by the discovery provider. 
