using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class s_AssetValidator : EditorWindow
{
    List<Object> selectedAssets = new List<Object>();
    VisualElement selectedAssetsContainer;

    [MenuItem("Tools/Asset Validator")]
    public static void ShowWindow()
    {
        s_AssetValidator window = GetWindow<s_AssetValidator>();
        window.titleContent = new GUIContent("Asset Validator");
    }

    void OnEnable()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/UI Documents/uxml_AssetValidator.uxml");
        VisualElement root = rootVisualElement;
        visualTree.CloneTree(root);

        selectedAssetsContainer = root.Q<VisualElement>("v_selectedAsset");

        Button validateButton = root.Q<Button>("b_validate");
        validateButton.clicked += ValidateTextures;

        Selection.selectionChanged += UpdateSelectedAssetsList;
    }

    void UpdateSelectedAssetsList()
    {
        selectedAssets.Clear();
        selectedAssetsContainer.Clear();

        foreach (Object asset in Selection.objects)
        {
            if (asset is Texture2D)
            {
                selectedAssets.Add(asset);


                Label assetLabel = new Label(asset.name);
                selectedAssetsContainer.Add(assetLabel);
            }
        }
    }

    void ValidateTextures()
    {
        foreach (Texture2D texture in selectedAssets)
        {
            if (!IsPowerOfTwo(texture.width) || !IsPowerOfTwo(texture.height))
            {
                Debug.LogError(texture.name + " is not a power of two!");
            }
            else
            {
                Debug.Log(texture.name + " is a power of two!");
            }
        }
    }

    bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }
}