﻿// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// Author: igor.zavoychinskiy@gmail.com 
// License: Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System.Linq;
using UnityEngine;

namespace SurfaceLights {

/// <summary>
/// A light module that keeps match between emitting light color and the glowing texture
/// color ("lens").
/// </summary>
/// <remarks>
/// If light color is being changed from a script the color may not be properly reflected by the
/// lens. In order to have it working as expected the change must be propagated to the module. See
/// examples below.
/// </remarks>
/// <example>
/// <para>
/// When values are changed via <see cref="BaseField.SetValue"/> method the changes will be
/// automatically picked up.
/// </para>
/// <code><![CDATA[
/// var lightModule = part.GetComponent<ModuleColoredLensLight>();
/// lightModule.Fields["lensBrightness"].SetValue(0.3f, lightModule);
/// lightModule.Fields["lightR"].SetValue(1.0f, lightModule);
/// lightModule.Fields["lightG"].SetValue(0.0f, lightModule);
/// lightModule.Fields["lightB"].SetValue(0.0f, lightModule);
/// ]]></code>
/// <para>
/// When changing values directly via fields the module must be notified to do the update.
/// </para>
/// <code><![CDATA[
/// var lightModule = part.GetComponent<ModuleColoredLensLight>();
/// lightModule.lensBrightness = 0.3f;
/// lightModule.lightR = 1.0f;
/// lightModule.lightG = 0.0f;
/// lightModule.lightB = 0.0f;
/// lightModule.UpdateLightTextureColor();
/// ]]></code>
/// </example>
/// <seealso cref="lensBrightness"/>
public class ModuleColoredLensLight : ModuleLight {
  /// <summary>Defines minimum white color level.</summary>
  /// <remarks>See module remarks with regard to changing this value from as script.</remarks>
  /// <seealso cref="ModuleColoredLensLight"/>
  [KSPField(guiName = "#SurfaceLights_ModuleColoredLensLight_lensBrightness", isPersistant = true)]
  [UI_FloatRange(stepIncrement = 0.05f, maxValue = 1f, minValue = 0f)]
  public float lensBrightness = 0.5f;

  /// <inheritdoc/>
  public override void OnInitialize() {
    base.OnInitialize();
    UpdateLightTextureColor();
  }

  /// <inheritdoc/>
  public override void OnAwake() {
    base.OnAwake();
    Fields["lensBrightness"].OnValueModified += (x => UpdateLightTextureColor());
    Fields["lightR"].OnValueModified += (x => UpdateLightTextureColor());
    Fields["lightG"].OnValueModified += (x => UpdateLightTextureColor());
    Fields["lightB"].OnValueModified += (x => UpdateLightTextureColor());
  }

  /// <summary>
  /// Updates the emissive color of the material so that it matches the light color.
  /// </summary>
  public virtual void UpdateLightTextureColor() {
    // By default, update all the emissive materials.
    part.FindModelComponents<Renderer>()
        .Where(r => r.material.HasProperty("_EmissiveColor"))
        .ToList()
        .ForEach(r => r.material.SetColor("_EmissiveColor", GetLightTextureColor()));
  }

  /// <summary>Returns a color to set on the emission texture.</summary>
  /// <remarks>Using raw color looks kinda ugly, so do some minor filtering. Add an intensity offset
  /// to the light's color to make the texture be colored even when the light is turned off.
  /// </remarks>
  protected virtual Color GetLightTextureColor() {
    return new Color(lightR * (1.0f - lensBrightness) + lensBrightness,
                     lightG * (1.0f - lensBrightness) + lensBrightness,
                     lightB * (1.0f - lensBrightness) + lensBrightness);
  }
}

}  // namespace
