# LocalInitialData audit (2.4.9 client vs the recreated adventure map)

Checked against: the client scripts in the fork (`PsUICheckpointBanner`, `PsMetagameData`, `ClientTools`, `PsPlanet`, `GameSignalTuneIn/OutState`,
`GenericSignalSequence`, `PsMenuDialogueFlow`, the `PsGameLoopBlock*` classes), `Data\ServerData\InitialData\LocalInitialData.txt`
(identical to the built-in copy and to `BBR\server\LocalInitialData_txt.bytes`) and what the server really serves after its patches
(`UnlockBuilder`, `MapContent`, `MiscApi.LoadPathBlob`). Not run inside the game: the graph and the served path were checked by simulating the
client's parsing (same chain walk as `PsMetagameData.AddUnlocks`) and by running the server's own path code outside the game.

## 1. Why the signal sign had a blank green disc

`menu_cp_image_signal` is only a plain green disc. The game draws the object the checkpoint will unlock on top of it, tinted black
(`PsUICheckpointBanner`, lines 147-171). It finds that object through the `willUnlockProgression` of the checkpoint:

* a node generated from the graph gets it from an OUTGOING edge `Block/Signal -> Unlock` (`PsMetagameData.AddUnlocks`, `m_blockWillUnlock`);
* a node already saved in the player's path carries it as `willUnlockProgression` in its JSON.

The recreated graph has no such edge on any of its 8 television checkpoints (every `Block`/`Signal` node has zero outgoing edges) and the
server's starter path wrote `"willUnlockProgression": ""`. So the disc was always empty and the star counter read `0 / 0` (a `Signal` node has no
star requirement in the client's parser, and no reward cue, hence the blank title).

Fixed (server side, runs on every copy of the map): `UnlockBuilder.AddWillUnlockEdges` adds the missing link, and `FixStoredSignalNodes` fills
`willUnlockProgression` in already saved paths. The target is the next section whose first icon is a real picture (the empty-dome fallback is skipped):

| TV checkpoint (section name as served) | node | shows (black silhouette) |
|---|---|---|
| `unlock_start` (intro, "Big Bang Racing TV") | Signal 107 | Offroader (`item_player_monster_car_icon`) |
| `unlock_treadmill_signal` | Block 110 | Treadmill (`item_boost_ramp_icon`) |
| `unlock_barrel` | Block 89 | Time Attack (`item_mode_timeattack`) |
| `unlock_timeattack_signal` | Signal 112 | Time Attack |
| `unlock_boulder` | Block 81 | Metal Plank (`item_metal_plank_icon`) |
| `unlock_dynamics_signal` | Signal 287 | Metal Plank |
| `unlock_lava` | Block 118 | Dirtbike (`item_player_motorcycle_icon`) |
| `unlock_airbomb` | Signal 121 | Dirtbike |

All these frames exist in `UiAtlas.json`, and the planet sprite sheet uses the same texture as the UI atlas (same `UiAtlasTex.png`).

## 2. First launch (restored)

The client's own first-launch flow is `InitializeMetagameState`: when the next section to unlock is named `unlock_start` it builds the first stretch
of the path itself and starts its first node, the `unlock_start` television, straight away. After the TV (intro_1 ... intro_end_1) the claim of that node
unlocks the next section's items (`PsResearchUnlockState`, the "new items" screen) and, because the current node is then 2, the name prompt
(`PsUICenterNewUser`) comes before the main menu.

The server used to hide this: it renamed the first section to `unlock_begin` and always handed a new player a ready-made path (levels, puzzle,
chest, TV last). Now:

* the first section keeps the name `unlock_start`;
* a player with no main path gets none (`PlayerApi.StoredPaths`), so the game plays the introduction and stores the path it builds;
* "unlock everything" links the extra items to `unlock_start` (flagged when the path is generated) instead of `unlock_car`, so the "new items"
  screen after the intro only lists the section's own items;
* saved paths in the old layout are kept working: their starter chest (the first checkpoint) takes the name `unlock_start` so the game does not
  think the player is new (`FixStoredSignalNodes`);
* `SyntheticMainPath` (TV first) is now only the fallback for an incomplete saved path.

To see it: Admin > Reset Local Path, or a fresh install.

## 3. Dialogues

How the client plays dialogues: only a dialogue attached to a path node, with the trigger the node fires (`LoopActivated`, `LoopStart`,
`MinigameEnter/Win/Lose/Exit`, `LoopClaimed`, `LoopReleased`), is ever played. `Manual` is just "next step of the same dialogue". A dialogue lookup
by name (`GetDialogueByIdentifier`) is used by the television sequences.

Found: 23 dialogue chains were attached to nothing but a `Unlock` block with the `Manual` trigger, i.e. dead in this client (e.g.
`unlock_4_quest_1`, `unlock_rentals_1`, `unlock_bike_1`, `unlock_dynamics_2`, `unlock_platforms_signal_1`...). Only 4 were really wired
(`unlock_1_quest_2`, `unlock_garage_tease_dialogue`, `unlock_2_quest_3`, `unlock_editor_signal_1`); the `test_*` dialogues on the garage are removed
on purpose.

Fixed (`UnlockBuilder.WireSectionDialogues` + `FixStoredSignalNodes`): each such dialogue is now fired as `LoopClaimed` of the checkpoint that CLOSES
the previous section, which is how the game's own wired examples work (the garage tease on the chest before the garage, etc.). 19 chains wired, for
example: `unlock_4_quest_1/2` after the garage block, `unlock_3_quest_1` ("Let's check if TV has anything new") after the block before the time attack
TV, `unlock_3_quest_3` ("RACE for 5 stars") after that TV, `unlock_bike_1` after the bike TV, `unlock_rentals_1/2` after the dynamics garage,
`unlock_dynamics_2`, the platforms / explosives / big dome / mud and ice / gears / elevators / crusher / triple-ups ones.

Left unwired on purpose (dev placeholders that start with "WIP" or "NOTE", they would show to players): `unlock_bouncer_signal_1`,
`unlock_fan_signal_1`, `unlock_portal_signal_1`, `unlock_endless_loop_1`; and the "WIP"/"NOTE" second steps of `unlock_bike_2`,
`unlock_bike_signal_2`, `unlock_powerups_dialogue_3` (the chain is cut before them).
Orphans (nothing in the client calls them): `unlock_steering`, `unlock_engine`, `unlock_tires` (garage upgrade prompts).

Television sequences (`GameSignalTuneInState`): the client picks the sequence from the node's section name and then reads the dialogue by name; a
missing dialogue is a crash in `GenericSignalSequence.StartSequence`. A sequence only uses the first steps: 1 for the normal ones, 2 for the intro,
1 for the intro end. Present and correct: `intro_1` (5 steps), `intro_end_1`, `treadmill_tv_dialogue_1`, `unlock_timeattack_tv_dialogue_1`,
`dynamics_tv_dialogue_1`, and (added by the server) `unlock_barrel_dialogue_1`, `unlock_boulder_dialogue_1`, `unlock_airbomb_dialogue_1`,
`unlock_lava_dialogue_1`.

## 4. Still different from the real 2.4.9 data (not changed, your call)

* The recreated map is an older design: the client has television sequences for 19 section names, the map only has television checkpoints for 8
  sections. Names the client expects but that have no checkpoint (and no dialogue) in the map: `unlock_platforms`, `unlock_mud`, `unlock_gears`,
  `unlock_crumbling_platforms`, `unlock_fan`, `unlock_elevators`, `unlock_ice`, `unlock_crusher`, `unlock_bouncer`, `unlock_portal`, plus
  `unlock_bike` (the map's bike TV is in `unlock_bike_signal`, which the server renames to `unlock_airbomb`; the client's real bike sequence
  `bike_intro_1` / `bike_outro_1` has no dialogue in the map). Today none of them can crash because no node uses those names.
* The server renames some sections so that a television gets a sequence that has a picture: `unlock_upgrade` -> `unlock_treadmill_signal`,
  `unlock_editor` -> `unlock_barrel`, `unlock_timeattack` -> `unlock_boulder`, `unlock_bike_signal` -> `unlock_airbomb`, `unlock_rentals` ->
  `unlock_lava`, first section `unlock_start` -> `unlock_begin`. Side effects: the client's tutorial hooks look for the ORIGINAL names:
  `unlock_rentals` (rent arrow in the vehicle select), `unlock_upgrades` (plural; the map says `unlock_upgrade`) and `unlock_nitros` (booster
  arrow; the map has `unlock_powerups`, no `unlock_nitros`). Those three tutorial effects never show.
* Titles on the checkpoint signs: the client reads them from `blockRewardCue`; the map has none, so garage / construction / television signs
  have no title (chests say "Treasure"). `Signal` nodes cannot carry one at all (the client's parser does not read it for that node type).

## 5. What was checked and is fine

* Every enum value of the served graph parses (`PsGameDifficulty`, `PsSubgenre`, `PsGameMode`, `PsDialogueCharacter`, `PsDialogueCharacterPosition`,
  `PsNodeEventTrigger`, `PsPathBlockType`, `PsUnlockableType`, `PsGameArea`) and every key the client reads without a fallback is present.
* All 29 sections chain correctly (Unlock -> levels -> closing block, Unlock -> next Unlock), every item referenced by a section exists in a category.
* All icons used by silhouettes exist in the atlas.
* The starter path ids and level numbers are consecutive after the move (1..6), the puzzle and the duplicate-TV passes leave it alone.

## 6. First launch: the map has to be right by itself (added after a first-launch test)

On a brand-new player the server hands out no path, so the game builds the first stretch from the map and only the NEXT login goes through the
server's patches (`InjectInsaneBranch`, `InjectVersus`, `RemoveDuplicateTv`...). A first session therefore showed things the second one did not.
Two fixes, both in the map itself (`MiscApi.LoadPathBlob`, last step, so they run on every copy of the map):

* **Duplicate televisions.** The recreated map has an "Antenna" checkpoint with stars (`0 / 6`) immediately followed by a lone "Signal" of its own
  section with no star requirement (`0 / 0`) showing the same item: `unlock_barrel`/`unlock_timeattack_signal`, `unlock_boulder`/
  `unlock_dynamics_signal`, `unlock_lava`/`unlock_airbomb`. `UnlockBuilder.RemoveLoneTvSections` drops the lone one (the empty section stays so the chain
  of sections is unchanged) and appends the dialogue that was hooked on it to the dialogue of the checkpoint before it. Paths saved earlier lose the
  lone node at the next login (`MapContent.RemoveLoneTv`, nodes after the player's current node only, ids and side-path start nodes follow).
* **Side paths.** `MapContent.AddSidePaths` puts a `SidePath` node (the client's own `PsMetagameData.AddUnlocks` chain) in the map: the "Impossible"
  branch (3 levels) after the 3rd bubble of the first section and a Versus race after the 2nd level of `unlock_treadmill`, `unlock_platforms` and
  `unlock_gears`. The game then creates them itself in the first session. The login-time injections stay for paths saved before and skip what is
  already there.

Checked by simulating the client's strip generation (`AddUnlocks` + `GeneratePersistentPathStrip`) on the served map, posting that strip through
`/v2/player/data/change` and logging in again: the main path comes back unchanged, one `4_SidePath` and one `13_SidePath`, nothing duplicated.
