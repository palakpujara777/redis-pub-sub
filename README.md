**📞 Call Processing Demo with Redis Cache and Pub/Sub**
 
This project demonstrates a Call Processing system built using ASP.NET Core and integrated with Redis for high-speed data access and Pub/Sub messaging. It processes call prefix data from CSV files, manages country codes, detects duplicates, and uses Redis to cache and publish changes in real time.
 
**🔍 1. What is Redis and Publisher/Subscriber?**
Redis is an in-memory data structure store used as a database, cache, and message broker.
The Publisher/Subscriber (Pub/Sub) pattern in Redis allows messages to be published on a channel and received by subscribers in real time.
 
Purpose in this project:
- Redis is used for caching country code mappings and calling numbers.
- Pub/Sub enables real-time updates across components when country codes are added, updated, or deleted.
 
**🧰 2. Prerequisites**
Before running this project, ensure the following tools and services are available:
- .NET 8.0 SDK
- Redis Server (local/remote or Docker)
- Docker (optional, to run Redis container)
- IDE like Visual Studio 2022+ or VS Code
- Postman or Swagger for testing API
 
**🧱 3. Redis Setup Instructions**
 
🖥️ Option A: Install Redis Locally
- Download Redis: https://redis.io/download
- Extract and install.
- Start Redis using command line.
 
🐳 Option B: Run Redis via Docker
    docker run --name redis-call-demo -p 6379:6379 -d redis
 
🧪 Verify Redis Is Running
You can use Redis CLI:
    redis-cli
    127.0.0.1:6379> PING
    PONG
 
**⚙️ 4. Backend Configuration Steps**
 
📦 Required NuGet Packages
- StackExchange.Redis
- Microsoft.Extensions.Caching.StackExchangeRedis
- Microsoft.AspNetCore.Mvc.NewtonsoftJson
 
⚙️ appsettings.json
{
  "CacheSettings": {
    "ConnectionString": "localhost:6379"
  }
}
 
🧾 Program.cs Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["CacheSettings:ConnectionString"];
});
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<CallManager>();
builder.Services.AddHostedService<RedisSubscriberService>();
 
**🗂️ 5. Project Structure Overview**
 
📁 CallProcessingDemo
│
├── Controllers/
│   └── CallPrefixController.cs     // Handles API requests for country codes
│
├── Services/
│   ├── ICacheService.cs            // Cache abstraction interface
│   ├── RedisCacheService.cs        // Redis implementation of ICacheService
│   ├── CallManager.cs              // In-memory manager using Redis and Pub/Sub
│   └── RedisSubscriberService.cs   // Background service to listen to Pub/Sub
│
├── FileProcessing/
│   └── ProcessFile.cs              // Console-driven file processing
│
├── Models/
│   └── CallEntry.cs                // Model for calling number entries
 
**📡 6. API Endpoints Summary**
 
➕ Add or Update Country Code
POST /CallPrefix/AddCountryDetails?code={code}&country={country}
Adds or updates a country code (e.g., 91, India) and publishes a notification.
 
📋 Get All Country Codes
GET /CallPrefix/GetAllCodeDetails
Retrieves all country code and country name mappings from Redis.
 
❌ Delete Country Code
DELETE /CallPrefix/DeleteCountryDetailsByCode?code={code}
Deletes a country code and publishes a deletion event.
 
**🏗️ 7. How the System Works**
 
🔄 File Processing
Command:
>> Process <file_path>
 
Reads a CSV file with the format:
    Code,CallingNumber,Capital,Country
 
- Gets country name via Redis using Code.
- Checks for duplicate CallingNumber in Redis.
- Saves valid entries to a new file.
- Saves duplicates to a separate file.
 
Example row:
    91,9876543210,New Delhi,India
 
🔁 Real-time Updates via Pub/Sub
- When country code data is added, updated, or deleted, a message is published to a Redis channel.
- The CallManager listens and updates in-memory dictionary accordingly.
 
🧠 Redis Caching
Redis stores:
- Country code → country name
- Calling numbers (for duplication detection)
- Duplicate retention handled by setting a retention period on cached calling numbers.
 
**✅ 8. Example Use Case**
 
Input File:
Code,CallingNumber,Capital,Country
91,9876543210,New Delhi,India
1,2025550182,Washington D.C.,USA
91,9876543210,New Delhi,India
 
Output:
✅ ValidRecords.txt:
Contains unique entries.
 
❌ DuplicateRecords.txt:
Contains 91,9876543210 because it appeared twice.
 
**📝 9. Conclusion**
 
This project illustrates how to:
- Integrate Redis as a cache layer
- Implement Pub/Sub for real-time data updates
- Process input files to detect duplicates
- Manage country codes dynamically via APIs
 
🔧 Ideal for scenarios needing real-time distributed data sync and fast lookups, such as call routing or telecom fraud detection.
