﻿// Surface Lights
// Author: igor.zavoychinskiy@gmail.com 
// License: Public Domain.
// Github: https://github.com/ihsoft/SurfaceLights

using System;
using UnityEngine;

namespace SurfaceLights {

/// <summary>
/// A module that allows exposing in EVA the light events and fields that are normally accessible
/// in the editor only. 
/// </summary>
/// <remarks>
/// This module allows exposing RGB color settings and light ON/OFF controls in EVA. It's controlled
/// by a special module settings <see cref="allowEvaControl"/>. If this setting is <c>false</c>,
/// then no exposure is made and the default behavior is preserved.  
/// </remarks>
public class ModuleLightEva : ModuleLight {
  /// <summary>Tells if the light can be adjusted and operated from EVA.</summary>
  /// <remarks>It affects the color and lens brightness settings.</remarks>
  [KSPField]
  public bool allowEvaControl;

  /// <inheritdoc/>
  public override void OnStart(StartState state) {
    base.OnStart(state);
    if (allowEvaControl) {
      SetupEvent(LightsOn, SetupEventForEva);
      SetupEvent(LightsOff, SetupEventForEva);
      SetupField(nameof(lightR), SetupFieldForEva);
      SetupField(nameof(lightG), SetupFieldForEva);
      SetupField(nameof(lightB), SetupFieldForEva);
    }
  }
  
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
  #endregion
}

}  // namespace
