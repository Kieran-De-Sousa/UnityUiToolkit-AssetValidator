using System;
using System.Collections;
using System.Collections.Generic;
using AssetValidator.Settings;
using UnityEngine;

namespace AssetValidator
{
    //
    namespace Settings
    {
        /// <summary>
        ///
        /// </summary>
        public enum LogLevel : int
        {
            None        = 0,
            PASSED      = 1,
            INFO        = 1 << 1,
            WARNING     = 1 << 2,
            CRITICAL    = 1 << 3,
        };

        /// <summary>
        ///
        /// </summary>
        /// <remarks>Initialised as <c>int</c> instead of <c>long</c> as <c>long</c> is not available to set in the Unity Inspector.</remarks>
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
        ///
        /// </summary>
        [System.Serializable]
        public struct FileSizeSettings
        {
            public SizeUnit _sizeUnit;
            public long _fileSize;
            public LogLevel _logLevel;
        }

        /// <summary>
        ///
        /// </summary>
        [System.Serializable]
        public struct TextureSizeSettings
        {
            public Vector2 _textureSize;
            public LogLevel _logLevel;
        }
    }

    //
    namespace Constants
    {
        /// <summary>
        ///
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// String constants of Asset Validator
            /// </summary>
            // Tool window
            public const string MENU_ITEM = "Tools/Asset Validator/Asset Validator";
            public const string WINDOW_NAME = "Asset Validator";

            // Paths
            public const string PATH_UIDOCUMENT = "Assets/UI/UI Documents/uxml_AssetValidator.uxml";

            // Asset selection
            public const string VE_ASSETCONTAINER = "v_selectedAsset";
            public const string LABEL_ASSET_NAME = "l_selectedAssetName";
            public const string LABEL_ASSET_TYPE = "l_selectedAssetType";

            // Asset Validator validation settings
            public const string OBJECT_SETTINGS = "o_settings";
            public const string SCROLL_SETTINGS = "s_scrollSettings";

            // General Asset Validator Settings
            public const string FOLDOUT_GENERAL = "f_generalSettings";
            public const string T_FILESIZE = "t_fileSize";

            // Texture2D Asset Validator Settings
            public const string FOLDOUT_TEXTURE2D = "f_texture2DSettings";
            public const string T_POWEROFTWO = "t_powerOfTwo";
            public const string T_TEXTURE_DIMENSIONS = "t_textureDimensions";

            // AudioClip Asset Validator Settings
            public const string FOLDOUT_AUDIOCLIP = "f_audioClipSettings";
            public const string T_AUDIOEXAMPLE = "t_audioClip";

            // Mesh Asset Validator Settings
            public const string FOLDOUT_MESH = "f_meshSettings";
            public const string T_MESHEXAMPLE = "t_mesh";

            // Asset Validator button
            public const string BUTTON_VALIDATE = "b_validate";

            public const float ASSET_LOAD_WAIT = 0.1f; // EditorWaitForSeconds value.

            // Types that are validated
            public static readonly List<Type> TYPESTOCHECK = new()
            {
                typeof(Texture2D),
                typeof(AudioClip),
                typeof(Mesh),
                //TODO: Add more types as necessary.
            };

            // Dictionary mapping LogLevel enums to specific default Color values
            public static readonly Dictionary<LogLevel, Color> DEFAULT_LOGCOLOURS = new()
            {
                {LogLevel.PASSED, Color.green},
                {LogLevel.INFO, Color.gray},
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
        }
    }
}