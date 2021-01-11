# Plan (Development)

## MurderMystery.cs
This file will be the derived class for the EXILED plugin framework.

It will contain main properties for the plugin, as well as info derived from the EXILED plugin class.  
This class with do nothing but merely enable and disable the plugin,  
the other properties NOT derived from EXILED base plugin class will be **main properties intended for use throughout the entire assembly**,  
For Example: a bool used to determine whether the plugin is in debug mode or not.

## Config.cs
This file will be the generic class for the MurderMystery.cs derived from the EXILED IConfig interface.

It will contain basic plugin configs that will be used in **more specific locations** throughout the assembly.

## EventHandlers.cs
This file will be used to handle most server events that occur throughout the assembly.

It will contain subsections of events: **Primary** and **Secondary**, as well as some basic functions for internal use.  
**Primary** handlers will be responsible for making sure the gamemode enables, starts, and disables properly.  
**Secondary** handlers will be responsible for making sure the event works properly.

## Utils/GamemodeStatus.cs
This file will contain properties that are assigned and read within the plugin to make sure the gamemode works properly.

This file is self explanatory if you read the source code.