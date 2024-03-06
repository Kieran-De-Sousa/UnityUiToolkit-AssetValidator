using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Serialization;

using AssetValidator.Constants;
using AssetValidator.Settings;

[CreateAssetMenu(fileName = "AV Setting", menuName = "ScriptableObjects/Asset Validator Settings", order = 1)]
public class so_AssetValidationSettings : ScriptableObject
{
    [Header("Logging Settings")]
    public Color _passed = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.PASSED];
    public Color _info = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.INFO];
    public Color _warning = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.WARNING];
    public Color _critical = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.CRITICAL];

    public Dictionary<LogLevel, Color> _logColours;

    // Settings Base List
    [HideInInspector] public List<SettingsBase> _settingsList = new List<SettingsBase>();

    [Header("General Settings")]
    public GeneralFileSizeSettings _generalFileSizeSettings = new GeneralFileSizeSettings();
    public GeneralSuffixSettings _generalSuffixSettings = new GeneralSuffixSettings();

    [Header("Texture2D Settings")]
    public TextureIsPowerOfTwoSettings _textureIsPowerOfTwoSettings = new TextureIsPowerOfTwoSettings();
    public TextureSizeSettings _textureSizeSettings = new TextureSizeSettings();

    [Header("AudioClip Settings")]
    public AudioLengthSettings _audioLengthSettings = new AudioLengthSettings();
    public AudioBitRateSettings _audioBitRateSettings = new AudioBitRateSettings();
    public AudioSampleRateSettings _audioSampleRateSettings = new AudioSampleRateSettings();

    [Header("Mesh Settings")]
    public MeshVertexCountSettings _meshVertexCountSettings = new MeshVertexCountSettings();
    public MeshNormalsSettings _meshNormalsSettings = new MeshNormalsSettings();

    public so_AssetValidationSettings()
    {
        _logColours = new Dictionary<LogLevel, Color>
        {
            {LogLevel.PASSED, _passed},
            {LogLevel.INFO, _info},
            {LogLevel.WARNING, _warning},
            {LogLevel.CRITICAL, _critical}
        };

        // Add derived classes to list of base class.
        _settingsList.AddRange(new List<SettingsBase>
        {
            _generalFileSizeSettings,
            _generalSuffixSettings,
            _textureIsPowerOfTwoSettings,
            _textureSizeSettings,
            _audioLengthSettings,
            _audioSampleRateSettings,
            _audioBitRateSettings,
            _meshVertexCountSettings,
            _meshNormalsSettings
            // NOTE: Add additional settings here...
        });
    }

    private void OnValidate()
    {
        _logColours[LogLevel.PASSED] = _passed;
        _logColours[LogLevel.INFO] = _info;
        _logColours[LogLevel.WARNING] = _warning;
        _logColours[LogLevel.CRITICAL] = _critical;
    }
}