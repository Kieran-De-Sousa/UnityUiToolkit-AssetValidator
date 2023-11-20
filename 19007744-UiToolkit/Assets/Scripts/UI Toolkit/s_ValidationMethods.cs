using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public class s_ValidationMethods
{
    // Types that are validated.
    private static List<Type> typesToCheck = new List<Type>
    {
        typeof(Texture2D),
        typeof(AudioClip),
        typeof(Mesh),
        //TODO: Add more types as necessary
    };
    
    /// <summary>
    /// Checks if the provided asset's type is contained within the list of types to check.
    /// </summary>
    /// <param name="asset">Asset to be validated.</param>
    /// <returns>
    /// <c>true</c> if the asset's type is in the list of types to check; otherwise, <c>false</c>.
    /// </returns>
    /// <seealso cref="typesToCheck"/>
    public static bool IsValidAssetType(Object asset)
    {
        // Check if the asset's type is in the list
        if (typesToCheck.Contains(asset.GetType()))
        {
            Debug.Log("Asset: "  + asset.name + "\n" +
                      asset.GetType() + ": Asset type is in the list of types to check.");
            return true;
        }
        Debug.LogError("Asset: " + asset.name + "\n" +
                       asset.GetType() + ": Asset type is not in the list of types to check.");
        return false;
    }

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
