// Surface Mounted Stock-Alike Lights for Self-Illumination
// Mod's author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// Module author: igor.zavoychinskiy@gmail.com
// This software is distributed under
// a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System;
using UnityEngine;

namespace KSP_Light_Mods {

/// <summary>A module to control a light part that has multiple light sources that can be
/// controlled independently.</summary>
/// <remarks>
/// Light name is used to make GUI strings so, keep it short and human readable.
/// E.g. "L1", "L2", etc.
/// </remarks>
public class ModuleMultiPointSurfaceLight : ModuleLight {
  /// <summary>Last known light state.</summary>
  bool lastOnState = false;

  /// <summary>All the light modules on the part.</summary>
  ModuleLight[] allLightModules {
    get {
      if (_allLightModules == null) {
        _allLightModules = part.GetComponents<ModuleLight>();
      }
      return _allLightModules;
    }
  }
  ModuleLight[] _allLightModules;

  /// <summary>Dimming animation effect.</summary>
  /// <remarks>For now it's global, for all the lights.</remarks>
  AnimationState animationState {
    get {
      if (!_animationState) {
        FindAnimation();
      }
      return _animationState;
    }
  }
  AnimationState _animationState;

  /// <summary>Scans for the animation in the model and records the found value.</summary>
  /// <remarks>Scanning model for components is an expensive operation so, cache the found values
  /// assuming they won't change.
  /// <para>In case of some control needs to mangle with animations in runtime allow this method to
  /// be called by the children.</para>
  /// </remarks>
  protected void FindAnimation() {
    var animations = part.FindModelComponents<Animation>();
    foreach (var modelAnimation in animations) {
      var modelAnimationState = modelAnimation[animationName];
      if (modelAnimationState != null) {
        _animationState = modelAnimationState;
        _animationState.normalizedSpeed = 0;  // Freeze.
        modelAnimation.Play(animationName);
        break;
      }
    }
  }

  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    if (animationState) {
      UpdateAnimationState();
    } else {
      Debug.LogErrorFormat("Bad model or config in part {0}. Cannot find animation: {1}",
                           part, animationName);
    }
  }
  
  public override void OnStart(PartModule.StartState state) {
    base.OnStart(state);
    UpdateAnimationState();

    // Rewrite GUI names to identify different lights. Only do it when part is created in the
    // editor. Rewritten names will be stored in the save file so, no need to fix them every time.
    // Though, editor starts part on every load so, soem check is needed to not modify names
    // endlessly.
    if (state == PartModule.StartState.Editor) {
      foreach (var action in Actions) {
        if (!action.guiName.StartsWith(lightName)) {
          action.guiName = lightName + ": " + action.guiName;
        }
      }
      foreach (var uiEvent in Events) {
        if (!uiEvent.guiName.StartsWith(lightName)) {
          uiEvent.guiName = lightName + ": " + uiEvent.guiName;
        }
      }
      var uiFields = new[] {Fields["lightR"], Fields["lightG"], Fields["lightB"]};
      foreach (var field in uiFields) {
        field.guiName = lightName + ": " + field.guiName;
      }
    }
  }

  public void Update() {
	  // Verify global state of the animtation on every light state change.
	  // FIXME: It's not the best idea from the performance perspective. So, fix the animation!
    if (lastOnState != isOn) {
      UpdateAnimationState();
    }
  }

	/// <summary>Verifies if all lights are OFF and sets animation to OFF.</summary>
	/// <remarks>Animation shows light texture for all the lights at the same time. To make part
  /// behaving nicely show the lighted texture if at least one of the lights is active. Disable the
  /// texture if none is enabled.</remarks>
	void UpdateAnimationState() {
    lastOnState = isOn;
    if (animationState) {
      var allAreOff = true;
      foreach (var module in allLightModules) {
        allAreOff &= !module.isOn;
      }
      animationState.normalizedTime = allAreOff ? 0 : 1;
    }
  }
}

}  // namespace
