using System;
using System.Collections;
using System.Collections.Generic;

using Unity.EditorCoroutines.Editor;

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

using Object = UnityEngine.Object;

using AssetValidator;
using AssetValidator.ValidationMethods;

public class s_AssetValidator : EditorWindow
{
    List<Object> m_selectedAssets = new List<Object>();
    private so_AssetValidationSettings m_settings;

    /// <summary>
    ///
    /// </summary>
    private VisualElement m_selectedAssetsContainer;
    private Label m_selectedAssetName;
    private Label m_selectedAssetType;
    private ObjectField m_assetValidatorSettings;

    private const float ASSET_LOAD_WAIT = 0.1f; // EditorWaitForSeconds value.

    [MenuItem(AssetValidator.Constants.Constants.MENU_ITEM)]
    public static void ShowWindow()
    {
        s_AssetValidator window = GetWindow<s_AssetValidator>();
        window.titleContent = new GUIContent(AssetValidator.Constants.Constants.WINDOW_NAME);
    }

    /// <summary>
    /// TODO: Code Comment
    /// </summary>
    private void OnEnable()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetValidator.Constants.Constants.PATH_UIDOCUMENT);
        VisualElement root = rootVisualElement;
        visualTree.CloneTree(root);

        // Link UXML elements to member variables from Visual tree asset.
        m_selectedAssetsContainer = root.Q<VisualElement>(AssetValidator.Constants.Constants.VE_ASSETCONTAINER);
        m_selectedAssetName = root.Q<Label>(AssetValidator.Constants.Constants.LABEL_ASSET_NAME);
        m_selectedAssetType = root.Q<Label>(AssetValidator.Constants.Constants.LABEL_ASSET_TYPE);
        m_assetValidatorSettings = root.Q<ObjectField>(AssetValidator.Constants.Constants.OBJECT_SETTINGS);

        // Event for getting new object
        m_assetValidatorSettings.RegisterValueChangedCallback(evt => OnObjectFieldValueChanged(evt.newValue as so_AssetValidationSettings));

        Button validateButton = root.Q<Button>(AssetValidator.Constants.Constants.BUTTON_VALIDATE);
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
            if (ValidateObject.IsValidAssetType(asset))
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

        foreach (Object asset in m_selectedAssets)
        {
            bool result = ValidateGeneral.IsFileSizeValid(asset, m_settings._sizeUnit, m_settings._fileSize);

            Debug.Log(result ? "Less than file max" : "More than file max");

            if (asset is Texture2D texture)
            {
                ValidateTexture2D.IsTexturePowerOfTwo(texture);
                // TODO: ADD ADDITIONAL CHECKS HERE (e.g. Size on disk, references, etc.)
            }
        }
    }

    private void OnObjectFieldValueChanged(so_AssetValidationSettings settings) =>
        m_settings = settings;


    private void LinkUXML()
    {

    }
}