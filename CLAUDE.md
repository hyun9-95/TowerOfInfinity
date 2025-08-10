# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**TowerOfInfinity** is a Unity 2D action RPG game with a sophisticated MVC architecture, modular character system, and addressable asset management. The project uses Unity 6000.0.54f1 with URP (Universal Render Pipeline) and includes Korean localization with multi-language support.

**Key Technologies:**
- Unity 2D with URP for rendering
- UniTask for async operations
- Addressables for asset management
- NavMeshPlus for 2D pathfinding
- DOTween for animations
- Custom MVC framework with data binding

## Development Commands

### Unity Build and Test
- **Build**: Use Unity Editor Build Settings (File → Build Settings) or Unity Cloud Build
- **Play Mode Testing**: Use Unity Editor Play button to test scenes in `Assets/Resources/Scenes/`
- **Unit Tests**: Use Unity Test Framework (Window → General → Test Runner)
- **Solution**: Open `TowerOfInfinity.sln` in Visual Studio/Rider for full project debugging

### Key Editor Tools
- **Window → Balance Editor**: Edit ScriptableObject balance data for abilities and battle events
- **Window → Script Generator**: Generate MVC controllers, views, and data containers
- **Window → Addressable Build Generator**: Manage addressable asset builds and path generation
- **Addressable Groups**: Configure asset loading in Window → Asset Management → Addressables → Groups

## Architecture Overview

### MVC Pattern
The project follows a strict MVC (Model-View-Controller) pattern:
- **Controllers**: Handle business logic and user input (`Assets/Scripts/Contents/*/`)
- **ViewModels**: Data binding and state management (implements `IBaseViewModel`)
- **Views**: UI presentation layer (extends `BaseView`)
- **Models**: Data structures and ScriptableObjects for game balance

### Manager System
Centralized singleton managers handle core systems:
- `GameManager`: Main game initialization and flow control
- `FlowManager`: Scene and state transitions between game flows (Intro, Battle, Town, etc.)
- `UIManager`: UI canvas management and view lifecycle
- `AddressableManager`: Asset loading and memory management
- `BattleSystemManager`: Combat mechanics and battle flow
- `LocalizationManager`: Multi-language text support

### Modular Character System
Characters use a component-based module system:
- `CharacterUnit`: Base character controller with state machine
- `ScriptableCharacterModule`: Modular behaviors (UI, collision damage, exp spawning)
- `CharacterState`: State pattern for different character behaviors (Idle, Move, Attack, etc.)
- `BattleEventTrigger`: Event-driven ability system with projectiles and effects

### Data Management
- **Excel/CSV Data**: `Assets/Data/Excels/` contains game balance data
- **ScriptableObjects**: Generated from CSV data for runtime balance configuration
- **Addressables**: Assets organized by feature groups for efficient loading
- **Localization**: CSV-based translation system supporting Korean, English, Chinese

### Core Battle System Flow
The battle system follows a structured event-driven architecture:

**AbilityProcessor** → **Ability** → **BattleEventTrigger** → **TriggerUnit** → **BattleEvent**

1. **AbilityProcessor**: Entry point that manages ability execution timing and conditions
   - Handles ability cooldowns, resource costs, and casting requirements
   - Validates target selection and ability prerequisites
   - Initiates the ability execution chain

2. **Ability**: Core ability logic container (ScriptableObject-based)
   - Defines ability properties: damage, range, effects, animation data
   - Contains ability-specific behavior and rule validation
   - Links to BattleEventTrigger for execution

3. **BattleEventTrigger**: Execution coordinator for ability effects
   - Manages timing, sequencing, and chaining of battle events
   - Handles projectile spawning, area effects, and multi-hit abilities
   - Coordinates with animation system and visual effects

4. **TriggerUnit**: Individual effect processing unit
   - Processes single battle events (damage, healing, buffs, debuffs)
   - Manages target filtering and effect application
   - Handles conditional effects and state changes

5. **BattleEvent**: Atomic battle operation
   - Executes specific game state changes (HP modification, status effects)
   - Triggers UI updates, particle effects, and audio feedback
   - Records battle log data and statistics

## Code Style Guidelines

### Conditional Statements and Early Returns
- **Always use negative conditions first** to reduce nesting with early returns or continues
- **Minimize scope depth** by returning/continuing early when conditions are not met
- **Omit braces `{}`** for single-line if/for statements

```csharp
// ✅ Good - Early return with negative condition
public void ProcessData(Data data)
{
    if (data == null)
        return;
    
    if (string.IsNullOrEmpty(data.Name))
        return;
    
    if (!data.IsValid)
        return;
    
    // Main logic here with minimal nesting
    ProcessValidData(data);
}

// ✅ Good - Early continue in loops
foreach (var item in items)
{
    if (item == null)
        continue;
    
    if (!item.IsEnabled)
        continue;
    
    ProcessItem(item);
}

// ❌ Bad - Nested conditions
public void ProcessData(Data data)
{
    if (data != null)
    {
        if (!string.IsNullOrEmpty(data.Name))
        {
            if (data.IsValid)
            {
                ProcessValidData(data);
            }
        }
    }
}
```

### Single-Line Statement Rules
- **Omit braces** when if/for/while statements execute only one line
- **Use braces** when multiple statements or complex logic is involved

```csharp
// ✅ Good - Single line without braces
if (condition)
    return;

for (int i = 0; i < count; i++)
    ProcessItem(i);

// ✅ Good - Multiple lines with braces
if (condition)
{
    DoSomething();
    DoSomethingElse();
}
```

### Method Structure Pattern
```csharp
public ReturnType MethodName(parameters)
{
    // 1. Early validation checks (negative conditions first)
    if (parameter == null)
        return defaultValue;
    
    if (!IsValidCondition())
        return defaultValue;
    
    // 2. Main logic with minimal nesting
    var result = ProcessData();
    return result;
}
```

## Automation Workflows

### `/translate`
Directly translates missing localization data in CSV format:

1. **Read** `Assets/Data/Excels/Localization.csv`
2. **Find** empty English/SimplifiedChinese fields (columns 4 and 5)
3. **Translate** Korean text using built-in translation dictionary
4. **Update** CSV file with translated text
5. **Report** changes made

**Usage**: Simply type `/translate` to execute the workflow.

**Translation Dictionary**:
- Korean → English: 탑→Tower, 무한→Infinity, 로딩→Loading, etc.
- Korean → Chinese: 탑→塔, 무한→无限, 로딩→加载, etc.

**Files Modified**:
- `Assets/Data/Excels/Localization.csv` (updated with translations)

## Code Restrictions

### Prohibited Features
- **Nullable operators (`??`, `?.`)**: Use explicit null checks instead
- **Reflection**: Direct property/method access prohibited for performance and maintainability
- **Comments**: Do not add comments unless explicitly requested by user

### Logging Guidelines
- **Always use Logger class** for all logging operations instead of Unity's Debug.Log
- **Available methods**: Logger.Log(), Logger.Warning(), Logger.Error(), Logger.BattleLog(), Logger.Success()
- **Apply to all contexts**: Runtime code, Editor scripts, and custom tools
- **No emojis**: Do not use emojis in log messages, comments, or code

```csharp
// Bad - Nullable operators
var result = obj?.Property ?? defaultValue;

// Good - Explicit null checks
var result = defaultValue;
if (obj != null && obj.Property != null)
    result = obj.Property;

// Bad - Reflection
var property = typeof(MyClass).GetProperty("PropertyName");
property.SetValue(instance, value);

// Good - Direct access with public methods
instance.SetPropertyName(value);

// Bad - Using emojis in logs
Logger.Log("✅ Data generation completed successfully!");

// Good - No emojis in logs
Logger.Log("Data generation completed successfully.");
```

## Common Development Patterns

### Creating New UI Screens
1. Use Script Generator: Window → Script Generator → MVC
2. This generates: `{Name}Controller.cs`, `{Name}ViewModel.cs`, `{Name}View.cs`
3. Prefab is auto-created in `Assets/Addressable/UI/{Name}/`
4. UI type enum is automatically updated in `UiTypeDefine.cs`

### Adding New Character Abilities
1. Create ScriptableObject balance data using Balance Editor
2. Implement ability logic in `Assets/Scripts/Battle/Ability/`
3. Create corresponding BattleEventTrigger for execution
4. Add addressable prefabs for visual effects

### Character Module Development
1. Extend `ScriptableCharacterModule` for new behaviors
2. Implement corresponding model class extending `IModuleModel`
3. Add to `ScriptableCharacterModuleGroup` for character assignment

## File Structure Guidelines

### Script Organization
- **Controllers**: `Assets/Scripts/Contents/{FeatureName}/{FeatureName}Controller.cs`
- **ViewModels**: `Assets/Scripts/Contents/{FeatureName}/{FeatureName}ViewModel.cs`  
- **Views**: `Assets/Scripts/Contents/{FeatureName}/{FeatureName}View.cs`
- **Managers**: `Assets/Scripts/Manager/{Name}Manager.cs`
- **Utilities**: `Assets/Scripts/Util/{Name}Utils.cs`
- **Data Structures**: `Assets/Scripts/Define/{Type}Define.cs`

### Asset Organization
- **UI Prefabs**: `Assets/Addressable/UI/{FeatureName}/`
- **Character Assets**: `Assets/Addressable/Character/{Type}/`
- **Battle Effects**: `Assets/Addressable/HitEffect/`
- **ScriptableObject Data**: `Assets/Addressable/{System}Core/`

### Template System
The project includes code generation templates in `Assets/Templates/` for:
- MVC components
- Manager classes
- Data containers
- Editor windows