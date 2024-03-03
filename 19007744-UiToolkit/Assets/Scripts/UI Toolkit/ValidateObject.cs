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
        /// General validation methods for ensuring valid object selection.
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
        /// Validation methods for <c>general</c> assets.
        /// </summary>
        public static class ValidateGeneral
        {
            public static so_AssetValidationSettings ValidationGeneral(Object obj, so_AssetValidationSettings settings)
            {
                if (settings._generalFileSizeSettings._uiVisuals._toggle.value)
                {
                    settings._generalFileSizeSettings._result = IsFileSizeValid(obj,
                        settings._generalFileSizeSettings._sizeUnit, settings._generalFileSizeSettings._fileSize);
                }

                return settings;
            }
            /// <summary>
            /// Check if the file size of the asset is within the specified limit.
            /// </summary>
            /// <param name="asset">Asset to be validated.</param>
            /// <param name="sizeUnit">Unit of memory (e.g. KB = Kilobyte).</param>
            /// <param name="maxFileSize">Maximum file size.</param>
            /// <returns>
            /// <c>true</c> if the file size is within the limit; otherwise, <c>false</c>.
            /// </returns>
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
            /// Retrieve the file size of the asset.
            /// </summary>
            /// <param name="assetPath">File path to asset</param>
            /// <returns>
            /// File size of asset if can be found; otherwise, <c>-1</c>.
            /// </returns>
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
        /// Validation methods for <c>Texture2D</c> assets.
        /// </summary>
        public static class ValidateTexture2D
        {
            public static so_AssetValidationSettings ValidationTexture2D(Object obj, so_AssetValidationSettings settings)
            {
                Texture2D texture = (Texture2D) obj;

                if (settings._textureIsPowerOfTwoSettings._uiVisuals._toggle.value)
                {
                    settings._textureIsPowerOfTwoSettings._result = IsTexturePowerOfTwo(texture);
                }

                if (settings._textureSizeSettings._uiVisuals._toggle.value)
                {
                    settings._textureSizeSettings._result = IsTextureDimensionsValid(texture, settings._textureSizeSettings._textureSize);
                }

                return settings;
            }

            /// <summary>
            /// Checks if the dimensions of the Texture2D asset are powers of two.
            /// </summary>
            /// <param name="texture">Texture2D object to validate.</param>
            /// <returns>
            /// <c>true</c> if the Texture is a power of two; otherwise, <c>false</c>.
            /// </returns>
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

            /// <summary>
            /// Checks if both dimensions of the Texture2D asset are below the set maximum dimensions.
            /// </summary>
            /// <param name="texture">Texture2D object to validate.</param>
            /// <param name="maxDimensions">Maximum X and Y dimension size.</param>
            /// <returns>
            /// <c>true</c> if both dimensions of texture are below <c>maxDimensions</c>; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsTextureDimensionsValid(Texture2D texture, Vector2 maxDimensions)
            {
                return texture.width <= maxDimensions.x && texture.height <= maxDimensions.y;
            }

            /// <summary>
            /// Checks if a number is a power of two.
            /// </summary>
            /// <param name="x">Number to be checked.</param>
            /// <returns>
            /// <c>true</c> if the number is a power of two; otherwise, <c>false</c>.
            /// </returns>
            private static bool IsPowerOfTwo(int x)
            {
                return (x & (x - 1)) == 0;
            }
        }

        /// <summary>
        /// Validation methods for <c>AudioClip</c> assets.
        /// </summary>
        public static class ValidateAudioClip
        {
            public static so_AssetValidationSettings ValidationAudioClip(Object obj, so_AssetValidationSettings settings)
            {
                AudioClip audioClip = (AudioClip) obj;

                if (settings._audioLengthSettings._uiVisuals._toggle.value)
                {
                    settings._audioLengthSettings._result = IsAudioBelowLength(audioClip, settings._audioLengthSettings._audioClipLength);
                }

                return settings;
            }

            /// <summary>
            /// Checks if an audio clip is below a specified length.
            /// </summary>
            /// <param name="audioClip">AudioClip object to validate.</param>
            /// <param name="clipLength">Maximum audio clip length in seconds.</param>
            /// <returns><c>true</c> if the audio clip is less than the length; otherwise, <c>false</c></returns>
            public static bool IsAudioBelowLength(AudioClip audioClip, float clipLength)
            {
                return audioClip.length <= clipLength;
            }
        }

        /// <summary>
        /// Validation methods for <c>Mesh</c> assets.
        /// </summary>
        public static class ValidateMesh
        {
            public static so_AssetValidationSettings ValidationMesh(Object obj, so_AssetValidationSettings settings)
            {
                Mesh mesh = (Mesh) obj;

                return settings;
            }
            // TODO: Implementations.
        }
    }
}
