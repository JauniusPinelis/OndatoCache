# Project 
Source code for Web Api solution using latest dotnet LTS version, where user could store key-value pairs: key - string value - `List<object>`

## Requirements

* .NET 5 installed
* Microsoft Sql Server for Database configuration

## Running

Adjust configuration in `Appsettings.json` and 'dotnet run' or run using Visual Studio 
To Choose between `DbCache` and `MemoryCache`, you have to configure `CacheType`

## Implementation
* Both `DbCache` and `MemoryCache` implement the same `ICache` interface and be substituted by modifying configuration.
* Integration and unit tests (XUnit, Moq, InMemoryDb, Autofixture) to ensure correct behaviour.
* `Hangfire` library to schedule automated task to clean cache (configured to run every minute atm)
* `FluentValidation` validators for input validation
* Factories responsible for `Cache` and `CacheItem` creation
* Generic implementations to allow cache implementation to be extended if needed


## Possible Improvements
* `DbCache` can be extracted into more generic implementation. Generic key can be serialized into string
* Generics implementation can be improved -> i think I overcomplicated the solution alot by imperfect use of generics. I could have modified `CacheItem` to include `IEnumerable<T>` rather `T` which probs would have allowed me to improve generic implementation and then i probs would not need `ObjectListCacheService`.
* Maybe instead of using TimeSpans and Cron formats I coulds have used just seconds in configuration which would have made it simplier and helped me to avoid Helper methods and etc.
* Add `Docker-compose` as well as `FluentDocker` in tests (to replace inmemory with real db ran in docker)