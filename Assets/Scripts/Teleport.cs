﻿// Copyright 2014 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour, ICardboardGazeResponder {
  private Vector3 startingPosition;
  private Vector3 smallThumb;
  private Vector3 largeThumb;

  void Start() {
    startingPosition = transform.localPosition;
    smallThumb = new Vector3(.2051785f, .2051785f, .2051785f);
    largeThumb = new Vector3(.3F, .3F, .3F);
    SetGazedAt(false);
  }

  void LateUpdate() {
    Cardboard.SDK.UpdateState();
    if (Cardboard.SDK.BackButtonPressed) {
      Application.Quit();
    }
  }

  public void SetGazedAt(bool gazedAt) {
      Renderer rend = GetComponent<Renderer>();
      Color color = rend.material.color;
      if (gazedAt) {
          color = Color.green;
          color.a = .4f;
          rend.transform.localScale = largeThumb;
      } else {
          color = Color.red;
          color.a = 1f;
          rend.transform.localScale = smallThumb;
      }
      rend.material.color = color;
  }

  public void Reset() {
    transform.localPosition = startingPosition;
  }

  public void ToggleVRMode() {
    Cardboard.SDK.VRModeEnabled = !Cardboard.SDK.VRModeEnabled;
  }

  public void ToggleDistortionCorrection() {
    switch(Cardboard.SDK.DistortionCorrection) {
    case Cardboard.DistortionCorrectionMethod.Unity:
      Cardboard.SDK.DistortionCorrection = Cardboard.DistortionCorrectionMethod.Native;
      break;
    case Cardboard.DistortionCorrectionMethod.Native:
      Cardboard.SDK.DistortionCorrection = Cardboard.DistortionCorrectionMethod.None;
      break;
    case Cardboard.DistortionCorrectionMethod.None:
    default:
      Cardboard.SDK.DistortionCorrection = Cardboard.DistortionCorrectionMethod.Unity;
      break;
    }
  }

  public void ToggleDirectRender() {
    Cardboard.Controller.directRender = !Cardboard.Controller.directRender;
  }

  public void TeleportRandomly() {
    Vector3 direction = Random.onUnitSphere;
    direction.y = Mathf.Clamp(direction.y, 0.5f, 1f);
    float distance = 2 * Random.value + 1.5f;
    transform.localPosition = direction * distance;
  }

  #region ICardboardGazeResponder implementation

  /// Called when the user is looking on a GameObject with this script,
  /// as long as it is set to an appropriate layer (see CardboardGaze).
  public void OnGazeEnter() {
    SetGazedAt(true);
  }

  /// Called when the user stops looking on the GameObject, after OnGazeEnter
  /// was already called.
  public void OnGazeExit() {
    SetGazedAt(false);
  }

  // Called when the Cardboard trigger is used, between OnGazeEnter
  /// and OnGazeExit.
  public void OnGazeTrigger() {
    TeleportRandomly();
  }

  #endregion
}