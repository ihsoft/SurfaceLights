// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// This software is distributed under
// a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System;
using System.Linq;
using UnityEngine;

namespace KSP_Light_Mods {

public class SurfaceLight : ModuleLight {
  [KSPField(isPersistant = false)]
  public Vector3 startingRGB = new Vector3(1.0f, 0.9f, 0.8f);

  Material mat;
  Color materialColor;
  Color colorLastFrame;

  const float MIN_BRIGHTNESS = 0.5f;
  protected bool loadedFromSavedCraft = false;

  public override void OnAwake() {
    base.OnAwake();
    mat = GetComponentInChildren<MeshRenderer>().material;
  }

  public override void OnInitialize() {
    base.OnInitialize();

    // When this is loading in the editor, the lightX values come from the part
    // config files. When this is loading in flight, they are saved from the editor.     
    if (HighLogic.LoadedSceneIsEditor) {
      InitEditor();
    } else if (HighLogic.LoadedSceneIsFlight) {
      InitFlight();
    }
  }

  public override void OnLoad(ConfigNode node) {
    loadedFromSavedCraft = true;
  }

  public override void OnUpdate() {     
    // Setting material color is expensive. Only do this in the editor where light
    // color can change, and only do it when the color has changed.
    if (HighLogic.LoadedSceneIsEditor) {
      materialColor = FilterColor(new Color(lightR, lightG, lightB));

      // Update the color of the light texture so that it matches the light color.
      if (materialColor != colorLastFrame) {
        UpdateEditorColors();
      }

      colorLastFrame = materialColor;
    }
  }

  protected virtual void UpdateEditorColors() {
    //print("CUSTOM LIGHT: Updating Color");
    mat.SetColor("_EmissiveColor", materialColor);
  }

  protected virtual void InitEditor() {
    if (!loadedFromSavedCraft) {
      lightR = startingRGB.x;
      lightG = startingRGB.y;
      lightB = startingRGB.z;
    }

    materialColor = FilterColor(new Color(lightR, lightG, lightB));
    colorLastFrame = materialColor;

    mat.SetColor("_EmissiveColor", materialColor);
  }

  protected virtual void InitFlight() {
    mat.SetColor("_EmissiveColor", new Color(lightR, lightG, lightB));
  }

  // Using raw color looks kinda ugly, so do some minor filtering.
  protected Color FilterColor(Color rawColor) {
    rawColor.r = rawColor.r * (1.0f - MIN_BRIGHTNESS) + MIN_BRIGHTNESS;
    rawColor.g = rawColor.g * (1.0f - MIN_BRIGHTNESS) + MIN_BRIGHTNESS;
    rawColor.b = rawColor.b * (1.0f - MIN_BRIGHTNESS) + MIN_BRIGHTNESS;

    return rawColor;
  }
}

}  // namespace
