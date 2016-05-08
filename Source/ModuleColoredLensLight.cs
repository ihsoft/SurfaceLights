﻿// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// Author: igor.zavoychinskiy@gmail.com 
// This software is distributed under
// a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System;
using System.Linq;
using UnityEngine;

namespace KSP_Light_Mods {

/// <summary>A light module that keeps macth between emitting light color and the glowing texture
/// color ("lens").</summary>
/// <remarks>Light color changes won't be picked up automaticaly since it's too expensive to do the
/// check on every update. Lens color is updated only once on the part load. If color changes in
/// runtime then the code that does the change must explicitly call
/// <see cref="UpdateLightTextureColor"/> to have lens updated.</remarks>
public class ModuleColoredLensLight : ModuleLight {
  Color currentLensColor;
  float lastLensBrightness;

  /// <summary>Light's material.</summary>
  protected Material lightMaterial {
    get {
      if (!_lightMaterial) {
        _lightMaterial = GetComponentInChildren<MeshRenderer>().material;
      }
      return _lightMaterial;
    }
  }
  Material _lightMaterial;

  [KSPField(guiName = "Lens brightness", isPersistant = true)]
  [UI_FloatRange(stepIncrement = 0.05f, maxValue = 1f, minValue = 0f)]
  public float lensBrightness = 0.5f;

  public override void OnInitialize() {
    base.OnInitialize();
    UpdateLightTextureColor();
  }

  public void Update() {
    // It's to expensive to fetch and compare colors on every single frame. And normally is not
    // needed. Light color is expected to change in the editor but changes in flight can only happen
    // via a script. If this happens the screep must also call this module to have the lens updated.
    if (HighLogic.LoadedSceneIsEditor) {
      UpdateLightTextureColor();
    }
  }

  /// <summary>
  /// Updates the emissive color of the material so that it matches the light color.
  /// </summary>
  /// <remarks>Won't do the update if currecnt material color is the same.</remarks>
  public virtual void UpdateLightTextureColor() {
    var newColor = GetLightTextureColor();
    if (currentLensColor != newColor
        || Math.Abs(lastLensBrightness - lensBrightness) > float.Epsilon) {
      currentLensColor = newColor;
      lastLensBrightness = lensBrightness;
      lightMaterial.SetColor("_EmissiveColor", newColor);
    }
  }

  /// <summary>Returns a color to set on the emission texture.</summary>
  /// <remarks>Using raw color looks kinda ugly, so do some minor filtering. Add an intensity offset
  /// to the light's color to make the texture be colored even when the lgiht is turned off.
  /// </remarks>
  protected virtual Color GetLightTextureColor() {
    return new Color(lightR * (1.0f - lensBrightness) + lensBrightness,
                     lightG * (1.0f - lensBrightness) + lensBrightness,
                     lightB * (1.0f - lensBrightness) + lensBrightness);
  }
}

}  // namespace