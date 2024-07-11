# MCC Arkiver

## Cross-platform Theater, Map, Mode and Carnage keeper for Halo: MCC

<img src="https://raw.githubusercontent.com/AQDuck97/assets/master/images/mccarkiver-demo.png">

## Your files are safe from The Great Journey
343 put a (per-game) file limit of 12 for theater, 36 for maps/modes and some random limit on carnage-reports (stats)

Ark does a check of the appdata location where these files are located and backs them up to the archive location of your choice and keeps them safe.

## Features (as of v1.0) :
* Cross-platform with Linux and Windows (Windows is untested but should work)
* Safely backs up files as you play
* Backwards compatible with old version of The Ark and Greater Ark
* Easily "export" .mov files back to the game and keep track which are there
* Gets the true name of map and modes as well as round-start time from the .mov files

## Files Ark backs up:
* .mov (theater files)
* .xml (carnage files)
* .bin (gamemode files)
* .mvar (map files)

## TODO:
* Add support for "exporting" map and mode files
* Match-details (complete stats for every player)
* Stat-tracking
* Better Steam Deck support (potential gamepad support)

## Usage:
### General:
Ark tries to put all files related to a match in the same folder, however this may not always work as there can be slight differences on times, usually it's just the carnage report that is a minute or so off. 
If this happens you can simply move it to the correct directory.

### Linux
⚠️ Make sure that the prefix location is correct!️️ ⚠️

Even if the default location has an MCC prefix it may not be the currently-used one if you have the game installed on a different drive.

* Prefix location is necessary as the files are located in the `AppData/LocalLow directory`. Please only include the path up to `compatdata` (e.g `/path/to/compatdata`)

* `fuse2` dependency required for AppImage, make sure you have that installed if it fails to launch.

**Steam Deck:**

⚠️ Default filepicker in GameMode is bugged and you will not be able to create new folder or select one! Configure directories in Plasma (Desktop Mode) before using in GameMode ⚠️
️
* Steam will default to gamepad, switch to a KBM layout in the configurator for use in GameMode.
* Launch MCC Arkiver _before_ launching the game to avoid some Steam weirdness.

### Windows (untested): 
* _Should_ "just work", simply add an archive location and you should be good to go