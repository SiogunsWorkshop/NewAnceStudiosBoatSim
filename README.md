# NewAnceStudiosBoatSim

A Unity sailboat simulation created as an introductory recruitment task for New Ance Studios. The project models a boat moving under wind and sail power using custom force calculations, without Unity's built-in physics components.

## Gameplay Preview

[![Watch the sailboat simulation](https://img.youtube.com/vi/1yJ6CBQ-zes/hqdefault.jpg)](https://youtu.be/1yJ6CBQ-zes)

[Watch the full gameplay demonstration on YouTube](https://youtu.be/1yJ6CBQ-zes).

## Screenshots

Screenshots show the boat with different sail angles and sail-open percentages.

![Boat simulation with the sail turned -28 degrees and open 60%](<Screenshots/Zrzut ekranu 2026-10-12 203714.png>)

![Boat simulation with the sail turned -45 degrees and open 37%](<Screenshots/Zrzut ekranu 2026-10-12 203727.png>)

![Boat simulation with the sail turned 45 degrees and open 100%](<Screenshots/Zrzut ekranu 2026-10-12 203750.png>)

## Features

- Custom sail aerodynamics using lift and drag to propel the boat.
- Wind and apparent-wind calculations that account for boat velocity.
- Boat movement and turning influenced by sail forces, yaw damping, and forward and lateral water resistance.
- Adjustable sail angle and sail area, with on-screen trim indicators.

## Technical Details

- Unity 6.5.11f1
- C#
- Universal Render Pipeline (URP)
- Unity Input System

## Getting Started

Open the project folder in Unity Hub using Unity Editor version **6000.5.11f1**.
