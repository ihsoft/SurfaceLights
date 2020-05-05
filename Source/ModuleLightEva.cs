﻿// Surface Lights
// Author: igor.zavoychinskiy@gmail.com 
// License: Public Domain.
// Github: https://github.com/ihsoft/SurfaceLights

using System;
using System.Linq;
using UnityEngine;

namespace SurfaceLights {

/// <summary>
/// A module that allows exposing in EVA the light events and fields that are normally accessible
/// in the editor only. 
/// </summary>
/// <remarks>
/// This module allows exposing some of the key light settings in both editor and flight. It's
/// controlled by a special module settings <see cref="allowEvaControl"/>. If this setting is
/// <c>false</c>, then no exposure is made and the default behavior is preserved.  
/// </remarks>
public class ModuleLightEva : ModuleLight {
  #region Part config fields
  /// <summary>Tells if the light can be adjusted and operated from EVA.</summary>
  [KSPField]
  public bool allowEvaControl;

  /// <summary>Sets the maximum range value in GUI.</summary>
  /// <remarks>
  /// If it's less than the model light currently have, the model's value will be used instead.
  /// </remarks>
  [KSPField]
  public float maxLightRange = -1f;
  #endregion

  #region Persistant config fields
  /// <summary>Spotlight angle to apply to the lights in the part.</summary>
  /// <remarks>
  /// If <c>null</c>, then no override is applied. The override is applied to all the lights in
  /// the part.
  /// </remarks>
  [KSPField(isPersistant = true)]
  public float spotAngleOverride = -1;

  /// <summary>Light range value to apply to the lights in the part.</summary>
  /// <remarks>
  /// If <c>null</c>, then no override is applied. The override is applied to all the lights in
  /// the part.
  /// </remarks>
  [KSPField(isPersistant = true)]
  public float lightRangeOverride = -1;
  #endregion

  #region PAW controls
  /// <summary>Allows changing the spot angle of the light(s).</summary>
  [KSPField(guiName = "#SurfaceLights_ModuleLightEva_SpotAngle", advancedTweakable = true)]
  [UI_FloatRange(stepIncrement = 1f, minValue = 10f, maxValue = 180f)]
  public float spotAngle;

  /// <summary>Allows changing the light(s) range.</summary>
  [KSPField(guiName = "#SurfaceLights_ModuleLightEva_LightRange", advancedTweakable = true)]
  [UI_FloatRange(stepIncrement = 0.1f, minValue = 0f, maxValue = 100f)]
  public float lightRange;
  #endregion

  #region PartModule overrides
  /// <inheritdoc/>
  public override void OnStart(StartState state) {
    base.OnStart(state);
    if (spotAngleOverride > 0) {
      UpdateSpotLightAngle(spotAngleOverride);
    }
    if (lightRangeOverride > 0) {
      UpdateLightRange(lightRangeOverride);
    }
    if (!allowEvaControl) {
      return;  // Nothing to do.
    }

    // Populate the UI controls with the actual data from the model.
    var refLight = lights[0];
    if (lights.Count > 1) {
      Debug.LogWarning(
          "Multiple lights are not supported! Use to the first one as the reference.");
    }
    spotAngle = refLight.spotAngle;
    lightRange = refLight.range;

    // Setup the stock fields and events for EVA usage. 
    SetupEvent(LightsOn, SetupEventForEva);
    SetupEvent(LightsOff, SetupEventForEva);
    SetupField(nameof(lightR), SetupFieldForEva);
    SetupField(nameof(lightG), SetupFieldForEva);
    SetupField(nameof(lightB), SetupFieldForEva);

    SetupEvent(ResetUnsavedSettings, SetupEventForEva);

    // Allow spotlight angle to be customized.
    if (refLight.type == LightType.Spot) {
      SetupField(nameof(spotAngle), f => {
        AdjustUiFloatMax(f, spotAngle);
        SetupFieldForEva(f);
        f.OnValueModified += x => UpdateSpotLightAngle((float) x);
      });
    }

    // Allow light range to be customized.
    SetupField(nameof(lightRange), f => {
      AdjustUiFloatMax(f, Math.Max(lightRange, maxLightRange));
      SetupFieldForEva(f);
      f.OnValueModified += x => UpdateLightRange((float) x);
    });
    }
  }
  #endregion

  #region Inheritable utility methods
  /// <summary>Makes the event to be fully accessible from EVA.</summary>
  protected static void SetupEventForEva(BaseEvent kspEvent) {
    kspEvent.guiActive = true;
    kspEvent.guiActiveUnfocused = true;
    kspEvent.guiActiveUncommand = true;
  }

  /// <summary>Makes the field to be fully accessible from EVA.</summary>
  protected static void SetupFieldForEva(BaseField kspField) {
    kspField.guiActiveUnfocused = true;
    kspField.guiActive = true;
  }

  /// <summary>Applies a setup function on a KSP part module event.</summary>
  /// <param name="eventFn">The event's method signature.</param>
  /// <param name="setupFn">The function to apply to the event if the one is found.</param>
  protected void SetupEvent(Action eventFn, Action<BaseEvent> setupFn) {
    var moduleEvent = Events[eventFn.Method.Name];
    if (moduleEvent == null) {
      Debug.LogErrorFormat("Cannot find event: {0}", eventFn.Method.Name);
      return;
    }
    setupFn.Invoke(moduleEvent);
  }
  
  /// <summary>Applies a setup function on a KSP part module field.</summary>
  /// <param name="fieldName">The fields name.</param>
  /// <param name="setupFn">The function to apply to the field if the one is found.</param>
  protected void SetupField(string fieldName, Action<BaseField> setupFn) {
    var kspField = Fields[fieldName];
    if (kspField == null) {
      Debug.LogErrorFormat("Cannot find field: {0}", fieldName);
      return;
    }
    setupFn.Invoke(kspField);
  }

  /// <summary>Ensures the FloatRange UI control can deal with the value.</summary>
  /// <remarks>Call it when the upper bound of the range is not known in advance.</remarks>
  /// <param name="field">The FloatRange field to adjust.</param>
  /// <param name="actualValue">The value that needs to fit the control.</param>
  protected static void AdjustUiFloatMax(BaseField field, float actualValue) {
    SetupFloatUiControlMax(field, field.uiControlEditor, actualValue);
    SetupFloatUiControlMax(field, field.uiControlFlight, actualValue);
  }
  #endregion

  #region Local utility methods
  /// <summary>
  /// Ensures that max setting of the FloatRange UI control is not less than the provided value. 
  /// </summary>
  static void SetupFloatUiControlMax(BaseField field, UI_Control control, float refValue) {
    if (control != null) {
      var uiFloat = control as UI_FloatRange;
      if (uiFloat == null) {
        Debug.LogErrorFormat(
            "Field is not of a FloatRange type: {0}.{1}",
            field.MemberInfo.DeclaringType, field.MemberInfo.Name);
        return;
      }
      if (uiFloat.maxValue < refValue) {
        uiFloat.maxValue = refValue;
      }
    }
    return;
  } 

  /// <summary>Updates the spot light angle in all of the lights in the module.</summary>
  void UpdateSpotLightAngle(float newValue) {
    spotAngleOverride = newValue;
    lights.ForEach(l => l.spotAngle = newValue);
  }

  /// <summary>Updates the light range in all of the lights in the module.</summary>
  void UpdateLightRange(float newValue) {
    lightRangeOverride = newValue;
    lights.ForEach(l => l.range = newValue);
  }
  #endregion
}

}  // namespace
