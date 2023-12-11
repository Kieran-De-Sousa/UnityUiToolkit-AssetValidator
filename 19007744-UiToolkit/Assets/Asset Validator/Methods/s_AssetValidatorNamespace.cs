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
            /// String constants of UI Toolkit
            /// </summary>
            public const string MENU_ITEM = "Tools/Asset Validator/Asset Validator";
            public const string WINDOW_NAME = "Asset Validator";

            public const string PATH_UIDOCUMENT = "Assets/UI/UI Documents/uxml_AssetValidator.uxml";

            public const string VE_ASSETCONTAINER = "v_selectedAsset";
            public const string LABEL_ASSET_NAME = "l_selectedAssetName";
            public const string LABEL_ASSET_TYPE = "l_selectedAssetType";
            public const string OBJECT_SETTINGS = "o_settings";
            public const string BUTTON_VALIDATE = "b_validate";

            // Types that are validated
            public static List<Type> TYPESTOCHECK = new List<Type>
            {
                typeof(Texture2D),
                typeof(AudioClip),
                typeof(Mesh),
                //TODO: Add more types as necessary.
            };

            public static readonly Dictionary<LogLevel, Color> DEFAULT_LOGCOLOURS = new Dictionary<LogLevel, Color>
            {
                {LogLevel.INFO, Color.gray},
                {LogLevel.PASSED, Color.green},
                {LogLevel.WARNING, Color.yellow},
                {LogLevel.CRITICAL, Color.red}
            };

            public static readonly Dictionary<SizeUnit, long> TO_BYTES =
                new Dictionary<SizeUnit, long>
                {
                    {SizeUnit.Byte, 1},
                    {SizeUnit.Kilobyte, 1000},
                    {SizeUnit.Megabyte, 1000000},
                    {SizeUnit.Gigabyte, 1000000000},
                    {SizeUnit.Terabyte, 1000000000000},
                    // NOTE: Add more values as necessary.
                };
        }
    }
}