Cloud Application Framework & Extensions (the correct short spelling is CloudFx, with no space between "Cloud" and "Fx", with upper-case "F" and lower-case "x") is a set of pre-assembled patterns intended to jump-start the implementation of the cloud-based solutions on the Windows Azure platform.

The CloudFx framework was developed with a single goal in mind that is to provide a Swiss army knife for Windows Azure developers to enable building high-end cloud-based solutions that scale well and never fail.

The framework is founded on the abstraction and extensibility model that hides the vast majority of complex mechanical aspects of how our platform services work and enables the developers to focus on those things that matter the most.

The CloudFx framework is intended to be used by the developers who find themselves in the affinity with any of the following statements:

* As a developer, I want to significantly cut the time to production so that I can play more golf.
* As a developer, I want to experience a smooth glide path when implementing cloud-based solutions.
* As a developer, I value a smooth journey towards a Go-Live date and get frustrated when something forces me to take a bumpy road.
* As a developer, I often find it very frustrating when simple & obvious things don't work as expected. It's the 21st century, not the 80's.

The example of how the CloudFx framework was used in the implementation of a real-world complex cloud-based solution can be found here: http://slidesha.re/oXrgzW

The detailed list of capabilities along with full documentation will appear in due course. Please follow us for updates.

Change history:

* June 2013 v2.0.0.0
---------------------------
Updated the framework to use Windows Azure SDK 2.0 and all latest NuGet package dependencies

* August 2013 v2.0.0.1
---------------------------
Fixed TraceStorageError to ensure the additional information about storage errors is available
Improved Rx scheduling and exception handling in ServiceBusPublishSubscribeChannel
Improved StorageTransientErrorDetectionStrategy to include recognition logic for IOException
Improved ExceptionTextFormatter to clearly mark inner exceptions when formatting the output
Fixed IConfigurationProviderExtension.GetSetting to return the correct default value
Fixed extension methods for ITraceEventProvider to check if tracing was enabled before relaying exceptions to the subscribers

* August 2013 v2.1.0-rc
---------------------------
Refreshed assembly dependencies for Windows Azure SDK v2.1
Updated the NuGet package dependencies such as WindowsAzure.Storage v2.1-rc and WindowsAzure.ServiceBus SDK v2.1
Added SQL error code 40174 to the list of known transient exceptions for Windows Azure SQL Database service
Added new convenience methods in ServiceBusEndpointInfo to simply construction of ServiceBusEndpointInfo objects
Added a new implementation of ICloudStorageEntitySerializer which performs serialization and deserialization using the same mechanism as implemented by BrokeredMessage
Improved ReliableServiceBusQueue.Delete<T> and ReliableCloudQueueStorage.Delete<T> to stop throwing an exception if a message could not be deleted from the queue (use the Boolean result instead)
Renamed all SomeActionAsync methods returning IObservable to ObserveSomeAction to comply with naming convention in asynchronous programming with Async and Await keywords in the .NET Framework 4.5
Introduced ReliableCloudBlobStorage.Delete<T> with built-in intelligence such as ability to pass lease ID

* August 2013 v2.1.1-rc
---------------------------
Added the UseHttps property in StorageAccountInfo which tells the Windows Azure Storage client to use HTTPS to connect to storage service endpoints (this was enabled by default in prior versions of CloudFx)
CloudQueueLocation was renamed to CloudQueueDescription to better align with the class naming convention adopted in parts of the Windows Azure SDK
Introduced StorageAccountObserver that projects the Windows Azure storage account as an observable sequence of objects carrying telemetry information about the account and its storage entities, such as current queue depth
Added a new sample called VerySimpleAsyncBlobUpload showing the simplistic method for uploading files into a blob container asynchronously

* August 2013 v2.1.2-rc
---------------------------
Updated the NuGet transform for *.config files to correct the version number for Microsoft.WindowsAzure.Diagnostics.dll and use v2.1

* September 2013 v2.1.3
---------------------------
Introduced optional extended behavior in ReliableSqlConnection that enables clearing the SQL connection pool upon encountering a timeout (as per not widely known point-in-time best practice)
Added support for Shared Access Signatures (SAS) in ServiceBusEndpointInfo to allow the use of SAS-based connection strings to the Windows Azure Service Bus
Added support for ExcludeAttribute that provides a declarative mechanism for excluding select properties from table entities (as requested by community members)
Updated the NuGet package dependencies such as WindowsAzure.Storage v2.1 (final release) and WindowsAzure.ServiceBus v2.1.3
Added support for customization in message TTL and enqueue delay in ReliableCloudQueueStorage (as requested by community members)