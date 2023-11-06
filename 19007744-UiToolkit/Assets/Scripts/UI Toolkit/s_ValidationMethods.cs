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
}
