using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using AssetValidator.Settings;

[CreateAssetMenu(fileName = "AV Setting", menuName = "ScriptableObjects/Asset Validator Settings", order = 1)]
public class so_AssetValidationSettings : ScriptableObject
{
    // enum of severity
    [Header("General Settings")]
    public SizeUnit _sizeUnit = default;
    public long _fileSize = default;

    [Header("Texture2D Settings")]
    public Vector2 _textureSize = default;

    [Header("AudioClip Settings")]
    public int example1 = default;

    [Header("Mesh Settings")]
    public int example2 = default;

}