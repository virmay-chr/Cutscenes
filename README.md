# CUTSCENES
**Cutscenes** is a BepInEx mod for [Project Arrhythmia](https://store.steampowered.com/app/440310/Project_Arrhythmia/), that allows you to create cutscene sections in the Editor and skip them in the Arcade (and in the Editor too).
The levels made with this mod are compatible with vanilla version.

## Installation
1. Open the **Project Arrhythmia folder**.
2. Download the corresponding version of [BepInEx](https://github.com/BepInEx/BepInEx/releases) and copy the contents of the archive into the **Project Arrhythmia folder**
3. Launch Project Arrhythmia once and then close it.
4. In the Project Arrhythmia folder, go to `BepInEx/plugins`.
5. Download the corresponding version of [Cutscenes](https://github.com/Reimnop/Catalyst/releases) and move it into the `plugins` folder.
6. Enjoy!

## Usage
To create a cutscene section, all you have to do is create one checkpoint, go to checkpoint editor and toggle the `Cutscene` button (name field of this checkpoint will be locked).

<img width="261" height="179" alt="Screenshot_1" src="https://github.com/user-attachments/assets/d190db50-4d81-4dfc-b6ba-57a0773f87a0" />

> [!CAUTION]
> At this moment, rewinding is not compatible with DOPitch trigger, so it is not recommended to use this trigger in cutscene sections!

After reaching tagged checkpoint, you can press the key, that will rewind the level to the next checkpoint.

> [!WARNING]
> When rewinding, the player can still receive hits!

![Project Arrhythmia 2026-01-11 17-53-43-088](https://github.com/user-attachments/assets/ce942709-9d1b-4fd8-833b-dcc7756752a5)

## Configurations
Mod configurations are stored in `BepInEx/configs/Virmay.Cutscenes.cfg`
- Key - the cutsene skip key.
- Glitch - The intensity of glitch effect on rewinding [0.00 - 1.00].
- AfterRestart - Rewind the cutscenes automatically after first restart.
