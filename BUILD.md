# Building and Testing Shatranj

This document provides comprehensive instructions for building and testing the Shatranj chess game project.

## Prerequisites

### Required Software

1. **.NET 9 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
   - Verify installation: `dotnet --version` (should show 9.0.x or higher)

2. **IDE (Choose one)**
   - **Visual Studio 2022** (Community, Professional, or Enterprise)
     - Download: https://visualstudio.microsoft.com/downloads/
     - Workload required: ".NET desktop development"
   - **Visual Studio Code**
     - Download: https://code.visualstudio.com/
     - Extensions: C# Dev Kit, C# Extension
   - **JetBrains Rider** (Alternative)

3. **Git** (for cloning the repository)
   - Download: https://git-scm.com/downloads

### Platform Support

- ✅ **Windows** - Fully supported (including Windows Forms GUI)
- ✅ **Linux** - Supported (command-line only)
- ✅ **macOS** - Supported (command-line only)

Note: The Windows Forms GUI project (ShatranjMain) only builds on Windows with .NET 9.0-windows target framework.

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/YourUsername/Shatranj.git
cd Shatranj
```

### 2. Restore Dependencies

```bash
# Restore NuGet packages for all projects
dotnet restore
```

This will restore all dependencies for all projects in the solution.

---

## Building the Project

### Option 1: Using .NET CLI (Recommended)

#### Build All Projects

```bash
# Build entire solution in Debug mode
dotnet build

# Build in Release mode
dotnet build --configuration Release
```

#### Build Specific Projects

```bash
# Build core library only
dotnet build ShatranjCore/ShatranjCore.csproj

# Build command-line application
dotnet build ShatranjCMD/ShatranjCMD.csproj

# Build Windows Forms GUI (Windows only)
dotnet build ShatranjMain/ShatranjMain.csproj

# Build test project
dotnet build tests/ShatranjCore.Tests/ShatranjCore.Tests.csproj
```

#### Clean Build

```bash
# Clean all build artifacts
dotnet clean

# Clean and rebuild
dotnet clean && dotnet build
```

### Option 2: Using Visual Studio

1. Open `Shatranj.sln` in Visual Studio 2022
2. Select **Build → Build Solution** (or press `Ctrl+Shift+B`)
3. Choose configuration: **Debug** or **Release**
4. View build output in the **Output** window

### Option 3: Using Visual Studio Code

1. Open the Shatranj folder in VS Code
2. Press `Ctrl+Shift+B` to run the build task
3. Or use the Terminal:
   ```bash
   dotnet build
   ```

---

## Running the Application

### Command-Line Version (ShatranjCMD)

#### Using .NET CLI

```bash
# Run directly
dotnet run --project ShatranjCMD

# Or navigate to the project directory
cd ShatranjCMD
dotnet run
```

#### Using Executable

```bash
# After building, run the executable
./ShatranjCMD/bin/Debug/net9.0/ShatranjCMD

# Or on Windows
ShatranjCMD\bin\Debug\net9.0\ShatranjCMD.exe
```

### Windows Forms GUI (ShatranjMain) - Windows Only

#### Using .NET CLI

```bash
dotnet run --project ShatranjMain
```

#### Using Visual Studio

1. Set `ShatranjMain` as the startup project (right-click → Set as Startup Project)
2. Press `F5` or click **Start**

---

## Running Tests

### Using .NET CLI

#### Run All Tests

```bash
# Run the custom test runner
dotnet run --project tests/ShatranjCore.Tests

# Alternative: Run from test directory
cd tests/ShatranjCore.Tests
dotnet run
```

#### Run with Verbose Output

```bash
dotnet run --project tests/ShatranjCore.Tests --verbosity detailed
```

### Expected Test Output

```
╔════════════════════════════════════════════════════════════════╗
║              Shatranj Chess - Test Suite                      ║
╚════════════════════════════════════════════════════════════════╝

┌─────────────────────────────────────────────────────────┐
│              Rook Movement Tests                         │
└─────────────────────────────────────────────────────────┘
  Testing rook center position (14 moves)... ✓ PASSED
  Testing rook corner position (14 moves)... ✓ PASSED
  Testing rook can capture enemy piece... ✓ PASSED
  Testing rook cannot move past friendly piece... ✓ PASSED
  Testing rook cannot move past enemy piece... ✓ PASSED
  Testing rook only moves horizontally and vertically... ✓ PASSED

┌─────────────────────────────────────────────────────────┐
│              Knight Movement Tests                       │
└─────────────────────────────────────────────────────────┘
  [... 6 tests ...]

┌─────────────────────────────────────────────────────────┐
│              Bishop Movement Tests                       │
└─────────────────────────────────────────────────────────┘
  [... 6 tests ...]

[... Queen, King, Pawn tests ...]

═══════════════════════════════════════════════════════════════
Test Suite Complete - All Piece Movement Tests Run
═══════════════════════════════════════════════════════════════

All 40 tests passed! ✓
```

### Test Coverage

The test suite includes 40 comprehensive tests:
- ✅ Rook tests (6)
- ✅ Knight tests (6)
- ✅ Bishop tests (6)
- ✅ Queen tests (6)
- ✅ King tests (6)
- ✅ Pawn tests (10 including en passant)

---

## Building for Different Configurations

### Debug Build (Default)

```bash
dotnet build --configuration Debug
```

- Includes debug symbols
- No optimizations
- Easier debugging

### Release Build

```bash
dotnet build --configuration Release
```

- Optimized for performance
- Smaller binary size
- Production-ready

---

## Publishing for Distribution

### Create Self-Contained Executable

#### Windows (x64)

```bash
dotnet publish ShatranjCMD/ShatranjCMD.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  --output ./publish/win-x64
```

#### Linux (x64)

```bash
dotnet publish ShatranjCMD/ShatranjCMD.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  --output ./publish/linux-x64
```

#### macOS (ARM64)

```bash
dotnet publish ShatranjCMD/ShatranjCMD.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  --output ./publish/osx-arm64
```

### Framework-Dependent Deployment

Smaller size, requires .NET 9 runtime installed:

```bash
dotnet publish ShatranjCMD/ShatranjCMD.csproj \
  --configuration Release \
  --output ./publish/portable
```

---

## Troubleshooting

### Common Build Errors

#### "SDK not found" Error

**Problem**: .NET 9 SDK not installed or not in PATH.

**Solution**:
```bash
# Check .NET version
dotnet --version

# Install .NET 9 SDK if not present
# Download from: https://dotnet.microsoft.com/download/dotnet/9.0
```

#### "Project file not found" Error

**Problem**: Running command from wrong directory.

**Solution**:
```bash
# Make sure you're in the project root directory
cd /path/to/Shatranj

# Then run build commands
dotnet build
```

#### "Target framework not supported" Error

**Problem**: Trying to build .NET 9 project without .NET 9 SDK.

**Solution**: Install .NET 9 SDK or higher.

#### Windows Forms Project Won't Build on Linux/Mac

**Expected behavior**: The `ShatranjMain` project targets `net9.0-windows` and requires Windows to build.

**Solution**: Build only command-line projects on non-Windows platforms:
```bash
dotnet build ShatranjCore/ShatranjCore.csproj
dotnet build ShatranjCMD/ShatranjCMD.csproj
```

### Common Test Errors

#### "Assembly not found" Error

**Problem**: Test project can't find ShatranjCore.

**Solution**: Build ShatranjCore first:
```bash
dotnet build ShatranjCore/ShatranjCore.csproj
dotnet run --project tests/ShatranjCore.Tests
```

#### Tests Fail with "Invalid Move" Errors

**Problem**: Game logic issue or breaking change.

**Solution**:
1. Check recent code changes
2. Run specific test file to isolate issue
3. Debug using Visual Studio or VS Code

---

## Development Workflow

### Recommended Build Cycle

```bash
# 1. Clean previous build
dotnet clean

# 2. Restore dependencies
dotnet restore

# 3. Build all projects
dotnet build

# 4. Run tests
dotnet run --project tests/ShatranjCore.Tests

# 5. Run application
dotnet run --project ShatranjCMD
```

### Watch Mode (Auto-Rebuild on File Changes)

```bash
# Automatically rebuild and run on file changes
dotnet watch --project ShatranjCMD run
```

---

## Project Structure

```
Shatranj/
├── Shatranj.sln                  # Solution file
├── ShatranjCore/                 # Core game logic library
│   └── ShatranjCore.csproj      # .NET 9 library
├── ShatranjCMD/                  # Command-line application
│   └── ShatranjCMD.csproj       # .NET 9 console app
├── ShatranjMain/                 # Windows Forms GUI
│   └── ShatranjMain.csproj      # .NET 9 Windows app
└── tests/
    └── ShatranjCore.Tests/       # Unit tests
        └── ShatranjCore.Tests.csproj  # .NET 9 test project
```

---

## Additional Resources

- **.NET 9 Documentation**: https://docs.microsoft.com/dotnet/core/whats-new/dotnet-8
- **Project Documentation**: See `docs/` folder
  - `PROJECT_ROADMAP.md` - Development phases
  - `SOLID_PRINCIPLES.md` - Architecture guide
  - `TERMINAL_COMMANDS.md` - Command reference
- **GitHub Repository**: (Your repository URL)

---

## Getting Help

If you encounter build issues:

1. **Check Prerequisites**: Ensure .NET 9 SDK is installed
2. **Read Error Messages**: They usually indicate the exact problem
3. **Clean and Rebuild**: Often resolves caching issues
4. **Check Documentation**: Review this file and project README
5. **Search Issues**: Check GitHub issues for similar problems
6. **Create Issue**: If problem persists, create a detailed issue report

---

**Last Updated**: 2025-11-05
**Target Framework**: .NET 9.0
**Supported Platforms**: Windows, Linux, macOS

---

**Happy Building! ♟️**
