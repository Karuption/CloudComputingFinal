# CloudComputingFinal
This is a containerized website for Cloud Computing and Distributed Systems final project at Oklahoma State

# Login password requirements
1. Longer than 6 charicters
2. At least 1 number
3. At least 1 special charicter
4. At least 1 uppercase charicter

# FrontEnd
- Completely in Vue.js
- Websocket updates for real time cross-device updates

# Backend 
- ASP.NET Core
- SignalR to provide websocket task notifications
- .NET Authentication providor via JWT
- DotNetCAP for messaging integration
  - Kafka for the messenger

# Database
- SQL
- Contains
  - User login
  - Task storage
  - Canvas integration access token and metadata
  
[Hosted Here](https://final.home.papederson.tech)
