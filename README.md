# TowerDefense-UnityChallenge
This Repository will be removed or made private after a while.

A concept Tower Defense game built in Unity with modular architecture and gameplay systems.

The Project took me nearly 4 hours and 40 minutes to prepare, utilizing a couple of models and textures, as well as AI tools like ChatGPT to make my code clearer.

## Instructions
- Download the .unitypackage and import it (URP).
- If you want to see logs, add "DEBUG_LOGS" to your scripting define symbols in Player Settings.
- You can easily add more enemies, Towers, or waves by creating related SO.

How does it work?
A WavesPattern is a complete data set for a stage or section of the game that can contain multiple waves.
Each wave can include an unlimited number of sequences with different data.
Each sequence can represent an enemy or a boss.

The WaveManager takes a WavesPattern data asset and runs it completely for the entire stage or section.
You can enable the looping option inside the WavesPattern, so when all waves are finished, it restarts from the beginning and creates infinite waves.

Difficulty Scaling in the WavesPattern allows us to quickly adjust all enemy values, making the game easier or harder as needed. Each Sequence also has this feature, allowing individual difficulty adjustments for specific enemies or bosses.

Both Tower and Enemy are also ScriptableObjects, making them simpler, more flexible, and easier to manage.


🎮 Features
Core Systems
Enemy Wave System - Configurable waves with multiple enemy types and boss waves
Tower Defense - Grid-based tower placement with targeting and projectiles
Enemy AI - Waypoint navigation with health systems and death animations
UI Management - Real-time wave counter, resource management, and game controls

🚀 Quick Start
Open the main scene in Assets/_Content/_Scene/game

Press Play to start the game

Select towers from the UI and place them on the edge area

Defend your base against incoming enemy waves

Hotkeys
WASD - Move Camera

MiddleMouse - Zoom Camera

RightClick - Rotate Camera

Space - Cycle game speed

Right Click - Cancel tower placement

🏗️ Architecture
Built with modular, scalable design:

ScriptableObjects for data-driven configuration

Event-driven communication system

Singleton pattern for game management

Component-based entity system
