// Base
using System;
using System.Collections;
using System.Collections.Generic;

// Unity
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Unity.EditorCoroutines.Editor;

using Object = UnityEngine.Object;

// Asset Validator namespace
using AssetValidator;
using AssetValidator.ValidationMethods;

public class s_AssetValidator : EditorWindow
{
    List<Object> m_selectedAssets = new();
    private so_AssetValidationSettings m_settings;

    /// <summary>
    /// UXML Elements
    /// </summary>
    private VisualElement m_selectedAssetsContainer;
    private Label m_selectedAssetName;
    private Label m_selectedAssetType;
    private Button m_validateButton;
    private ObjectField m_assetValidatorSettings;

    private ToggleFields m_toggleValidation;

    [MenuItem(AssetValidator.Constants.Constants.MENU_ITEM)]
    public static void ShowWindow()
    {
        s_AssetValidator window = GetWindow<s_AssetValidator>();
        window.titleContent = new GUIContent(AssetValidator.Constants.Constants.WINDOW_NAME);
    }

    /// <summary>
    /// Called when <c>Asset Validator</c> window is opened.
    /// </summary>
    private void OnEnable()
    {
        LinkUXML();
        LinkEvents();
    }

    /// <summary>
    /// Updates the list of selected assets depending on selection in Unity window.
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
            yield return new EditorWaitForSeconds(AssetValidator.Constants.Constants.ASSET_LOAD_WAIT);
        }
    }

    /// <summary>
    /// Validates selected objects based on <c>Asset Validator Settings</c> and toggles.
    /// </summary>
    private void ValidateSelection()
    {
        if (m_settings == null)
        {
            Debug.LogError("NO ASSET VALIDATOR SETTINGS HAVE BEEN SET!");
            return;
        }

        foreach (Object asset in m_selectedAssets)
        {
            // General / ALL type checks
            ValidationGeneral(asset);

            // Texture2D type checks
            if (asset is Texture2D texture)
            {
                ValidationTexture2D(texture);
            }

            // AudioClip type checks
            if (asset is AudioClip audioClip)
            {
                ValidationAudioClip(audioClip);
            }

            // Mesh type checks
            if (asset is Mesh mesh)
            {
                ValidationMesh(mesh);
            }
        }
    }

    /// <summary>
    /// Checks toggled toggles for general asset validation.
    /// </summary>
    /// <param name="asset">Asset to be validated.</param>
    private void ValidationGeneral(Object asset)
    {
        if (m_toggleValidation._fileSize.value)
        {
            bool result = ValidateGeneral.IsFileSizeValid(asset, m_settings._fileSizeSettings._sizeUnit, m_settings._fileSizeSettings._fileSize);
            Debug.Log(result ? "Less than file max" : "More than file max");
        }
    }

    /// <summary>
    /// Checks toggled toggles for texture asset validation.
    /// </summary>
    /// <param name="texture">Texture asset to be validated.</param>
    private void ValidationTexture2D(Texture2D texture)
    {
        if (m_toggleValidation._isPowerOfTwo.value)
        {
            ValidateTexture2D.IsTexturePowerOfTwo(texture);
        }

        if (m_toggleValidation._textureDimensions.value)
        {
            ValidateTexture2D.IsTextureDimensionsValid(texture, m_settings._textureSizeSettings._textureSize);
        }
    }

    /// <summary>
    /// Checks toggled toggles for audio clip asset validation.
    /// </summary>
    /// <param name="audioClip">AudioClip asset to be validated.</param>
    private void ValidationAudioClip(AudioClip audioClip)
    {
        if (m_toggleValidation._audioExample.value)
        {
            // TODO: Audio validation
        }
    }

    /// <summary>
    /// Checks toggled toggles for mesh asset validation.
    /// </summary>
    /// <param name="mesh">Mesh asset to be validated.</param>
    private void ValidationMesh(Mesh mesh)
    {
        if (m_toggleValidation._meshExample.value)
        {
            // TODO: Mesh validation
        }
    }

    /// <summary>
    /// Bind object field changing event to <c>m_settings</c> member variable.
    /// </summary>
    /// <param name="settings">Scriptable Object <c>Asset Validator Settings</c></param>
    /// <seealso cref="m_settings"/>
    private void OnObjectFieldValueChanged(so_AssetValidationSettings settings) =>
        m_settings = settings;


    /// <summary>
    /// Links UXML elements to member variables / structs.
    /// </summary>
    private void LinkUXML()
    {
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetValidator.Constants.Constants.PATH_UIDOCUMENT);
        VisualElement root = rootVisualElement;
        visualTree.CloneTree(root);

        // Link UXML elements to member variables from visual tree asset.
        m_selectedAssetsContainer = root.Q<VisualElement>(AssetValidator.Constants.Constants.VE_ASSETCONTAINER);
        m_selectedAssetName = root.Q<Label>(AssetValidator.Constants.Constants.LABEL_ASSET_NAME);
        m_selectedAssetType = root.Q<Label>(AssetValidator.Constants.Constants.LABEL_ASSET_TYPE);

        m_assetValidatorSettings = root.Q<ObjectField>(AssetValidator.Constants.Constants.OBJECT_SETTINGS);

        // Links toggle elements from visual tree asset to toggle fields struct
        m_toggleValidation._fileSize = root.Q<Toggle>(AssetValidator.Constants.Constants.T_FILESIZE);
        m_toggleValidation._isPowerOfTwo = root.Q<Toggle>(AssetValidator.Constants.Constants.T_POWEROFTWO);
        m_toggleValidation._textureDimensions = root.Q<Toggle>(AssetValidator.Constants.Constants.T_TEXTURE_DIMENSIONS);
        m_toggleValidation._audioExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_AUDIOEXAMPLE);
        m_toggleValidation._meshExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_MESHEXAMPLE);

        m_validateButton = root.Q<Button>(AssetValidator.Constants.Constants.BUTTON_VALIDATE);

    }

    /// <summary>
    /// Links UI events and callbacks.
    /// </summary>
    private void LinkEvents()
    {
        // Register event for when Asset Validator scriptable object is assigned to Object field in UI Toolkit.
        m_assetValidatorSettings.RegisterValueChangedCallback(evt => OnObjectFieldValueChanged(evt.newValue as so_AssetValidationSettings));

        m_validateButton.clicked += ValidateSelection;

        Selection.selectionChanged += UpdateSelectedAssetsList;
    }
}

/// <summary>
/// Data struct to group toggle instances
/// </summary>
public struct ToggleFields
{
    [Header("General Toggles")]
    public Toggle _fileSize;

    [Header("Texture2D Toggles")]
    public Toggle _isPowerOfTwo;
    public Toggle _textureDimensions;

    [Header("AudioClip Toggles")]
    public Toggle _audioExample;

    [Header("Mesh Toggles")]
    public Toggle _meshExample;
}