// Base
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;

// Asset Validator
using AssetValidator.Settings;

using Object = UnityEngine.Object;

// Root namespace for all Asset Validator-related utilities.
namespace AssetValidator
{
    // Sub-namespace for settings-related utilities.
    namespace Settings
    {
        /// <summary>
        /// Defined log level type to be set by a designer when an asset validation fails
        /// </summary>
        public enum LogLevel : int
        {
            INFO        = 1,
            WARNING     = 1 << 1,
            CRITICAL    = 1 << 2,
            [Obsolete] // Note: This is the only way to hide a specific enum value from use in the inspector, please ignore any warning in Unity...
            PASSED      = 1 << 3,
        };

        /// <summary>
        /// Defined file size units.
        /// </summary>
        /// <remarks>Initialised as <c>int</c> instead of <c>long</c> as <c>long</c> is not able to be set in the Unity Inspector.</remarks>
        public enum SizeUnit : int
        {
            None     = 0,
            Byte     = 1,
            Kilobyte = 1 << 1,
            Megabyte = 1 << 2,
            Gigabyte = 1 << 3,
            Terabyte = 1 << 4,
            // Add more file size types as necessary...
        };

        /// <summary>
        /// Defined types of suffix to check an asset for.
        /// </summary>
        public enum Suffix : int
        {
            None = 0,
            Prefix = 1,     // The start of an asset name.
            Postfix = 2,    // The end of an asset name.
        };

        /// <summary>
        /// Base class inherited by settings to pass Logging, Result, and UI data.
        /// </summary>
        public class SettingsBase
        {
            [Tooltip("The type of log that will be outputted for a failed asset validation.")]
            public LogLevel _logLevel = LogLevel.INFO;
            [HideInInspector] public bool _result = false;
            [HideInInspector] public UIVisuals _uiVisuals = new UIVisuals();
        }

        /// <summary>
        /// Class defining settings used for validating Asset file size.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateGeneral.IsFileSizeValid"/>
        [System.Serializable]
        public class GeneralFileSizeSettings : SettingsBase
        {
            [Tooltip("The unit used to measure the file size of the asset.")]
            public SizeUnit _sizeUnit;
            [Tooltip("The size of the file in the unit of measurement.")]
            public long _fileSize;
        }

        /// <summary>
        /// Class defining settings used for validating prefix / postfix of a files name.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateGeneral.IsPrePostFixValid"/>
        [System.Serializable]
        public class GeneralSuffixSettings : SettingsBase
        {
            [Tooltip("The type to check where the string is: " +
                     "Prefix will check the beginning of the asset name. " +
                     "Postfix will check the end of the asset name.")]
            public Suffix _suffix;
            [Tooltip("The string to check as the prefix / postfix.")]
            public string _suffixString;
        }

        /// <summary>
        /// Class defining settings used for validating Texture2D power of two..
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateTexture2D.IsTexturePowerOfTwo"/>
        [System.Serializable]
        public class TextureIsPowerOfTwoSettings : SettingsBase {}

        /// <summary>
        /// Class defining settings used for validating Texture2D dimensions.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateTexture2D.IsTextureDimensionsValid"/>
        [System.Serializable]
        public class TextureSizeSettings : SettingsBase
        {
            [Tooltip("The maximum width and height in pixels for a Texture.")]
            public Vector2 _textureSize;
        }

        /// <summary>
        /// Class defining settings used for validating the length of an AudioClip.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateAudioClip.IsAudioBelowLength"/>
        [System.Serializable]
        public class AudioLengthSettings : SettingsBase
        {
            [Tooltip("The maximum length of time in seconds an AudioClip can be.")]
            public float _audioClipLength;
        }

        /// <summary>
        /// Class defining settings used for validating the minimum bitrate of an AudioClip.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateAudioClip.IsAudioQualityValid"/>
        [System.Serializable]
        public class AudioBitRateSettings : SettingsBase
        {
            [Tooltip("The minimum bitrate allowed for an AudioClip, measured in KBPS.")]
            public int _minBitrate;
        }

        /// <summary>
        /// Class defining settings used for validating the sample rates of an AudioClip.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateAudioClip.IsValidSampleRate"/>
        [System.Serializable]
        public class AudioSampleRateSettings : SettingsBase
        {
            [Tooltip("A list of different sample rates that are valid for an AudioClip, measured in Hz.")]
            public long[] _sampleRates;
        }

        /// <summary>
        /// Class defining settings used for validating the maximum number of vertices in a Mesh.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateMesh.IsValidVertexCount"/>
        [System.Serializable]
        public class MeshVertexCountSettings : SettingsBase
        {
            [Tooltip("The maximum number of vertices a Mesh can have.")]
            public int _maxVertexCount;
        }

        /// <summary>
        /// Class defining settings used for validating if a Mesh's normals are oriented and smooth.
        /// </summary>
        /// <seealso cref="ValidationMethods.ValidateMesh.IsNormalsValid"/>
        [System.Serializable]
        public class MeshNormalsSettings : SettingsBase {}

        /// -------------------- ADD MORE SETTINGS HERE -------------------- ///
    }

    // Sub-namespace for constant / readonly values used by Asset Validator
    namespace Constants
    {
        /// <summary>
        /// Static class containing string constants and predefined values used by Asset Validator.
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// String constants of Asset Validator
            /// </summary>

            // Tool window
            public const string MENU_ITEM = "Tools/Asset Validator/Asset Validator";
            public const string WINDOW_NAME = "Asset Validator";

            // AV Settings (Scriptable Object)
            public const string FILE_NAME = "AV Setting";
            public const string MENU_NAME = "ScriptableObjects/Asset Validator Settings";

            // Paths
            public const string PATH_UIDOCUMENT = "Assets/Asset Validator/UI/UI Documents/uxml_AssetValidator.uxml";

            // Asset selection
            public const string VE_ASSETCONTAINER = "v_selectedAsset";
            public const string LABEL_ASSET_NAME = "l_selectedAssetName";
            public const string LABEL_ASSET_TYPE = "l_selectedAssetType";

            // Asset Validator validation settings
            public const string OBJECT_SETTINGS = "o_settings";
            public const string SCROLL_SETTINGS = "s_scrollSettings";
            public const string LABEL_RESULT = "l_result";
            public const string VE_RESULT = "ve_result";

            // Toggle Names List
            public static readonly List<string> LIST_T_SETTINGS; // Initialised in constructor...

            // General Asset Validator Settings
            public const string FOLDOUT_GENERAL = "f_generalSettings";
            public const string T_FILESIZE = "t_fileSize";
            public const string T_PREPOSTFIX = "t_prePostFix";

            // Texture2D Asset Validator Settings
            public const string FOLDOUT_TEXTURE2D = "f_texture2DSettings";
            public const string T_POWEROFTWO = "t_powerOfTwo";
            public const string T_TEXTURE_DIMENSIONS = "t_textureDimensions";

            // AudioClip Asset Validator Settings
            public const string FOLDOUT_AUDIOCLIP = "f_audioClipSettings";
            public const string T_CLIPLENGTH = "t_clipLength";
            public const string T_SAMPLERATE = "t_sampleRate";
            public const string T_AUDIOQUALITY = "t_audioQuality";

            // Mesh Asset Validator Settings
            public const string FOLDOUT_MESH = "f_meshSettings";
            public const string T_MESHVERTEX = "t_meshVertex";
            public const string T_MESHNORMALS = "t_meshNormals";

            // Asset Validator button
            public const string BUTTON_VALIDATE = "b_validate";

            public const float ASSET_LOAD_WAIT = 0.1f; // EditorWaitForSeconds value.

            // Default colours
            public static readonly Color DEFAULT_TEXT_COLOUR = new(27,27,27,255);
            public static readonly Color DISABLED_TEXT_COLOUR = Color.grey;
            public static readonly Color ENABLED_RESULT_BOX_COLOUR = Color.black;
            public static readonly Color DISABLED_RESULT_BOX_COLOUR = Color.grey;

            // Types that are validated
            public static readonly List<Type> TYPESTOCHECK = new()
            {
                typeof(Texture2D),
                typeof(AudioClip),
                typeof(Mesh),
                //TODO: Add more types as necessary.
            };

            // Dictionary mapping asset types to their respective validation methods.
            public static readonly Dictionary<Type, Func<Object, so_AssetValidationSettings, so_AssetValidationSettings>> TYPEMETHODASSIGNMENT = new()
            {
                { typeof(Texture2D), AssetValidator.ValidationMethods.ValidateTexture2D.ValidationTexture2D },
                { typeof(AudioClip), AssetValidator.ValidationMethods.ValidateAudioClip.ValidationAudioClip },
                { typeof(Mesh), AssetValidator.ValidationMethods.ValidateMesh.ValidationMesh },
            };

            // Dictionary mapping LogLevel enums to specific default Color values
            public static readonly Dictionary<LogLevel, Color> DEFAULT_LOGCOLOURS = new()
            {
                {LogLevel.PASSED, Color.green},
                {LogLevel.INFO, new Color(197, 197, 197, 255)},
                {LogLevel.WARNING, Color.yellow},
                {LogLevel.CRITICAL, Color.red}
            };

            // Dictionary mapping SizeUnit enums to their respective byte values.
            public static readonly Dictionary<SizeUnit, long> TO_BYTES = new()
            {
                {SizeUnit.Byte, 1},                 // Byte     = 1 byte
                {SizeUnit.Kilobyte, 1000},          // Kilobyte = 1,000 bytes
                {SizeUnit.Megabyte, 1000000},       // Megabyte = 1,000,000 bytes
                {SizeUnit.Gigabyte, 1000000000},    // Gigabyte = 1,000,000,000 bytes
                {SizeUnit.Terabyte, 1000000000000}, // Terabyte = 1,000,000,000,000 bytes
                // NOTE: Add more values as necessary.
            };

            static Constants()
            {
                // Instantiate the list values here to link to other systems.
                LIST_T_SETTINGS = new List<string>
                {
                    T_FILESIZE,
                    T_PREPOSTFIX,
                    T_POWEROFTWO,
                    T_TEXTURE_DIMENSIONS,
                    T_CLIPLENGTH,
                    T_SAMPLERATE,
                    T_AUDIOQUALITY,
                    T_MESHVERTEX,
                    T_MESHNORMALS,
                    // NOTE: Add additional constants here...
                };
            }
        }
    }
}