# Proc-Gen's Game Design Document for 7DRL 2024

## Title

Land of the Fallen Magi

## Table of Contents

- [Premise](#premise)
- [Overview](#overview)
	- [Game Start](#game-start)
	- [Core Game Loop](#core-game-loop)
	- [Level Generation](#level-generation)
- [UI](#ui)
	- [Splash Screen](#splash-screen)
	- [Main Menu](#main-menu)
	- [New Game Screen](#new-game-screen)
	- [Main Game Screen](#main-game-screen)
	- [Game Over Screen](#game-over-screen)

## Premise

<p>
	In the world of Elison, magic power doesn't disappear when a wielder dies. Rather, it merges with the land where they are laid to rest. This usually results in benefits to the surrounding area, such as the tomb of a lightning mage acting as a lightning rod for a town during storms or the tomb of a plant mage granting fertile soil.
</p>
<p>
	However, recent events have shown these tombs to also have the potential to cause great problems. The warmth provided by a fire mage's tomb became too intense and the town caught fire. The mines rich in minerals and ore for another town have become too dangerous for workers because of constant earthquakes coming from the tomb of an earth mage.</p>
<p>
	To combat the growing number of catastrophies, a new group has been formed called the Magic Justiciars. Their job is to go into these problematic tombs and deal with whatever is causing the magic power to run rampant.
</p>

## Overview

### Game Start

<p>
	The player will start by selecting a weapon and an element of magic to be used by their magic justiciar.
</p>

#### Starter Weapons

- Sword
- Bow
- Staff

#### Starter Elements

- Fire: Ember Shot
- Ice: Ice Imbuement
- Lightning: Flash Step

### Core Game Loop

<p>
	After creating their character, the player will select one of three missions. Each mission will have the name and element of the fallen magic wielder as well as their most prominent characteristic, such as mischievous or generous. How these characteristics affect gameplay will be described in the level generation section.
</p>
<p>
	Defeating the magic wielder at the end of a dungeon will allow the player to add or replace a skill, up to a maximum of four skills. The player will also gain experience and levels through combat to make their character more resilient and effective.
</p>
<p>
	Upon completion of a mission, the player will be given another three choices for their next mission. Missions will go up in difficulty after each completion, and will continue to do so until the player dies.
</p>

### Level Generation

#### Elements

<p>
	The element of a tomb's magic wielder will determine the overall color scheme. Lightning will have hues of yellow and white. Fire will have a combination of orange and red.
</p>
<p>
	The element will also determine what strengths and weaknesses the enemies in the tomb will have. Basically, using fire spells in a water mage's tomb is going to be a rough time.
</p>

#### Mage Characteristics

<p>
	The characteristic of the magic wielder will determine either the shape or cause special areas to exist in a dungeon. Some examples below:
</p>

- Generous
    - Increased number of enemies, chests, and drops (Make it rain blood and gold)
- Puzzle Master
	- Some sort of puzzle will be required to reach the magic wielder
- Secretive
	- One or more keys will need to be recovered to unlock the room of the magic wielder

#### Algorithms

TBD

## UI

### Splash Screen

TBD

### Main Menu

- Has title at top
- Buttons/options
	- New Game
		- Starts a new game and destroys any existing save file
	- Continue Game
		- Loads the game from a save file
	- Settings
		- Options for things (volume if sound/music is included)
	- Quit
		- Self explanatory

### New Game Screen

### Main Game Screen

![Diagram of Main Game Screen](/images/game-screen.png)

- A: Map/Player Area
	- The character controlled by the player will move around in this area
- B: Important Player Stats
	- Stats for health and skills will appear here 
- C: Message console
	- Messages about occurences such as attacking, opening a door, hitting a button, etc.
	- The 10 most recent messages will display here
- D: Tile Info
	- Info about the tile the player is standing on
	- If in inspect mode, info about the selected tile and entities there if inside player FOV

#### Inventory

<p>
	The inventory window will pop up over the main game screen. Top left will be character stats, top right will be equipped items, bottom left will be inventory items (max of 10?), and bottom right will be a description of the item selected. 
</p>

#### Skill Selection

<p>
	A series of 3 cards will be displayed over top of the main game screen. The name will be at the top of the card, a description of it just below, and then stats and targeting style will be at the bottom.
</p>

#### Level Up

<p>
	The player will have a chance to increase one of their base stats every time they level up. They will also receive an increase to their maximum health and be fully healed.
</p>

#### Mission Selection

<p>
	Similar to skill selection, 3 cards will be displayed. The title will be the name of the fallen mage and below that will be some information about enemies to face and skill usage to expect in battle.
</p>

#### Pause Menu

<p>
	Small menu that shows up over the screen with options to change settings or go back to the main menu.
</p>

### Game Over Screen