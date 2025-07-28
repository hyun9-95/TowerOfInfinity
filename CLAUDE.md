# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tower of Infinity is a Unity 2D action RPG built with Unity 6000.0.24f1. The project uses a sophisticated MVC architecture with modular character systems, addressable asset management, and event-driven battle mechanics.

## Development Commands

### Unity Build and Test
- **Build**: Use Unity Editor Build Settings or Unity Cloud Build
- **Play Mode Testing**: Use Unity Editor Play button to test scenes
- **Unit Tests**: Use Unity Test Framework (no custom test scripts found)
- **Code Generation**: Use Editor Windows → Script Generator for data structures and MVC components

### Key Editor Tools
- **Window → Balance Editor**: Edit ScriptableObject balance data for abilities and battle events
- **Window → Script Generator**: Generate MVC controllers, views, and data containers
- **Window → Addressable Build Generator**: Manage addressable asset builds

## Core Architecture

### MVC Pattern Implementation
- **Controllers**: Extend `BaseController` with async lifecycle (LoadingProcess → Process → Exit)
- **Views**: Extend `BaseView` (which inherits `AddressableMono`) for automatic resource management
- **ViewModels**: Implement `IBaseViewModel` for data binding between controllers and views
- All UI follows this pattern: `BattleController` ↔ `BattleViewModel` ↔ `BattleView`

### Flow Management System
The game uses a hierarchical flow system managed by `FlowManager`:
- **IntroFlow**: Initial data loading and setup
- **LobbyFlow**: Main menu and character customization
- **BattleFlow**: Combat gameplay
- Each flow has async lifecycle with automatic resource cleanup

### Character System Architecture
- **CharacterUnit**: Core character entity with state machine and modular components
- **CharacterModule**: Pluggable functionality (UI, collision damage, etc.)
- **State Pattern**: ScriptableObject-based states with priority and conditions
- **Factory Pattern**: `CharacterFactory` handles instantiation and pooling

### Battle System
- **BattleSystemManager**: Coordinates all battle logic
- **BattleEventProcessor**: Handles damage, effects, and battle events
- **Observer Pattern**: Event-driven communication via `ObserverManager`
- **Ability System**: Data-driven abilities with scriptable balance objects

## Key Managers and Their Roles

- **GameManager**: Application entry point and global settings
- **FlowManager**: High-level game state transitions and scene management
- **UIManager**: View/popup stack management with addressable loading
- **BattleSystemManager**: Battle coordination and character switching
- **AddressableManager**: Async asset loading and handle management
- **ObjectPoolManager**: GameObject pooling for performance
- **DataManager**: Type-safe data container access for game configuration

## Development Patterns

### Addressable Asset Management
- All major assets use Unity Addressables for memory management
- `AddressableMono` base class provides automatic handle cleanup
- Resources automatically released during flow transitions
- Use `AddressableManager.LoadAssetAsync<T>()` for asset loading

### Data-Driven Design
- ScriptableObjects for character stats, abilities, and balance data
- JSON-based data containers with automatic code generation
- Editor tools generate data access code from templates

### Async Operations
- Extensive use of UniTask for async operations
- All manager operations are async-first
- Flow transitions use async loading processes

### Event-Driven Communication
- Observer pattern via `ObserverManager` for cross-system communication
- Battle events automatically trigger UI updates
- Character modules communicate through events

## File Organization

### Script Structure
- `/Assets/Scripts/Manager/`: Core system managers
- `/Assets/Scripts/Flow/`: Game flow controllers
- `/Assets/Scripts/Contents/`: MVC implementations for game features
- `/Assets/Scripts/Character/`: Character system and states
- `/Assets/Scripts/Battle/`: Combat system and abilities
- `/Assets/Scripts/MVCBase/`: Base classes for MVC pattern
- `/Assets/Scripts/Observer/`: Event system implementation
- `/Assets/Scripts/Editor/`: Custom editor tools and generators

### Asset Organization
- `/Assets/Addressable/`: Addressable asset groups organized by system
- `/Assets/Resources/`: Direct-load resources and managers
- `/Assets/Scripts/Define/`: Constants and enums
- `/Assets/Data/`: JSON data files for game configuration

## Character Customization System
- **CharacterBuilder**: Sprite-based character appearance system
- **SpriteLibraryBuilder**: Dynamic sprite library generation
- **UserCharacterAppearanceInfo**: Serializable character appearance data
- Supports modular character parts (armor, weapons, accessories)

## Performance Considerations
- Object pooling for frequently instantiated objects (characters, projectiles)
- Addressable system for memory-conscious asset loading
- Automatic resource cleanup between major state transitions
- UniTask for non-blocking async operations

## Testing and Debugging
- Use Unity Profiler for performance analysis
- Battle events can be monitored through `ObserverManager` logging
- Character states visible in Unity Inspector during play mode
- Addressable handles tracked automatically for memory leak detection

## Automation Workflows

### `/translate-localization`
Automatically translates and synchronizes localization data:

1. **Read** `Assets/Data/Jsons/Localization.json`
2. **Identify** empty English and SimplifiedChinese fields
3. **Translate** Korean text to missing languages:
   - Korean → English (natural translation)
   - Korean → Simplified Chinese (natural translation)
4. **Update** JSON file with translations
5. **Convert** updated JSON back to `Assets/Data/Excels/Localization.csv`
6. **Maintain** CSV format compatible with DataGenerator

**Usage**: Simply type `/translate-localization` to execute the complete workflow.

**Files Modified**:
- `Assets/Data/Jsons/Localization.json` (updated with translations)
- `Assets/Data/Excels/Localization.csv` (synchronized with JSON changes)