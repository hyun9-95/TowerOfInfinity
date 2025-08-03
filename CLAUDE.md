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
Efficiently translates missing localization data using temporary Python script:

1. **Generate** temporary Python translation script
2. **Execute** script to process `Assets/Data/Excels/Localization.csv`:
   - Identify empty English and SimplifiedChinese fields (columns 4 and 5)
   - Translate Korean text to missing languages using translation dictionary
   - Update CSV file directly with natural translations
3. **Clean up** temporary script after execution
4. **Report** translation results

**Usage**: Simply type `/translate` to execute the complete workflow.

**Advantages**:
- Minimal token usage for large datasets
- Efficient batch processing
- Maintains CSV format compatibility with DataGenerator

**Files Modified**:
- `Assets/Data/Excels/Localization.csv` (updated with translations)

# important-instruction-reminders
ALWAYS follow the code style guidelines above when writing or modifying code.