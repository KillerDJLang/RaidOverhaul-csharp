# Raid Overhaul

To start, I want to give a huge thanks to Kobrakon ([Immersive Raids](https://forge.sp-tarkov.com/mod/642/immersive-raids)) for the base of features that I've worked off of and CJ for being patient and helping me get this all up and running (teaching me tons along the way). Absolute legends and none of this would've been possible without them!!

Hope you enjoy

If you enjoy what I do and want to buy me a coffee to support my totally not an addiction, you can check out my ko-fi here → [DJ's ko-fi](https://ko-fi.com/djlang)

---

## Table of Contents

- [Features](#features)
- [Events](#events)
- [Installation](#installation)
- [To Uninstall](#to-uninstall)
- [Recommended Mods](#recommended-mods)
- [Config](#config)
- [Issues](#issues)
- [Credits](#credits)

---

## Features

- Almost unlimited raid timer
- Events that will happen periodically throughout your raids
- A body cleanup service that is configurable in the F12 menu alongside a backpack drop chance config
- A deafness effect that can cause concussion and ear ringing depending on the ammo/weapon you're firing without ear protection on
- Slower hydration and energy decay
- Added in quests to get modified versions of the secure containers with more storage space
- A new trader, the "Requisition Office", that sells an assortment of useful items/equipment including some new weapons and equipment, and 3 new currencies "Special Request Forms", "Requisition Slips" and "Requisition Coins" to buy them with (they can also be found in raid)
- More QoL options that are listed in the [Config](#config) section
- A custom boss "Legion" with a dynamic spawn chance that can be found all across Tarkov alongside his legionnaire followers

Most of these can be tweaked in the F12 menu or the server config in `user/mods/RaidOverhaul/RO-Config.exe`

---

## Events

Currently implemented events are:

- **Airdrop** — An airdrop will be brought in
- **Heart Attack** *(LOL)* — Breaks your arm and deals a set amount of damage to your thorax alongside some short debuffs. If disabled, will play a Joke event which will give a notification and then choose another random event
- **Healing** — You will be healed to full health
- **Armor Repair** — All equipped armor will be fully repaired
- **Blackout** — All lights and power switches will be turned off for a set amount of time
- **Power Switch** — A random power switch on the map will be activated
- **Door Unlock** — A random door on the map will be unlocked and opened
- **Skill** — A chance to either gain or lose a single level in a random skill
- **Metabolism** — A chance to either disable energy and hydration loss completely, decrease energy and hydration drain by 20%, or increase energy and hydration drain by 20%
- **Trader** — Shifts your standing with a random trader up by 0.1 or down by 0.05
- **Malfunction** — Makes malfunctions significantly more likely to happen for a minute
- **Weight** — Doubles the weight of all items in your inventory for a few minutes
- **Berserk** — Gives you buffs to weapon stats (ergonomics and recoil control) for a short period of time. This event will be expanded upon in a future update
- **Shopping Spree** — Raises your rep with every trader by 1 for a set period of time, then reverts back to normal
- **Exfil Lockdown** — Locks down all exfils for 10 minutes, after which they become available again
- **Artillery** — Calls in mortar strikes on your position 30 seconds after the event starts
- **Exfil Menu** *(hotkey configurable in the BepInEx menu)* — Allows you to spend any of the new Requisition currencies to either call the train for exfil on supported maps, call for an exfil with a delay (cheaper option), exfil immediately (expensive option), or exfil a set amount of gear/loot that you choose in the UI, which you will receive in a message from the Req Shop post-raid

---

## Installation

1. Open the 7zip file with 7zip
2. Drag both the `BepInEx` and `user` folders into your base SPT directory

![Installation GIF](https://i.imgur.com/vKRw58b.gif)

> I know this is SAIN but the same will apply here (thanks Drakia for the GIF!)

---

## To Uninstall

To remove Raid Overhaul and prevent any errors, you need to remove the mod files from the `user/mods` folder, `BepInEx/plugins`, and `BepInEx/patchers`.

| Component | Location | What to Remove |
|-----------|----------|----------------|
| **Prepatch** | `BepInEx/patchers` | `RO-Prepatch.dll` |
| **Plugin** | `BepInEx/plugins` | Entire `RaidOverhaul` folder |
| **Server** | `user/mods` | Entire `RaidOverhaul` folder |

BOOM — you've uninstalled the mod and not borked everything.

---

## Recommended Mods

- [ABPS - Acid's Bot Placement System](https://forge.sp-tarkov.com/mod/2097/abps-acids-bot-placement-system)
- [Questing Bots](https://forge.sp-tarkov.com/mod/1109/questing-bots)
- [Looting Bots](https://forge.sp-tarkov.com/mod/812/looting-bots)
- [SAIN - Solarint's AI Modifications - Full AI Combat System Replacement](https://forge.sp-tarkov.com/mod/791/sain-solarints-ai-modifications-full-ai-combat-system-replacement)
- [Quest Tracker](https://forge.sp-tarkov.com/mod/1140/quest-tracker)
- [Expanded Task Text](https://forge.sp-tarkov.com/mod/2389/expanded-task-text)
- [Skills Extended](https://forge.sp-tarkov.com/mod/2383/skills-extended)

I was going to put an explanation for each of these but they're all amazing mods that I think combine well with this, so check them out if you haven't already!

---

## Config

The config files are all located in the server side of the mod at `user/mods/RaidOverhaul/config/*.json` and can be edited either manually or with the `RO-Config.exe` in the server mod's base directory.

> The `RO-Config.exe` must stay located in the root server mod directory so it can find the config files, but you can make a shortcut wherever you want.

### Main Mod Config

#### Raid > Boss & Spawns

```json5
"EnableCustomBoss"            // Enable the custom boss "Legion" to spawn in your raids. Beating and turning in his headgear is required to unlock the Requisitions Office trader. If disabled, the trader quest will be changed to be completeable without Legion.
"EnableRequisitionOffice"     // Enables the new trader. The Req Office uses new currencies to trade for almost any item in the game. The trader assort is randomly generated each server start and item prices are all either the flea or handbook price of the item to keep in line with base game balance.
"UseLegionGlobalSpawnChance"  // This lets you set a custom percent chance that Legion will spawn in your raids as opposed to using a progressive spawn chance. If disabled, the spawn chance will slowly tick up until you eventually beat the boss and it resets to its base spawn percentage.
"GlobalSpawnChance"           // The chance that Legion will spawn in raid if "Use Legion Global Spawn Chance" is enabled.
"EnableCustomItems"           // Enables the custom items created for the mod in your game. These are available as a mix of in-raid loot and trader purchases. Legion's gear and the custom currency/containers are not affected by this option.
```

#### Raid > Quality of Life

```json5
"BackupProfile"        // Let this mod save a backup of your profile into a folder in the mod folder in the case of a game breaking issue or if you just want to revert to a previous point in your playthrough.
"SaveQuestItems"       // Allow your character to save quest items on death. This counts for all items that go into the quest items box in your inventory.
"NoRunThrough"         // Disables the run through status in your raids.
"LootableMelee"        // Allows you to loot melee weapons off of dead players/scavs etc.
"LootableArmbands"     // Allows you to loot armbands off of dead players/scavs etc.
"EnableExtendedRaids"  // Lets you set a custom max time limit for your raids.
"TimeLimit"            // The max length of your raids in minutes if "Enable Extended Raids" is enabled.
"HolsterAnything"      // Allows you to holster any weapon in your sidearm slot.
"LowerExamineTime"     // (No tooltip provided)
"SpecialSlotChanges"   // Lets you put anything into your special slots. A bit game breaking but great if you want to be the biggest loot goblin around.
"ChangeBackpackSizes"  // Changes the internal sizes of most of the backpacks in the game. Makes them all slightly (massively for boss backpacks) bigger. Useful if you intend on staying and looting in raid a lot longer with Extended Raids enabled.
"ModifyEnemyBotHealth" // Changes all enemy bot health to be the same as the player. Makes life less stressful when you run into a boss or raiders an hour+ into a raid.
```

#### Raid > Energy and Hydration Settings

```json5
"ReduceFoodAndHydroDegradeEnabled" // Enables the multipliers for Hydration and Energy decay.
"EnergyDecay"                      // Energy decay will be multiplied by this amount. e.g. 1.5 = 50% more energy loss, 0.5 = 50% less.
"HydroDecay"                       // Hydration decay will be multiplied by this amount. e.g. 1.5 = 50% more hydration loss, 0.5 = 50% less.
```

#### Raid > Weather and Season Settings

```json5
"WeatherChangesEnabled" // Enables the weather and season options below.
"AllSeasons"            // Randomizes the weather/season each time you start the game.
"NoWinter"              // Randomizes the weather/season each time you start the game, excluding Winter.
"SeasonalProgression"   // Progresses through each season in sequence. The mod keeps track of how many raids you've completed and will progress to the next season every couple of raids.
"WinterWonderland"      // Sets the in-game season to Winter.
```

#### Map/Loot > Loot Changes

```json5
"LootChangesEnabled"        // Enable the loot multipliers for each type of loot on all maps.
"StaticLootMultiplier"      // The multiplier for loot found in containers.
"LooseLootMultiplier"       // The multiplier for loot found on the ground/out in the world on the map.
"MarkedRoomLootMultiplier"  // The multiplier for loot found in marked rooms.
```

#### Map/Loot > Airdrop Changes

```json5
"ChangeAirdropValuesEnabled" // Enable the changes to airdrop chances.
"Customs"                    // The percent chance to get an airdrop on Customs.
"Woods"                      // The percent chance to get an airdrop on Woods.
"Lighthouse"                 // The percent chance to get an airdrop on Lighthouse.
"Interchange"                // The percent chance to get an airdrop on Interchange.
"Shoreline"                  // The percent chance to get an airdrop on Shoreline.
"Reserve"                    // The percent chance to get an airdrop on Reserve.
"Streets"                    // The percent chance to get an airdrop on Streets.
"GroundZero"                 // The percent chance to get an airdrop on Ground Zero.
```

#### Traders

```json5
"DisableFleaBlacklist"           // Disable the flea blacklist, which allows all items to be bought and sold on the flea market.
"Ll1Items"                       // Set all of the Requisition Shop's items to be available at Loyalty Level 1 as opposed to being spread across all tiers.
"RemoveFirRequirementsForQuests" // Remove the found-in-raid requirements to turn items in for all quests (not just for the Req Shop).
```

#### Traders > Insurance Changes

```json5
"InsuranceChangesEnabled" // Enable the trader insurance time tweaks.
"PraporMinReturn"         // The minimum time for Prapor to return your insurance items.
"PraporMaxReturn"         // The maximum time for Prapor to return your insurance items.
"TherapistMinReturn"      // The minimum time for Therapist to return your insurance items.
"TherapistMaxReturn"      // The maximum time for Therapist to return your insurance items.
```

#### Stack Tuning > Basic Stack Changes

```json5
"BasicStackTuningEnabled" // Enable the basic changes to ammo stack size. Cannot be used with Advanced Stack Changes — only have one enabled.
"StackMultiplier"         // The multiplier for ammo max stack size. The vanilla stack size for all ammo will be multiplied by this number.
```

#### Stack Tuning > Advanced Stack Changes

```json5
"AdvancedStackTuningEnabled" // Enable the advanced changes to ammo stack size. Cannot be used with Basic Stack Changes — only have one enabled.
"ShotgunStack"               // The total max stack size for shotgun ammo. Not a multiplier.
"FlaresAndUbgl"              // The total max stack size for flare and UBGL ammo. Not a multiplier.
"SniperStack"                // The total max stack size for sniper ammo. Not a multiplier.
"SmgStack"                   // The total max stack size for SMG ammo. Not a multiplier.
"RifleStack"                 // The total max stack size for rifle ammo. Not a multiplier.
```

#### Stack Tuning > Money Stack Changes

```json5
"MoneyStackMultiplierEnabled" // Enable the changes to money stack size.
"MoneyMultiplier"             // The multiplier for money max stack size. The vanilla stack size will be multiplied by this number.
```

#### Player > Weight Changes

```json5
"WeightChangesEnabled" // Enable the weight multiplier.
"WeightMultiplier"     // The multiplier for your total PMC weight.
```

#### Player > Pocket Changes

```json5
"PocketChangesEnabled"  // Enable the changes to your PMC pocket size.
"Pocket1Vertical"       // The total vertical size for your first pocket slot.
"Pocket1Horizontal"     // The total horizontal size for your first pocket slot.
"Pocket2Vertical"       // The total vertical size for your second pocket slot.
"Pocket2Horizontal"     // The total horizontal size for your second pocket slot.
"Pocket3Vertical"       // The total vertical size for your third pocket slot.
"Pocket3Horizontal"     // The total horizontal size for your third pocket slot.
"Pocket4Vertical"       // The total vertical size for your fourth pocket slot.
"Pocket4Horizontal"     // The total horizontal size for your fourth pocket slot.
```

---

### Event Weighting Config

> The numbers here are their relative chances to be pulled in relation to the rest of the events.

#### Random Raid Events > In-Raid Event Weightings

```json5
"DamageEvent"              // Weight for the Heart Attack random event.
"AirdropEvent"             // Weight for the Airdrop random event.
"BlackoutEvent"            // Weight for the Blackout random event.
"JokeEvent"                // Weight for the Joke random event.
"HealEvent"                // Weight for the Heal random event.
"ArmorEvent"               // Weight for the Armor Repair random event.
"SkillEvent"               // Weight for the Skill Point random event.
"MetabolismEvent"          // Weight for the Metabolism random event.
"MalfunctionEvent"         // Weight for the Malfunction random event.
"TraderEvent"              // Weight for the Trader Reputation random event.
"BerserkEvent"             // Weight for the Berserk random event.
"WeightEvent"              // Weight for the Player Weight random event.
"MaxLLEvent"               // Weight for the Max Loyalty Level random event.
"LockdownEvent"            // Weight for the Lockdown random event.
"ArtilleryEvent"           // Weight for the Artillery random event.
"RandomEventRangeMinimum"  // The minimum time into a raid before a random event can trigger.
"RandomEventRangeMaximum"  // The maximum time into a raid before a random event can trigger.
```

#### Random Raid Events > Unlock Event Weightings

```json5
"SwitchToggle"          // Weight for the Power Switch Toggle unlock event.
"DoorUnlock"            // Weight for the Door Unlock event.
"KeycardUnlock"         // Weight for the Keycard Unlock event.
"DoorEventRangeMinimum" // The minimum time into a raid before a door unlock event can trigger.
"DoorEventRangeMaximum" // The maximum time into a raid before a door unlock event can trigger.
```

---

## Issues

As a result of me going on my little hiatus I have disabled comments on my mod pages.

I always appreciate the support and love but I needed to take a wee break from the flood of notifications.

All issues can be reported on my GitHub issues section for RO found here:

- [Raid Overhaul GitHub 4.x.x](https://github.com/KillerDJLang/RaidOverhaul-csharp/issues)
- [Raid Overhaul GitHub ~3.11](https://github.com/KillerDJLang/Raid-Overhaul/issues)

---

## Credits

- [Kobrakon](https://hub.sp-tarkov.com/user/7008-kobrakon/?highlight=kobrakon) for the original [Immersive Raids](https://hub.sp-tarkov.com/files/file/876-immersive-raids/) mod this is based off of
- [CJ](https://hub.sp-tarkov.com/user/37201-dirtbikercj/?highlight=dirtbik) for helping me out a ton early on and answering all sorts of dumb questions
- [Ducky](https://hub.sp-tarkov.com/user/32691-jehree/?highlight=jehree) for the Fika wrapper code idea which helped me a ton in getting everything synced up for Fika
- [necrodream](https://sketchfab.com/necrodream) for the [model](https://sketchfab.com/3d-models/black-envelope-286bc5d673eb465a98d00a451f7260ed) used for the special request forms
- [Qwestgamp](https://sketchfab.com/Qwestgamp.) for the [model](https://sketchfab.com/3d-models/military-container-free-0baa1161466f4b50a775c12625565104) used for the new storage crate
- [RadioactiveAG](https://sketchfab.com/RadioactiveAG) for the [model](https://sketchfab.com/3d-models/gold-dragon-coin-001-rag-6467afed3bce4317b326159e8eecbb87) used for the requisition coins
- [divadan](https://sketchfab.com/divadan) for the [model](https://sketchfab.com/3d-models/corrupted-metal-armor-0bfaa972710241b69de124e77cd46a5c) used for the new legion facemask and armor
- [Rovex](https://sketchfab.com/Rovex833) for the [model](https://sketchfab.com/3d-models/lone-dragon-plate-carrier-5241973849324c2db77a77a51efe07d1) used for the lone dragon plate carrier
- [Quinn](https://sketchfab.com/qkuslic1) for the [model](https://sketchfab.com/3d-models/oakley-kitchen-sink-backpack-model-c2440ae21b14485095ba3da4d947f1ee) used for the new Oakley backpack
- [Karnaval](https://sketchfab.com/amadions) for the [model](https://sketchfab.com/3d-models/free-armor-plate-carrier-gameready-metahuman-39d3b162b14444d487eb39186268e7a7) used for the new rhino plate carrier
- [FoFlaFe](https://sketchfab.com/FoFLaFe) for the [model](https://sketchfab.com/3d-models/army-wallet-free-3d-model-54d38c6c8d824edf908c7c9adbb61143) used for the army wallet
- [Sky Sentry](https://sketchfab.com/skysentry) for the [model](https://sketchfab.com/3d-models/ammo-pouch-military-f52f785634ca42009f4e5172f6faaf12) used for the ammo pouch
- [carlosvasc670](https://sketchfab.com/carlosvasc670) for the [model](https://sketchfab.com/3d-models/sci-fi-medkit-139314307c144fb7989d713c60f7bd10) used for the large medical carrying case
- [Maxime66410](https://sketchfab.com/Maxime66410) for the [model](https://sketchfab.com/3d-models/medkit-scp-f4556c26a80d481ea575aebd6a53de80) used for the medium medical carrying case
- [dominikborovicka](https://sketchfab.com/dominikborovicka) for the [model](https://sketchfab.com/3d-models/dirty-red-medkit-0c37f7910746402ca5bb62899843bcd2) used for the small medical carrying case
- [plaggy](https://www.cgtrader.com/designers/plaggy) for the [model](https://www.cgtrader.com/free-3d-models/household/household-tools/keys-iron) used for the small keytool

Much love all around 💙
