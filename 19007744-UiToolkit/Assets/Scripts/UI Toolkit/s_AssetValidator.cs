using System;
using System.Collections;
using System.Collections.Generic;

using Unity.EditorCoroutines.Editor;

using UnityEngine;
using UnityEditor;

using UnityEngine.UIElements;

using Object = UnityEngine.Object;

public class s_AssetValidator : EditorWindow
{
    List<Object> m_selectedAssets = new List<Object>();

    /// <summary>
    ///
    /// </summary>
    private VisualElement m_selectedAssetsContainer;
    private Label m_selectedAssetName;
    private Label m_selectedAssetType;

    private const float ASSET_LOAD_WAIT = 0.1f; // EditorWaitForSeconds value.

    /// <summary>
    /// String constants of UI Toolkit
    /// </summary>
    private const string MENU_ITEM = "Tools/Asset Validator/Asset Validator";
    private const string WINDOW_NAME = "Asset Validator";

    private const string PATH_UIDOCUMENT = "Assets/UI/UI Documents/uxml_AssetValidator.uxml";

    private const string VE_ASSETCONTAINER = "v_selectedAsset";
    private const string LABEL_ASSET_NAME = "l_selectedAssetName";
    private const string LABEL_ASSET_TYPE = "l_selectedAssetType";
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

        // Link UXML elements to member variables from Visual tree asset.
        m_selectedAssetsContainer = root.Q<VisualElement>(VE_ASSETCONTAINER);
        m_selectedAssetName = root.Q<Label>(LABEL_ASSET_NAME);
        m_selectedAssetType = root.Q<Label>(LABEL_ASSET_TYPE);


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
                // Sets Asset name in tool window.
                SetPreviewLabel(asset);

                // Sets Asset preview GUI image in tool window.
                EditorCoroutineUtility.StartCoroutineOwnerless(SetPreviewImage(asset));

                m_selectedAssets.Add(asset);
            }
        }
    }

    /// <summary>
    /// Sets the preview name of the asset as the preview name in the <c>window</c> container.
    /// </summary>
    /// <param name="asset">Asset to have preview name shown.</param>
    private void SetPreviewLabel(Object asset)
    {
        // Display a label of the assets name.
        m_selectedAssetName.text = $"Asset: {asset.name}";
        m_selectedAssetType.text = $"Type: {asset.GetType()}";
    }


    /// <summary>
    /// Sets the preview image of the asset as the preview image in the <c>window</c> container.
    /// </summary>
    /// <param name="asset">Asset to have preview image shown.</param>
    /// <returns><c>backgroundImage</c> for asset preview.</returns>
    IEnumerator SetPreviewImage(Object asset)
    {
        Texture2D texture = AssetPreview.GetAssetPreview(asset);

        // Check if a valid texture preview is available for the asset
        if (texture != null)
        {
            m_selectedAssetsContainer.style.backgroundImage = texture;
        }
        else
        {
            // If asset preview is not available, wait for ASSET_LOAD_WAIT amount of time before attempting
            // to set the asset preview again. Repeat until successful.
            yield return new EditorWaitForSeconds(ASSET_LOAD_WAIT);
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
            s_ValidationMethods.IsTexturePowerOfTwo(texture);

            // TODO: ADD ADDITIONAL CHECKS HERE (e.g. Size on disk, references, etc.)
        }
    }

    private bool IsPowerOfTwo(int x)
    {
        return (x & (x - 1)) == 0;
    }
}