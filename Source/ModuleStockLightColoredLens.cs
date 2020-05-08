﻿// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// Author: igor.zavoychinskiy@gmail.com 
// License: Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using KSPDev.LogUtils;
using System.Linq;
using UnityEngine;

namespace SurfaceLights {

/// <summary>
/// Module that changes len's color on the stock lighting parts. Dimming animation in the stock
/// parts ignores color settings and resets emissive color to default.
/// </summary>
public class ModuleStockLightColoredLens : ModuleColoredLensLight {

  /// <summary>The model path for animation to adjust the emission color.</summary>
  /// <remarks>
  /// It's the model that contains the light's lens. It may change from model to model, but this
  /// module was designed specifically for the game <i>stock</i> parts! So keep this string in sync
  /// with the <i>stock</i> models.
  /// </remarks>
  /// <seealso cref="ReplaceLightOnOffAnimation"/>
  const string EmissiveLensModelPath = "";

  /// <inheritdoc/>
  public override void OnLoad(ConfigNode node) {
    base.OnLoad(node);
    if (!PartLoader.Instance.IsReady()) {
      // Replace animation on database load.
      part.FindModelComponents<Animation>()
          .Where(x => x[animationName] != null)
          .ToList()
          .ForEach(ReplaceLightOnOffAnimation);
    }
  }

  /// <summary>Replaces stock light dimming animation to properly adjust emissive color.</summary>
  /// <param name="animation">Animation object to fix.</param>
  void ReplaceLightOnOffAnimation(Animation animation) {
    HostedDebugLog.Info(this, "Replacing animation clip with {1}", animation.clip.name);
    var clip = animation.clip;
    clip.ClearCurves();
    clip.SetCurve(EmissiveLensModelPath, typeof(Material), "_EmissiveColor.a",
                  AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f));
    clip.SetCurve(lightName, typeof(Light), "m_Intensity",
                  AnimationCurve.EaseInOut(0, 0, 1.0f, 1.0f));
  }
}

}  // namespace
