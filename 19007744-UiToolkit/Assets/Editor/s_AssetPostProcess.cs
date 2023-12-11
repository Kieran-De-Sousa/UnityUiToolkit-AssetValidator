using UnityEditor;
using UnityEngine;

public class s_AssetPostProcess : AssetPostprocessor
{
    // This method will be called whenever assets are imported into the project
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            // Load the imported asset
            Object importedAsset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

            if (AssetValidator.ValidationMethods.ValidateObject.IsValidAssetType(importedAsset))
            {
                // Select the imported asset in the Editor
                Selection.activeObject = importedAsset;

                // Open Asset Validator window
                s_AssetValidator.ShowWindow();
            }
        }
    }
}
