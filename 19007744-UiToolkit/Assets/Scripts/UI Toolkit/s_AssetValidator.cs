using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class s_AssetValidator : EditorWindow
{
    List<Object> m_selectedAssets = new List<Object>();
    VisualElement m_selectedAssetsContainer;
    VisualElement m_selectedAssetsImage;

    /// <summary>
    /// String constants of UI Toolkit
    /// </summary>
    private const string MENU_ITEM = "Tools/Asset Validator";
    private const string WINDOW_NAME = "Asset Validator";

    private const string PATH_UIDOCUMENT = "Assets/UI/UI Documents/uxml_AssetValidator.uxml";

    private const string VE_ASSETCONTAINER = "v_selectedAsset";
    // const string VE_ASSETPREVIEW = "i_assetPreview";
    private const string BUTTON_VALIDATE = "b_validate";

    [MenuItem(MENU_ITEM)]
    public static void ShowWindow()
    {
        s_AssetValidator window = GetWindow<s_AssetValidator>();
        window.titleContent = new GUIContent(WINDOW_NAME);
    }

    /// <summary>
    /// TODO: Code Comment
    /// </summary>
    private void OnEnable()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PATH_UIDOCUMENT);
        VisualElement root = rootVisualElement;
        visualTree.CloneTree(root);

        m_selectedAssetsContainer = root.Q<VisualElement>(VE_ASSETCONTAINER);
        //m_selectedAssetsImage = m_selectedAssetsContainer.Q<VisualElement>(VE_ASSETPREVIEW);


        Button validateButton = root.Q<Button>(BUTTON_VALIDATE);
        validateButton.clicked += ValidateSelection;

        Selection.selectionChanged += UpdateSelectedAssetsList;
    }

    /// <summary>
    /// TODO: Code Comment
    /// </summary>
    private void UpdateSelectedAssetsList()
    {
        m_selectedAssets.Clear();
        m_selectedAssetsContainer.Clear();

        foreach (Object asset in Selection.objects)
        {
            if (s_ValidationMethods.IsValidAssetType(asset))
            {
                // Display a label of the assets name.
                Label assetLabel = new Label(asset.name);
                m_selectedAssetsContainer.Add(assetLabel);
            
                // Display the selected (or most recently selected) asset's Unity preview image.
                Texture2D texture = AssetPreview.GetAssetPreview(asset);
                m_selectedAssetsContainer.style.backgroundImage = texture;
                
                m_selectedAssets.Add(asset);
            }

            // TODO: OTHER ASSET TYPES
        }
    }

    /// <summary>
    /// TODO: Code Comment
    /// </summary>
    private void ValidateSelection()
    {
        // foreach (Object asset in m_selectedAssets)
        // {
        //     if (asset is Texture2D)
        //     {
        //         
        //     }
        //     
        // }
        
        foreach (Texture2D texture in m_selectedAssets)
        {
            if (!IsPowerOfTwo(texture.width) || !IsPowerOfTwo(texture.height))
            {
                Debug.LogError(texture.name + " is not a power of two!");
            }
            else
            {
                Debug.Log(texture.name + " is a power of two!");
            }

            // TODO: ADD ADDITIONAL CHECKS HERE (e.g. Size on disk, references, etc.)
        }
    }

    private bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }
}