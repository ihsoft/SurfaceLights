﻿// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// Author: igor.zavoychinskiy@gmail.com 
// License: Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System;
using System.Linq;
using UnityEngine;

namespace SurfaceLights {

/// <summary>
/// Module that changes len's color on the stock lighting parts. Dimming animation in the stock
/// parts ignores color settings and resets emissive color to default.
/// </summary>
public class ModuleStockLightColoredLens : ModuleColoredLensLight {
  /// <inheritdoc/>
  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    if (!PartLoader.Instance.IsReady()) {
      // Replace animation on database load.
      part.FindModelComponents<Animation>()
          .Where(x => x[animationName] != null)
          .ToList()
          .ForEach(ReplaceLigthOnOffAnimation);
    }
  }

  /// <summary>Replaces stock light dimming animation to properly adjust emissive color.</summary>
  /// <param name="animation">Animation object to fix.</param>
  void ReplaceLigthOnOffAnimation(Animation animation) {
    Debug.LogFormat("Replacing animation clip in part {0} for {1}", part, animation.clip.name);
    var clip = animation.clip;
    clip.ClearCurves();
    clip.SetCurve("", typeof(Material), "_EmissiveColor.a",
                  AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f));
    clip.SetCurve("spotlight", typeof(Light), "m_Intensity",
                  AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f));
  }
}

}  // namespace
