# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Grimspire** is a Unity 2D management game where players manage an adventurer city-fortress on the frontiers of a fantasy kingdom. The dual gameplay involves developing a prosperous city AND managing a guild of adventurers who explore dangerous surrounding lands. Built with Unity 2022.3.55f1 LTS and URP, the project follows clean code principles with one class per file.

## Unity Project Structure

### Key Directories
- `Assets/Scripts/` - Main source code organized into functional folders:
  - `Core/` - Core game systems (GameManager, etc.)
  - `Data/` - Data structures and ScriptableObjects
  - `Enums/` - Enumeration definitions
  - `Managers/` - Management systems
  - `Systems/` - Game systems and mechanics
  - `UI/` - User interface components
- `Assets/Scenes/` - Unity scene files
- `Assets/Settings/` - Project configuration and render pipeline settings
- `ProjectSettings/` - Unity project configuration files
- `Packages/` - Package dependencies (manifest.json)

### Current State
- Minimal codebase with basic GameManager template
- URP configured for 2D development
- Standard Unity 2D feature set enabled
- No custom build scripts or test framework configured yet

## Development Commands

### Building the Project
Unity projects are built through the Unity Editor or command line:
```bash
# Build via Unity Editor: File → Build Settings → Build
# Command line build (if needed later):
# /Applications/Unity/Unity.app/Contents/MacOS/Unity -batchmode -quit -projectPath . -buildTarget StandaloneOSX -buildPath ./Builds/
```

### Running the Game
- Open project in Unity Editor
- Press Play button in Editor, or
- Open the main scene and play from there

## Game Design Overview

### Core Gameplay Loop
**Dual-phase gameplay:**
- **Day Phase (Management)**: City construction, adventurer recruitment, trade, diplomacy
- **Night Phase (Adventure)**: Send adventurer parties on missions, receive reports and loot

### Key Game Systems
- **City Management**: Residential, commercial, industrial, and administrative districts
- **Resource Management**: Gold, population, materials, magic crystals, reputation
- **Adventurer System**: Recruit, equip, level heroes across classes (warriors, mages, rogues, clerics)
- **Mission System**: Procedural dungeons, exploration, dynamic events
- **Economy**: Circular system where adventurers bring resources → artisans craft equipment → better missions

### Development Phases (Roadmap)
1. **Phase 1**: Core systems (data architecture, resources, buildings)
2. **Phase 2**: Adventurer system (recruitment, equipment, progression)
3. **Phase 3**: Mission system (procedural generation, combat resolution)
4. **Phase 4**: Gameplay cycles (day/night, events)
5. **Phase 5**: Advanced systems (diplomacy, tech tree)
6. **Phase 6**: Narrative content
7. **Phase 7**: Polish and optimization

## Code Architecture

### Planned Core Systems
- **GameManager**: Main game controller (currently template)
- **ResourceManager**: Gold, population, materials, magic management
- **BuildingSystem**: Construction and building effects
- **AdventurerManager**: Hero recruitment, parties, progression
- **MissionSystem**: Procedural quest generation and resolution
- **TimeManager**: Day/night cycle management
- **EventSystem**: Dynamic events and consequences

### Unity-Specific Patterns
- MonoBehaviour-based components
- ScriptableObject data containers for game configuration
- Unity's component-entity system
- Event-driven architecture
- Clean code with one class per file

## Key Technologies
- **Unity 2022.3.55f1 LTS** - Game engine
- **Universal Render Pipeline (URP)** - Rendering pipeline optimized for 2D
- **C#** - Primary scripting language
- **Unity 2D Feature Set** - 2D-specific tools and components

## Development Guidelines
- **Code Style**: Clean code principles, one class per file
- **Architecture**: Menu-based UI initially, data-driven design patterns
- **Language**: C# with Unity best practices
- **Game Design**: Dark but hopeful atmosphere, risk/reward mechanics
- **Progression**: Modular development following the 7-phase roadmap

## Development Notes
- Project uses Unity's standard .gitignore for proper version control
- No custom package dependencies beyond Unity's standard 2D packages
- Ready for expansion with management game mechanics
- MIT licensed project by Frédéric Fréville
- French documentation available in `Docs/` folder for detailed game design