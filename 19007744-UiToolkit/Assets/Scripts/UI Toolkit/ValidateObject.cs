using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace AssetValidator
{
    namespace ValidationMethods
    {
        /// <summary>
        ///
        /// </summary>
        public static class ValidateObject
        {
            /// <summary>
            /// Checks if the provided asset's type is contained within the list of types to check.
            /// </summary>
            /// <param name="asset">Asset to be validated.</param>
            /// <returns>
            /// <c>true</c> if the asset's type is in the list of types to check; otherwise, <c>false</c>.
            /// </returns>
            /// <seealso cref="Constants.Constants.TYPESTOCHECK"/>
            public static bool IsValidAssetType(Object asset)
            {
                // Check if the asset's type is in the list
                if (AssetValidator.Constants.Constants.TYPESTOCHECK.Contains(asset.GetType()))
                {
                    Debug.Log("Asset: "  + asset.name + "\n" +
                              asset.GetType() + ": Asset type is in the list of types to check.");
                    return true;
                }
                Debug.LogError("Asset: " + asset.name + "\n" +
                               asset.GetType() + ": Asset type is not in the list of types to check.");
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static class ValidateGeneral
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="asset"></param>
            /// <param name="sizeUnit"></param>
            /// <param name="maxFileSize"></param>
            /// <returns></returns>
            public static bool IsFileSizeValid(Object asset, AssetValidator.Settings.SizeUnit sizeUnit, long maxFileSize)
            {
                long power = AssetValidator.Constants.Constants.TO_BYTES.GetValueOrDefault(sizeUnit, 1);

                // Convert max file size unit and value assigned in settings to bytes for comparison
                long maxSize = maxFileSize * power;

                long fileSize = GetFileSize(AssetDatabase.GetAssetPath(asset));

                if (fileSize == -1)
                {
                    Debug.LogError($"Asset - {asset} filepath could not be loaded!");
                    return false;
                }

                return fileSize <= maxSize;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="assetPath"></param>
            /// <returns></returns>
            private static long GetFileSize(string assetPath)
            {
                if (!string.IsNullOrEmpty(assetPath))
                {
                    FileInfo fileInfo = new FileInfo(assetPath);
                    if (fileInfo.Exists)
                    {
                        return fileInfo.Length;
                    }
                }

                return -1; // Flag an error if could not retrieve file path
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static class ValidateTexture2D
        {
            /// <summary>
            ///
            /// </summary>
            /// <param name="texture">Texture 2D object to validate.</param>
            /// <returns><c>true</c> if the Texture is a power of two; otherwise, <c>false</c>.</returns>
            public static bool IsTexturePowerOfTwo(Texture2D texture)
            {
                if (!IsPowerOfTwo(texture.width) || !IsPowerOfTwo(texture.height))
                {
                    Debug.LogError(texture.name + " is not a power of two!");
                    return false;
                }

                Debug.Log(texture.name + " is a power of two!");
                return true;
            }

            public static bool IsTextureDimensionsValid(Texture2D texture, Vector2 maxDimensions)
            {

                return false;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="x"></param>
            /// <returns><c>true</c> if the number is a power of two; otherwise, <c>false</c>.</returns>
            private static bool IsPowerOfTwo(int x)
            {
                return (x & (x - 1)) == 0;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static class ValidateAudioClip
        {

        }

        /// <summary>
        ///
        /// </summary>
        public static class ValidateMesh
        {

        }
    }
}
