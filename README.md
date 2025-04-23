# GitHub README


# Unity Leaderboard System (Experimental)

![Unity Version](https://img.shields.io/badge/Unity-2021.3%2B-blue)

An experimental leaderboard system implementing core CRUD operations with optimized UI rendering and data persistence. Built to test software engineering architecture patterns in Unity.

## Features

- 🏆 Dynamic leaderboard with player profiles
- 📸 Profile picture management
- ✅ CRUD operations:
  - Add new players
  - Update existing entries
  - Remove players
  - View detailed profiles
- 📱 Responsive UI using Unity Layout Groups
- 💾 Persistent storage system with:
  - Async file operations
  - Separate data repositories
  - Service base design
  - Scalable infrastructure
  - Flexible Codebase

## Installation

1. Clone repository
2. Open in Unity 6.0.44f1 or newer
3. Navigate to Main scene
4. Run the leaderboard demo

## Project Structure

```
Assets/
├── Storage/
│   ├── File system adapters (Text/Photo)
│   └── Async IO operations
├── Repositories/
│   ├── Player data management
│   ├── Avatar handling
│   └── Leaderboard persistence
├── Services/
│   ├── Business logic layer
│   ├── Authentication service
│   └── Score management
└── UI/
    ├── Leaderboard view components
    ├── Profile display system
    └── Responsive layout controllers
```
```mermaid
classDiagram

    %% ========== Storage Layer ==========
    class IStorageAdapter {
        <<interface>>
        +ReadFromFileAsync(string path) Task<byte[]>
        +WriteToFileAsync(string path, byte[] data) Task
        +Exists(string path) bool
    }

    class TextFileAdapter {
        +ReadFromFileAsync(string path) Task<string>
        +WriteToFileAsync(string path, string text) Task
    }

    class PhotoFileAdapter {
        +ReadFromFileAsync(string path) Task<byte[]>
        +WriteToFileAsync(string path, byte[] imageData) Task
    }

    IStorageAdapter <|.. TextFileAdapter
    IStorageAdapter <|.. PhotoFileAdapter


    %% ========== Repository Layer ==========
    class IPlayerRepository {
        <<interface>>
        +AddPlayerAsync(Player player) Task
        +GetByIdAsync(string id) Task<Player>
    }

    class IAvatarRepository {
        <<interface>>
        +GetByIdAsync(string id) Task<byte[]>
    }

    class ILeaderboardRepository {
        <<interface>>
        +LoadScoresAsync() Task<List<PlayerScore>>
    }

    class PlayerRepository {
        -_textStorage: TextFileAdapter
        +LoadPlayersAsync() Task<List<Player>>
    }

    class AvatarRepository {
        -_photoStorage: PhotoFileAdapter
        +HasAvatarAsync(string playerId) Task<bool>
    }

    class LeaderboardRepository {
        -_textStorage: TextFileAdapter
        +LoadScoresAsync() Task<List<PlayerScore>>
    }

    IPlayerRepository <|.. PlayerRepository
    IAvatarRepository <|.. AvatarRepository
    ILeaderboardRepository <|.. LeaderboardRepository


    %% ========== Services Layer ==========
    class PlayersAuthenticationService {
        -_playerRepo: PlayerRepository
        -_avatarRepo: AvatarRepository
        +UpdatePlayerAvatar(string id, byte[] avatar) Task
    }

    class LeaderboardService {
        -_leaderboardRepo: ILeaderboardRepository
        -_authService: PlayersAuthenticationService
        +GetSortedScores() Task<List<PlayerScore>>
    }

    class LeaderboardJunction {
        -_leaderboardService: LeaderboardService
        +Setup() IEnumerator
    }


    %% ========== UI Layer ==========
    class LeaderboardViewMenu {
        -_leaderboardJunction: LeaderboardJunction
        -_authService: PlayersAuthenticationService
        +RefreshLeaderboard() Task
    }

    class PlayerLeaderboardRecord {
        -_player: Player
        -_score: PlayerScore
        +SetData(Player player, PlayerScore score) void
    }


    %% ========== Data Models ==========
    class Player {
        +string Id
        +string Name
        +int Score
    }

    class PlayerScore {
        +string PlayerId
        +int Value
    }


    %% ========== Final Relationships ==========
    PlayerRepository --> TextFileAdapter : Uses
    AvatarRepository --> PhotoFileAdapter : Uses
    LeaderboardRepository --> TextFileAdapter : Uses

    PlayersAuthenticationService --> PlayerRepository : Uses
    PlayersAuthenticationService --> AvatarRepository : Uses

    LeaderboardService --> ILeaderboardRepository : Depends on
    LeaderboardService --> PlayersAuthenticationService : Coordinates

    LeaderboardJunction --> LeaderboardService : Manages

    LeaderboardViewMenu --> LeaderboardJunction : Uses
    LeaderboardViewMenu --> PlayersAuthenticationService : Requests data
    LeaderboardViewMenu --> PlayerLeaderboardRecord : Creates

    PlayerLeaderboardRecord --> Player : Displays
    PlayerLeaderboardRecord --> PlayerScore : Displays

    LeaderboardService --> PlayerScore : Processes
```
## Key Components

- **Async Operations**: Non-blocking file operations using C# async/await
- **Storage Adapters**: Abstract file system operations
- **Repository Pattern**: Separates data access from business logic
- **Service Layer**: Handles core application logic
- **Optimized UI**: Uses pooling and selective updates

## Experimental Notes

This system was created to explore:
- Clean architecture principles
- Repository pattern implementation
- Service base design
- Async data persistence
- UI optimization techniques
- Modular system design

Not recommended for production use - some components are simplified for experimentation purposes.

## Possible Improvements

- Add cloud storage integration
- Implement binary serialization
- Add undo/redo system
- Create automated tests
- Better Sprites


## Diagram Legend

- **Storage Layer**: File system operations and adapters
  - `IStorageAdapter`: Interface for storage operations
  - Concrete adapters handle specific data types (text/photos)
  
- **Repository Layer**: Data management
  - Player/Avatar repositories handle domain objects
  - Uses storage adapters for persistence
  - Contains data models (Player, PlayerScore)

- **Service Layer**: Business logic
  - Coordinates repositories and UI
  - Handles authentication and score management
  - Uses coroutine runner for async operations

- **UI Layer**: Presentation
  - View components for leaderboard display
  - Profile management interface
  - Responsive layout controllers

## Flow
1. UI components request data through Services
2. Services coordinate repositories
3. Repositories use storage adapters for persistence
4. Data flows back through same chain
```mermaid
graph TD
    UI[LeaderboardViewMenu] --> Junction[LeaderboardJunction]
    UI --> Auth[PlayersAuthenticationService]
    UI --> Record[PlayerLeaderboardRecord]
    Junction --> Service[LeaderboardService]
    Service --> LRepo[LeaderboardRepository]
    Service --> Auth
    Auth --> PRepo[PlayerRepository]
    Auth --> ARepo[AvatarRepository]
    LRepo[LeaderboardRepository] --> textFile[TextFileAdapter]
    PRepo[PlayerRepository] --> textFile[TextFileAdapter]
    ARepo[AvatarRepository] --> photoFile[PhotoFileAdapter]
```
## Key Interfaces
- `IStorageAdapter`: File operations contract
- `IPlayerRepository`: Player CRUD operations
- `ILeaderboardService`: Score management API
