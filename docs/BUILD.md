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

- ✅ **Windows** - Fully supported
- ✅ **Linux** - Fully supported
- ✅ **macOS** - Fully supported

Note: All projects are cross-platform compatible. GUI versions are planned for Phase 5.

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
# Build abstractions library (no dependencies)
dotnet build ShatranjCore.Abstractions/ShatranjCore.Abstractions.csproj

# Build core library
dotnet build ShatranjCore/ShatranjCore.csproj

# Build AI library
dotnet build ShatranjAI/ShatranjAI.csproj

# Build command-line application
dotnet build ShatranjCMD/ShatranjCMD.csproj

# Build core test project
dotnet build tests/ShatranjCore.Tests/ShatranjCore.Tests.csproj

# Build AI test project
dotnet build ShatranjAI.Tests/ShatranjAI.Tests.csproj

# Build integration test project
dotnet build tests/ShatranjIntegration.Tests/ShatranjIntegration.Tests.csproj
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

---

## Running Tests

The project has three test suites organized in a test pyramid:

### Using .NET CLI

#### Run All Test Suites

```bash
# Run core tests (40+ tests)
dotnet run --project tests/ShatranjCore.Tests

# Run AI tests (6 tests)
dotnet run --project ShatranjAI.Tests

# Run integration tests (6 tests)
dotnet run --project tests/ShatranjIntegration.Tests
```

#### Run from Test Directory

```bash
# Core tests
cd tests/ShatranjCore.Tests
dotnet run

# AI tests
cd ShatranjAI.Tests
dotnet run

# Integration tests
cd tests/ShatranjIntegration.Tests
dotnet run
```

#### Run with Verbose Output

```bash
dotnet run --project tests/ShatranjCore.Tests --verbosity detailed
```

For detailed test documentation, see [TESTING.md](docs/TESTING.md)

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

The project has 50+ comprehensive tests across three test suites:

**ShatranjCore.Tests (40+ tests)**
- ✅ Rook tests (6)
- ✅ Knight tests (6)
- ✅ Bishop tests (6)
- ✅ Queen tests (6)
- ✅ King tests (6)
- ✅ Pawn tests (10 including en passant)
- ✅ Castling tests (6)
- ✅ Check detection tests (4)

**ShatranjAI.Tests (6 tests)**
- ✅ BasicAI tests (3)
- ✅ MoveEvaluator tests (3)

**ShatranjIntegration.Tests (6 tests)**
- ✅ AI Integration tests (3)
- ✅ Game Flow tests (3)

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
├── Shatranj.sln                       # Solution file
├── ShatranjCore.Abstractions/         # Core abstractions (NO DEPENDENCIES)
│   └── ShatranjCore.Abstractions.csproj  # .NET 9 library
├── ShatranjCore/                      # Core game logic library
│   └── ShatranjCore.csproj           # .NET 9 library
├── ShatranjAI/                        # AI implementation
│   └── ShatranjAI.csproj             # .NET 9 library
├── ShatranjCMD/                       # Command-line application
│   └── ShatranjCMD.csproj            # .NET 9 console app
└── tests/
    ├── ShatranjCore.Tests/            # Core unit tests (40+ tests)
    │   └── ShatranjCore.Tests.csproj # .NET 9 test project
    ├── ShatranjAI.Tests/              # AI unit tests (6 tests)
    │   └── ShatranjAI.Tests.csproj   # .NET 9 test project
    └── ShatranjIntegration.Tests/     # Integration tests (6 tests)
        └── ShatranjIntegration.Tests.csproj  # .NET 9 test project
```

### Dependency Graph

```
ShatranjCore.Abstractions (no dependencies)
    ↑               ↑
    │               │
    │               └─────── ShatranjAI
    │                            ↑
    │                            │
    └─── ShatranjCore ───────────┘
            ↑
            │
        ShatranjCMD
```

---

## Additional Resources

- **.NET 9 Documentation**: https://docs.microsoft.com/dotnet/core/whats-new/dotnet-9
- **Project Documentation**: See `docs/` folder
  - `ARCHITECTURE.md` - Technical architecture details
  - `TESTING.md` - Test architecture and running tests
  - `SOLID_PRINCIPLES.md` - Architecture guide and SOLID analysis
- **Main Documentation**:
  - `README.md` - Quick start guide
  - `ROADMAP.md` - Complete development roadmap
- **GitHub Repository**: https://github.com/YourUsername/Shatranj

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
