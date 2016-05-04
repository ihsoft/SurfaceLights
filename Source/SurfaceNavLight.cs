// Surface Mounted Stock-Alike Lights for Self-Illumination
// Author: Why485 (http://forum.kerbalspaceprogram.com/index.php?/profile/26795-why485/)
// This software is distributed under
// a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International.

using System;
using System.Linq;
using UnityEngine;

namespace KSP_Light_Mods {
class SurfaceNavLight : SurfaceLight {
  [KSPField(isPersistant = false)]
  public float minDistance = 1.0f;

  [KSPField(isPersistant = false)]
  public float maxDistance = 10000.0f;

  [KSPField(isPersistant = false)]
  public float scaleFactor = 300.0f;

  [KSPField(isPersistant = false)]
  public float billboardOffset = 0.2f;

  [KSPField(guiName = "Unlimited view", isPersistant = true,
            guiActive = false, guiActiveEditor = true)]
  [UI_Toggle()]
  public bool full360View = true;

  Camera fxCam;

  MeshRenderer model;
  Material billboardMat;

  Transform billboard;
  Transform billboardQuad;

  const float BILLBOARD_ALPHA = 0.25f;

  public override void OnAwake() {
    base.OnAwake();
            
    // After KSP compiles a model, it all gets put under a GameObject called "model"
    billboard = transform.FindChild("model/FlareBillboard");
    billboardQuad = billboard.FindChild("Quad");

    model = billboardQuad.GetComponent<MeshRenderer>();
    billboardMat = model.material;

    model.enabled = false;

    Camera[] allCams = Camera.allCameras;
    foreach (Camera cam in allCams) {
      if (cam.name == "FXCamera") {
        fxCam = cam;
      }
    }
  }

  public override void OnInitialize() {
    base.OnInitialize();

    billboardMat.SetColor(
        "_TintColor",
        new Color(lightR * BILLBOARD_ALPHA, lightG * BILLBOARD_ALPHA, lightB * BILLBOARD_ALPHA));
    billboardQuad.localPosition = Vector3.forward * billboardOffset;
  }

  protected override void InitFlight() {
    base.InitFlight();

    billboardMat.SetColor(
        "_TintColor",
        new Color(lightR * BILLBOARD_ALPHA, lightG * BILLBOARD_ALPHA, lightB * BILLBOARD_ALPHA));
  }

  protected override void UpdateEditorColors() {
    base.UpdateEditorColors();

    billboardMat.SetColor(
        "_TintColor",
        new Color(lightR * BILLBOARD_ALPHA, lightG * BILLBOARD_ALPHA, lightB * BILLBOARD_ALPHA));
  }

  public override void OnUpdate() {
    base.OnUpdate();

    // KSP uses a bunch of different cameras.
    Camera cam = ChooseAppropriateCamera();

    if (isOn) {
      model.enabled = true;    

      // Rotate billboard towards the camera.
      billboard.rotation = Quaternion.LookRotation(cam.transform.position - transform.position);

      // Scale the billboard with distance so the navlight can be seen from afar.
      float distance = Vector3.Distance(cam.transform.position, transform.position);

      if (distance <= minDistance) {
        billboard.localScale = Vector3.one;
      } else if (distance >= maxDistance) {
        billboard.localScale = Vector3.one * scaleFactor;
      } else {
        billboard.localScale = Vector3.one
            * Mathf.Lerp(1.0f, scaleFactor, Mathf.InverseLerp(minDistance, maxDistance, distance));
      }

      // Limited angles means that the navlight has a realistic viewing range.
      if (!full360View) {
        Vector3 relCamPos = transform.InverseTransformPoint(cam.transform.position);
        bool hideModel = false;

        if (relCamPos.z < 0.0f && Vector3.Angle(Vector3.forward, relCamPos) > 160.0f) {
          hideModel = true;
        }

        model.enabled = hideModel;
      }
    } else {
      model.enabled = false;
    }            
  }

  Camera ChooseAppropriateCamera() {
    if (HighLogic.LoadedSceneIsFlight) {
      return fxCam;
    } else {
      return Camera.main;
    }
  }
}

}  // namespace
