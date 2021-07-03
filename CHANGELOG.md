# 1.19 (July 2st, 2021):
* [Change] `KSP 1.11` compatibility. __WARNING__: the mod won't work with version lower than `KSP 1.11`!
* [Change] Use a specialized `KSPDev Utils` version ot workaround `KPS 1.12` bug when duplicated mods are detected.
* [Change] Deprecate `ModuleStockLightColoredLens` module.
* [Fix #39] Color picker is not available.
* [Fix #40] Deprecate the lens brightness control. The lens is now controlled by the stock game.
* [Fix #41] Light identifier gets lost when state changes ON/OFF.
* [Fix #44] Deal with duplicated "light" entries in the by module filter.

# 1.18 (February 2nd, 2021):
* [Fix #36] ReStock lights get broken.
* [Change] Disable "lens brightness" option in the stock parts to not break their design.

# 1.17 (December 26th, 2020):
* [Fix #33] Duplicated "Light On/Off" events in PAW.
* [Change] Deprecate the legacy RGB controls in favor of the new stock color changer.
* [Change] Deprecate the stock lights lens customization since it's now supported by the game.
* [Enhancement] Make the light parts compatible with the stock inventory system.
* [Enhancement] Allow "Blink On/Off" toggle on the uncommand vessels.
* [Enhancement] Add light customization controls in FLIGHT to the new stock light parts.
* [Enhancement] Group the new stock lights into the "Lights" category.

# 1.16 (May 12th, 2020):
* [Fix #4] Add range and spot angle adjutsable controls.
* [Fix #27] Allow light color and status be controllable in EVA and in flight.
* [Fix #28] Stock lights don't show up in the CCK category.
* [Change] Stop complaining about KSP minor version change.
* [Enhancement] Add PAW action to reset the changed settings to the state at the scene load.
* [Enhancement] Allow ReStock+ light parts orientation for EVA adjustment.

# 1.15 (December 1st, 2019):
* [Fix #25] Stock lights don't emit light, conflict with Restock.

# 1.14 (October 27th, 2019):
* [Change] `KSP 1.8` compatibility. __WARNING__: the mod won't work with version lower than `KSP 1.8`!

# 1.13 (April 23rd, 2019):
* [Change] KSP 1.7 compatibility.

# 1.12 (March 8th, 2019):
* [Fix #19] Compatibility with ReStock mod.

# 1.11 (January 12th, 2019):
* [Enhancement] Add Portuguese localization (PT_br).
* [Fix #16] Parts without bulkheadProfiles breaks KSP 1.6.

# 1.10 (December 22nd, 2018)
* [Change] KSP 1.6 compatibility.

# 1.9 (October 16th, 2018)
* [Change] KSP 1.5 compatibility.

# 1.8 (July 8th, 2018)
* [Change] Upgrade the `ModuleManager` dependency version.
* [Enhancement] Add Italian localization (IT_it).
* [Enhancement] Add Spanish localization (ES_es).

# 1.7 (March 8th, 2018)
* KSP 1.4 support.

# 1.6 (October 2nd, 2017)
* [Change] Put the stock lights in tyhe CCK category.
* [Enhancement] Support CCK Lights "category".

# 1.5 (September 21st, 2017)
* Support localization.
* Add RU localization.

# 1.4.0 (May 25th, 2017)
* KSP 1.3 support.

# 1.3.1 (December 1st, 2016)
* [Enhancement #3] Support lens color in the stock lighting parts.

# 1.3.0 (October 12th, 2016)
* [Enhancement] KSP 1.2 support

# 1.2.4 (May 23, 2016):
* [Change] Add surface light modules to stackable KIS items. Needs Module Manager.

# 1.2.3 (May 13, 2016):
* [Change] Code adjustments to compile under KSP 1.1.2
* [Change] Disable animation on 4-way light since it doesn't work properly anyways.
* [Change] Rename GUI action and event names in 4-way light to identify which light they control.
* [Change] Expose setting "Lens brightness" to allow customizing light's minimum brightness level.

# 1.2.2
* Fixed bug where light color was not correctly restored when loading a ship in the editor.
* Refactored code for expansion (future development of navlights)

# 1.2.1
* Removed extraneous debug log text

# 1.2
* Lights now use a custom light module called SurfaceLight
* The surface lights now default to a color set in the parts file
* The color of the light on the model now chanegs to reflect the light's color

# 1.1 (by peteletroll)
* Lower part temperature tolerance to match the stock lights
* Converted textures to DDS and lowered their size to make them more efficient

# 1.0
* Initial release

