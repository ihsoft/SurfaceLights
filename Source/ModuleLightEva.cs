﻿// Surface Lights
// Author: igor.zavoychinskiy@gmail.com 
// License: Public Domain.
// Github: https://github.com/ihsoft/SurfaceLights

using KSPDev.GUIUtils;
using KSPDev.LogUtils;
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
public class ModuleLightEva : ModuleLight, IsLocalizableModule {
  #region Part config fields
  /// <summary>Tells if the light can be adjusted and operated from EVA.</summary>
  [KSPField]
  public bool allowEvaControl;

  /// <summary>
  /// Tells if the <c>ModuleAnimateGeneric</c> setting can be adjusted and operated from EVA.
  /// </summary>
  [KSPField]
  public bool allowEvaAnimationControl;

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
  [KSPField(advancedTweakable = true)]
  [UI_FloatRange(stepIncrement = 1f, minValue = 10f, maxValue = 180f)]
  [LocalizableItem(
      tag = "#SurfaceLights_ModuleLightEva_SpotAngle",
      defaultTemplate = "Beam angle",
      description = "A UI control that allows setting spotlight beam angle.")]
  public float spotAngle;

  /// <summary>Allows changing the light(s) range.</summary>
  [KSPField(advancedTweakable = true)]
  [UI_FloatRange(stepIncrement = 0.1f, minValue = 0f, maxValue = 100f)]
  [LocalizableItem(
      tag = "#SurfaceLights_ModuleLightEva_LightRange",
      defaultTemplate = "Light range",
      description = "A UI control that allows setting the max range of the light.")]
  public float lightRange;

  /// <summary>Resets all the adjustable settings to the state on the scene load.</summary>
  /// <summary>Once the scene is saved, this state becomes the base.</summary>
  [KSPEvent(guiActive = true, guiActiveEditor = true, unfocusedRange = 10f)]
  [LocalizableItem(
      tag = "#SurfaceLights_ModuleLightEva_ResetUnsaved",
      defaultTemplate = "Reset changes",
      description = "A PAW action that resets all changed values to the state at the last load.")]
  public void ResetUnsavedSettings() {
    RestorePersistedSettings();
  }
  #endregion

  #region Local fields and properties
  /// <summary>Spot angle of the light at the scene load.</summary>
  /// <seealso cref="SavePersistedSettings"/>
  float _originalSpotAngle;

  /// <summary>Light's range at the scene load.</summary>
  /// <seealso cref="SavePersistedSettings"/>
  float _originalLightRange;

  /// <summary>Light's color at the scene load.</summary>
  /// <seealso cref="SavePersistedSettings"/>
  Color _originalColor;

  /// <summary>The original values of all the <c>ModuleAnimateGeneric</c> modules.</summary>
  /// <seealso cref="SavePersistedSettings"/>
  /// <seealso cref="allowEvaAnimationControl"/>
  float[] _originalAnimationSettings = new float[0];
  #endregion

  #region IsLocalizableModule implementation
  /// <inheritdoc/>
  public void LocalizeModule() {
    LocalizationLoader.LoadItemsInModule(this);
  }
  #endregion

  #region PartModule overrides
  /// <inheritdoc />
  public override void OnAwake()
  {
    base.OnAwake();
    LocalizeModule();
  }

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
    SavePersistedSettings();

    // Populate the UI controls with the actual data from the model.
    var refLight = lights[0];
    if (lights.Count > 1) {
      HostedDebugLog.Warning(
          this, "Multiple lights are not supported! Use to the first one as the reference.");
    }
    spotAngle = refLight.spotAngle;
    lightRange = refLight.range;

    // Setup the stock fields and events for EVA usage. 
    SetupField(nameof(lightR), SetupFieldForEva);
    SetupField(nameof(lightG), SetupFieldForEva);
    SetupField(nameof(lightB), SetupFieldForEva);

    SetupEvent(ToggleLights, SetupEventForEva);
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

    // Enable generic animations if there are eligible.
    foreach (var animationModule in GetGenericAnimationModulesForEva()) {
      SetupFieldForEva(animationModule.Fields[nameof(ModuleAnimateGeneric.deployPercent)]);
    }
  }
  #endregion

  #region Inheritable utility methods
  /// <summary>Makes the event to be fully accessible from EVA.</summary>
  protected static void SetupEventForEva(BaseEvent kspEvent) {
    kspEvent.guiActiveUnfocused = true;
    kspEvent.guiActiveUncommand = true;
  }

  /// <summary>Makes the field to be fully accessible from EVA.</summary>
  protected static void SetupFieldForEva(BaseField kspField) {
    kspField.guiActiveUnfocused = true;
    kspField.guiActive = true;
  }

  /// <summary>Saves the light settings from the loaded part state.</summary>
  /// <remarks>
  /// This state is already affected by any overrides. It's not the default settings in the part
  /// prefab.
  /// </remarks>
  /// <seealso cref="RestorePersistedSettings"/>
  protected virtual void SavePersistedSettings() {
    var refLight = lights[0];
    _originalSpotAngle = refLight.spotAngle;
    _originalLightRange = refLight.range;
    _originalColor = new Color(lightR, lightG, lightB);

    var animationModules = GetGenericAnimationModulesForEva();
    _originalAnimationSettings = new float[animationModules.Length];
    for (var i = 0; i < animationModules.Length; i++) {
      var animationModule = animationModules[i];
      var kspField = animationModule.Fields[nameof(ModuleAnimateGeneric.deployPercent)];
      SetupFieldForEva(kspField);
      _originalAnimationSettings[i] = kspField.GetValue<float>(animationModule);
    }
  }

  /// <summary>Restores light settings to what was in effect at the part load.</summary>
  /// <remarks>
  /// This method must restore values via the <c>BasicField.SetValue</c> method. Adjusting the part
  /// in any other way would likely introduce a state inconsistency. 
  /// </remarks>
  /// <seealso cref="SavePersistedSettings"/>
  /// <seealso cref="ResetUnsavedSettings"/>
  protected virtual void RestorePersistedSettings() {
    SetupField(nameof(spotAngle), f => f.SetValue(_originalSpotAngle, this));
    SetupField(nameof(lightRange), f => f.SetValue(_originalLightRange, this));
    SetupField(nameof(lightR), f => f.SetValue(_originalColor.r, this));
    SetupField(nameof(lightG), f => f.SetValue(_originalColor.g, this));
    SetupField(nameof(lightB), f => f.SetValue(_originalColor.b, this));

    var animationModules = GetGenericAnimationModulesForEva();
    for (var i = 0; i < Math.Min(animationModules.Length, _originalAnimationSettings.Length); i++) {
      var animationModule = animationModules[i];
      var kspField = animationModule.Fields[nameof(ModuleAnimateGeneric.deployPercent)];
      kspField.SetValue(_originalAnimationSettings[i], animationModule);
    }
  }

  /// <summary>Applies a setup function on a KSP part module event.</summary>
  /// <param name="eventFn">The event's method signature.</param>
  /// <param name="setupFn">The function to apply to the event if the one is found.</param>
  protected void SetupEvent(Action eventFn, Action<BaseEvent> setupFn) {
    var moduleEvent = Events[eventFn.Method.Name];
    if (moduleEvent == null) {
      HostedDebugLog.Error(this, "Cannot find event: {0}", eventFn.Method.Name);
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
      HostedDebugLog.Error(this, "Cannot find field: {0}", fieldName);
      return;
    }
    setupFn.Invoke(kspField);
  }

  /// <summary>Ensures the FloatRange UI control can deal with the value.</summary>
  /// <remarks>Call it when the upper bound of the range is not known in advance.</remarks>
  /// <param name="field">The FloatRange field to adjust.</param>
  /// <param name="actualValue">The value that needs to fit the control.</param>
  protected void AdjustUiFloatMax(BaseField field, float actualValue) {
    SetupFloatUiControlMax(field, field.uiControlEditor, actualValue);
    SetupFloatUiControlMax(field, field.uiControlFlight, actualValue);
  }

  /// <summary>Returns generic animation modules that can be adjusted for EVA.</summary>
  /// <remarks>
  /// If settings <see cref="allowEvaAnimationControl"/> is not set, then the result is always an
  /// empty array. Otherwise, the existing modules on the part that allows adjusting deploy limit in
  /// the editor will be returned for enabling them in EVA.
  /// </remarks>
  /// <returns>Empty array or an array of the modules that acn be adjusted for EVA.</returns>
  protected ModuleAnimateGeneric[] GetGenericAnimationModulesForEva() {
    if (allowEvaAnimationControl) {
      return part.Modules.OfType<ModuleAnimateGeneric>()
          .Where(x => x.allowDeployLimit)
          .ToArray();
    }
    return new ModuleAnimateGeneric[0];
  }
  #endregion

  #region Local utility methods
  /// <summary>
  /// Ensures that max setting of the FloatRange UI control is not less than the provided value. 
  /// </summary>
  void SetupFloatUiControlMax(BaseField field, UI_Control control, float refValue) {
    if (control != null) {
      var uiFloat = control as UI_FloatRange;
      if (uiFloat == null) {
        HostedDebugLog.Error(
            this, "Field is not of a FloatRange type: {0}", field.MemberInfo.Name);
        return;
      }
      if (uiFloat.maxValue < refValue) {
        HostedDebugLog.Fine(
            this, "Adjust field max value: field={0}, oldMax={1}, newMax={2}",
            field.MemberInfo.Name, uiFloat.maxValue, refValue);
        uiFloat.maxValue = refValue;
      }
    }
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
