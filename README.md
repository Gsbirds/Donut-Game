# Dependencies for Donut Game

This document outlines all dependencies required to build and run the game.

## Framework Dependencies

- **.NET 8.0** - The core framework used for the application
- **MonoGame.Framework.DesktopGL** (Version 3.8.2.1105) - Cross-platform game development framework
- **MonoGame.Content.Builder.Task** (Version 3.8.2.1105) - Builds content for MonoGame projects

## Development Tools

- **dotnet CLI** - Command line tools for .NET development
- **MonoGame Content Pipeline** - Tool for processing game assets

## How to Install/Update Dependencies

### Installing .NET SDK

```bash
# For Ubuntu/Debian
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# For other distributions, visit: https://dotnet.microsoft.com/download
```

### Restoring Project Dependencies

After cloning the repository, restore the dependencies:

```bash
cd /path/to/monogame-game/monogame
dotnet restore
```

### Updating Dependencies

To update to the latest compatible version of dependencies:

```bash
dotnet add package MonoGame.Framework.DesktopGL --version 3.8.2.1105
dotnet add package MonoGame.Content.Builder.Task --version 3.8.2.1105
```

## Building and Running

```bash
# Build the project
dotnet build

# Run the game
dotnet run
```

## Content Pipeline

The game uses the MonoGame Content Pipeline to process assets like textures, sounds, and fonts. The content pipeline is configured in the `.mgcb` file, which can be edited with the MonoGame Content Builder tool.

```bash
# Install the MGCB editor (if not already installed)
dotnet tool install --global dotnet-mgcb-editor
mgcb-editor
```

## System Requirements

- Operating System: Windows, macOS, or Linux
- Graphics: OpenGL 2.0+ compatible
- Memory: 2GB RAM recommended
- Storage: 100MB available space
