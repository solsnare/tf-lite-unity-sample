﻿using System;
using System.Linq;
using TensorFlowLite;
using UnityEngine;
using UnityEngine.UI;

public class TestWebcamRotation : MonoBehaviour
{
    [SerializeField] RawImage rawInputView = null;
    [SerializeField] RawImage resizedInputView = null;
    [SerializeField] RawImage rawInputTrimedView = null;
    [SerializeField] Dropdown aspectModeDropdown = null;
    [SerializeField] Toggle mirrorHorizontalToggle = null;
    [SerializeField] Toggle mirrorVerticalToggle = null;
    [SerializeField] Text infoLabel = null;


    WebCamTexture webcamTexture;
    TextureResizer resizer;
    TextureResizer.ResizeOptions resizeOptions;
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    void Start()
    {
        string cameraName = WebCamUtil.FindName(WebCamKind.WideAngle, false);
        webcamTexture = new WebCamTexture(cameraName, 1280, 720, 30);
        webcamTexture.Play();
        rawInputView.texture = webcamTexture;

        resizer = new TextureResizer();
        resizeOptions = new TextureResizer.ResizeOptions()
        {
            aspectMode = AspectMode.Fill,
            rotationDegree = 0,
            mirrorHorizontal = false,
            mirrorVertical = false,
            width = 720,
            height = 720,
        };

        // Setup UIs

        // Dropdown
        var modes = Enum.GetValues(typeof(AspectMode)).Cast<AspectMode>();
        aspectModeDropdown.ClearOptions();
        aspectModeDropdown.AddOptions(modes.Select(m => new Dropdown.OptionData(m.ToString())).ToList());
        aspectModeDropdown.SetValueWithoutNotify((int)resizeOptions.aspectMode);
        aspectModeDropdown.onValueChanged.AddListener((int index) =>
        {
            resizeOptions.aspectMode = (AspectMode)index;
        });

        // Mirror
        mirrorHorizontalToggle.SetIsOnWithoutNotify(resizeOptions.mirrorHorizontal);
        mirrorHorizontalToggle.onValueChanged.AddListener((bool isOn) =>
        {
            resizeOptions.mirrorHorizontal = isOn;
        });
        mirrorVerticalToggle.SetIsOnWithoutNotify(resizeOptions.mirrorVertical);
        mirrorVerticalToggle.onValueChanged.AddListener((bool isOn) =>
        {
            resizeOptions.mirrorVertical = isOn;
        });


    }

    void OnDestroy()
    {
        webcamTexture?.Stop();
        resizer?.Dispose();
    }

    void Update()
    {
        resizedInputView.texture = resizer.Resize(webcamTexture, resizeOptions);

        rawInputTrimedView.texture = webcamTexture;
        rawInputTrimedView.material = resizer.material;

        var options = resizeOptions.GetModifedForWebcam(webcamTexture);

        sb.Clear();
        sb.AppendLine("Midified Options");
        sb.AppendLine($"Rotation: {options.rotationDegree}");
        sb.AppendLine($"Mirror Horizontal: {options.mirrorHorizontal}");
        sb.AppendLine($"Mirror Vertical: {options.mirrorVertical}");
        infoLabel.text = sb.ToString();

    }
}
