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

                if (settings._generalSuffixSettings._uiVisuals._toggle.value)
                {
                    settings._generalSuffixSettings._result = IsPrePostFixValid(obj,
                        settings._generalSuffixSettings._suffix, settings._generalSuffixSettings._suffixString);
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
            /// Validates if a file name has a prefix or postfix that matches the provided string.
            /// </summary>
            /// <param name="asset">Asset to be validated.</param>
            /// <param name="suffixType">The type of suffix to validate (Prefix or Postfix).</param>
            /// <param name="suffixString">The string to match as a suffix.</param>
            /// <returns>
            /// <c>true</c> if the asset name has a matching prefix or postfix; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsPrePostFixValid(Object asset, AssetValidator.Settings.Suffix suffixType,
                string suffixString)
            {
                string assetName = asset.name;

                switch (suffixType)
                {
                    case AssetValidator.Settings.Suffix.Prefix:
                    {
                        return assetName.StartsWith(suffixString);
                    }
                    case AssetValidator.Settings.Suffix.Postfix:
                    {
                        return assetName.EndsWith(suffixString);
                    }
                    case AssetValidator.Settings.Suffix.None:
                    {
                        return false;
                    }
                    default:
                        return false;
                }
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
                return IsPowerOfTwo(texture.width) && IsPowerOfTwo(texture.height);
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

                if (settings._audioSampleRateSettings._uiVisuals._toggle.value)
                {
                    settings._audioSampleRateSettings._result = IsValidSampleRate(audioClip, settings._audioSampleRateSettings._sampleRates);
                }

                if (settings._audioBitRateSettings._uiVisuals._toggle.value)
                {
                    settings._audioBitRateSettings._result = IsAudioQualityValid(audioClip, settings._audioBitRateSettings._minBitrate);
                }

                return settings;
            }

            /// <summary>
            /// Checks if an audio clip is below a specified length.
            /// </summary>
            /// <param name="audioClip">AudioClip object to validate.</param>
            /// <param name="clipLength">Maximum audio clip length in seconds.</param>
            /// <returns>
            /// <c>true</c> if the audio clip is less than the length; otherwise, <c>false</c>
            /// </returns>
            public static bool IsAudioBelowLength(AudioClip audioClip, float clipLength)
            {
                return audioClip.length <= clipLength;
            }

            /// <summary>
            /// Checks if the sample rate of the audio clip is valid.
            /// </summary>
            /// <param name="audioClip">AudioClip object to validate.</param>
            /// <param name="validSampleRates"><c>Int</c> array of valid sample rates.</param>
            /// <returns>
            /// <c>true</c> if the sample rate of the audio clip is in the list of valid sample rates
            /// </returns>
            public static bool IsValidSampleRate(AudioClip audioClip, int[] validSampleRates)
            {
                // Get the sample rate of the audio clip
                var sampleRate = audioClip.frequency;

                // Check if the sample rate is in the list of valid sample rates
                foreach (var rate in validSampleRates)
                {
                    if (sampleRate == rate)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Checks if the audio clip meets certain quality standards.
            /// </summary>
            /// <param name="audioClip">AudioClip object to validate.</param>
            /// <param name="minBitrate">The minimum bitrate for the AudioClip.</param>
            /// <returns>
            /// <c>True</c> if the audio clip is above the minimum bitrate; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsAudioQualityValid(AudioClip audioClip, int minBitrate)
            {
                // Get the bitrate of the audio clip.
                int bitrate = audioClip.loadType == AudioClipLoadType.DecompressOnLoad ?
                    (int)(audioClip.frequency * audioClip.channels * (audioClip.length / 8)) : audioClip.samples * audioClip.channels;

                return bitrate >= minBitrate;
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

                if (settings._meshVertexCountSettings._uiVisuals._toggle.value)
                {
                    settings._meshVertexCountSettings._result =
                        IsValidVertexCount(mesh, settings._meshVertexCountSettings._maxVertexCount);
                }

                if (settings._meshNormalsSettings._uiVisuals._toggle.value)
                {
                    settings._meshNormalsSettings._result = IsNormalsValid(mesh);
                }

                return settings;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="maxVertices"></param>
            /// <returns></returns>
            public static bool IsValidVertexCount(Mesh mesh, int maxVertices)
            {
                return mesh.vertexCount <= maxVertices;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="mesh"></param>
            /// <returns></returns>
            public static bool IsNormalsValid(Mesh mesh)
            {
                Vector3[] normals = mesh.normals;

                // Check if any normals are zero - Return false if failed.
                foreach (var normal in normals)
                {
                    if (normal == Vector3.zero)
                    {
                        return false;
                    }
                }

                // Check if the normals are smoothly interpolated
                for (var i = 0; i < mesh.subMeshCount; i++)
                {
                    var triangles = mesh.GetTriangles(i);
                    for (var j = 0; j < triangles.Length; j += 3)
                    {
                        Vector3 v0 = mesh.vertices[triangles[j]];
                        Vector3 v1 = mesh.vertices[triangles[j + 1]];
                        Vector3 v2 = mesh.vertices[triangles[j + 2]];

                        Vector3 faceNormal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

                        // Check if any of the mesh normals are not smoothly interpolated.
                        if (Vector3.Dot(faceNormal, normals[triangles[j]]) < 0 ||
                            Vector3.Dot(faceNormal, normals[triangles[j + 1]]) < 0 ||
                            Vector3.Dot(faceNormal, normals[triangles[j + 2]]) < 0)
                        {
                            return false;
                        }
                    }
                }

                // Mesh is oriented and smooth.
                return true;
            }
        }
    }
}
