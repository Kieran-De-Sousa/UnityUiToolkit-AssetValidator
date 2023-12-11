using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AssetValidator
{
    /// <summary>
    ///
    /// </summary>
    public static class Constants
    {
        public static List<Type> TYPESTOCHECK = new List<Type>
        {
            typeof(Texture2D),
            typeof(AudioClip),
            typeof(Mesh),
            //TODO: Add more types as necessary
        };

        public static readonly Dictionary<LogLevel, Color> DEFAULT_LOGCOLOURS = new Dictionary<LogLevel, Color>
        {
            {LogLevel.INFO, Color.gray},
            {LogLevel.WARNING, Color.yellow},
            {LogLevel.CRITICAL, Color.red}
        };
    }
}

/// <summary>
///
/// </summary>
public enum LogLevel : int
{
    None        = 0,
    INFO        = 1,
    WARNING     = 1 << 1,
    CRITICAL    = 1 << 2,
};

/// <summary>
///
/// </summary>
public enum SizeUnit : int
{
    None = 0,
    KiloByte = 1,
    MegaByte = 1 << 1,
    GigaByte = 1 << 2,
    TeraByte = 1 << 3,
    PetaByte = 1 << 4,
    // Add more file size types as necessary...
};

[CreateAssetMenu(fileName = "AV Setting", menuName = "ScriptableObjects/Asset Validator Settings", order = 1)]
public class so_AssetValidationSettings : ScriptableObject
{
    // enum of severity
    [Header("General Settings")]
    public SizeUnit _sizeUnit = default;
    public int _fileSize = default;

    [Header("Texture2D Settings")]
    public Vector2 _textureSize = default;

    [Header("AudioClip Settings")]
    public int example1 = default;

    [Header("Mesh Settings")]
    public int example2 = default;

}