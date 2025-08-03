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