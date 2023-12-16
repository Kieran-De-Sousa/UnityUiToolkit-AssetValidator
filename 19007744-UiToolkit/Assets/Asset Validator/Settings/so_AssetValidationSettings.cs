using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using AssetValidator.Settings;

[CreateAssetMenu(fileName = "AV Setting", menuName = "ScriptableObjects/Asset Validator Settings", order = 1)]
public class so_AssetValidationSettings : ScriptableObject
{
    [Header("General Settings")]
    public FileSizeSettings _fileSizeSettings;

    [Header("Texture2D Settings")]
    public TextureSizeSettings _textureSizeSettings;

    [Header("AudioClip Settings")]
    public int example1 = default;

    [Header("Mesh Settings")]
    public int example2 = default;

}