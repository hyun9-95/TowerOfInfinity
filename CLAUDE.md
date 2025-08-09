# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

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
- **Available methods**: Logger.Log(), Logger.Warning(), Logger.Error()
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

# important-instruction-reminders
ALWAYS follow the code style guidelines above when writing or modifying code.