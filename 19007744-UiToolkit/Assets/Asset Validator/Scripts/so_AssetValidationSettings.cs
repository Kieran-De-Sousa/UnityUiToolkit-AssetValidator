// Base
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// Unity
using UnityEngine;
using UnityEngine.Serialization;

// Asset Validator namespace
using AssetValidator.Constants;
using AssetValidator.Settings;

[CreateAssetMenu(fileName = Constants.FILE_NAME, menuName = Constants.MENU_NAME, order = 1)]
public class so_AssetValidationSettings : ScriptableObject
{
    [Header("Logging Settings")]
    [Tooltip("The colour to display if an asset passes a validation check.")]
    public Color _passed = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.PASSED];
    [Tooltip("The colour to display if an asset fails a validation check and the check is logged as INFO.")]
    public Color _info = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.INFO];
    [Tooltip("The colour to display if an asset fails a validation check and the check is logged as WARNING.")]
    public Color _warning = AssetValidator.Constants.Constants.DEFAULT_LOGCOLOURS[LogLevel.WARNING];
    [Tooltip("The colour to display if an asset fails a validation check and the check is logged as CRITICAL.")]
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