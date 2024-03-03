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
using AssetValidator.Settings;
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
    private List<Toggle> m_toggleList = new();

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

        so_AssetValidationSettings results = m_settings;
        foreach (Object asset in m_selectedAssets)
        {
            results = AssetValidator.ValidationMethods.ValidateGeneral.ValidationGeneral(asset, results,
                m_toggleValidation);

            foreach (var type in AssetValidator.Constants.Constants.TYPESTOCHECK)
            {
                if (type.IsInstanceOfType(asset) &&
                    AssetValidator.Constants.Constants.TYPEMETHODASSIGNMENT.TryGetValue(type, out var validationMethod))
                {
                    results = validationMethod?.Invoke(asset, results, m_toggleValidation);
                }
            }
        }

        m_settings = results;

        if (m_settings != null)
        {
            foreach (SettingsBase setting in m_settings._settingsList)
            {
                // If the toggle was enabled
                if (setting._uiVisuals._toggle.value)
                {
                    if (setting._result)
                    {
                        setting._uiVisuals._resultBox.style.backgroundColor = m_settings._passed;
                        setting._uiVisuals._resultLabel.style.color = m_settings._passed;
                    }

                    else if (!setting._result)
                    {
                        if (m_settings._logColours.TryGetValue(setting._logLevel, out Color color))
                        {
                            setting._uiVisuals._resultBox.style.backgroundColor = color;
                            setting._uiVisuals._resultLabel.style.color = color;

                        }
                    }
                }
            }
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
    ///
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="isOn"></param>
    private void OnToggleValueChanged(Toggle toggle, bool isOn)
    {
        Label resultLabel = toggle.Q<Label>(AssetValidator.Constants.Constants.LABEL_RESULT);
        VisualElement resultBox = toggle.Q<VisualElement>(AssetValidator.Constants.Constants.VE_RESULT);

        // Change the color of the label when the Toggle is enabled
        if (resultLabel != null)
        {
            resultLabel.style.color = isOn ?
                AssetValidator.Constants.Constants.DEFAULT_TEXT_COLOUR :
                AssetValidator.Constants.Constants.DISABLED_TEXT_COLOUR;
        }

        // Change the color of the VisualElement when the Toggle is enabled
        if (resultBox != null)
        {
            resultBox.style.backgroundColor = isOn ?
                AssetValidator.Constants.Constants.ENABLED_RESULT_BOX_COLOUR :
                // DO ADDITIONAL STYLING HERE
                AssetValidator.Constants.Constants.DISABLED_RESULT_BOX_COLOUR;
        }
    }

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

        // Adds toggles to list and links toggle elements from visual tree asset to toggle fields struct
        m_toggleList.Add(m_toggleValidation._fileSize = root.Q<Toggle>(AssetValidator.Constants.Constants.T_FILESIZE));
        m_toggleList.Add(m_toggleValidation._isPowerOfTwo = root.Q<Toggle>(AssetValidator.Constants.Constants.T_POWEROFTWO));
        m_toggleList.Add(m_toggleValidation._textureDimensions = root.Q<Toggle>(AssetValidator.Constants.Constants.T_TEXTURE_DIMENSIONS));
        m_toggleList.Add(m_toggleValidation._audioExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_AUDIOEXAMPLE));
        m_toggleList.Add(m_toggleValidation._meshExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_MESHEXAMPLE));

        /*m_toggleList.Add(m_settings._settingsList[0]._uiVisuals._toggle = root.Q<Toggle>(AssetValidator.Constants.Constants.T_FILESIZE));
        m_toggleList.Add(m_settings._settingsList[1]._uiVisuals._toggle = root.Q<Toggle>(AssetValidator.Constants.Constants.T_POWEROFTWO));
        m_toggleList.Add(m_settings._settingsList[2]._uiVisuals._toggle = root.Q<Toggle>(AssetValidator.Constants.Constants.T_TEXTURE_DIMENSIONS));
        m_toggleList.Add(m_toggleValidation._audioExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_AUDIOEXAMPLE));
        m_toggleList.Add(m_toggleValidation._meshExample = root.Q<Toggle>(AssetValidator.Constants.Constants.T_MESHEXAMPLE));*/

        m_settings._settingsList[0]._uiVisuals =
            AssignResultsVisualElements(root, AssetValidator.Constants.Constants.T_FILESIZE);
        m_settings._settingsList[1]._uiVisuals =
            AssignResultsVisualElements(root, AssetValidator.Constants.Constants.T_POWEROFTWO);
        m_settings._settingsList[2]._uiVisuals =
            AssignResultsVisualElements(root, AssetValidator.Constants.Constants.T_TEXTURE_DIMENSIONS);

        Debug.Log($"{m_settings._settingsList[0]._uiVisuals._toggle}");
        Debug.Log($"{m_settings._settingsList[0]._uiVisuals._resultBox}");

        m_validateButton = root.Q<Button>(AssetValidator.Constants.Constants.BUTTON_VALIDATE);

    }

    private UIVisuals AssignResultsVisualElements(VisualElement root, string toggleName)
    {
        UIVisuals uiVisuals = default;

        uiVisuals._toggle = root.Q<Toggle>(toggleName);
        uiVisuals._resultLabel = uiVisuals._toggle.Q<Label>(AssetValidator.Constants.Constants.LABEL_RESULT);
        uiVisuals._resultBox = uiVisuals._toggle.Q<VisualElement>(AssetValidator.Constants.Constants.VE_RESULT);

        return uiVisuals;
    }

    /// <summary>
    /// Links UI events and callbacks.
    /// </summary>
    private void LinkEvents()
    {
        // Register event for when Asset Validator scriptable object is assigned to Object field in UI Toolkit.
        m_assetValidatorSettings.RegisterValueChangedCallback(evt => OnObjectFieldValueChanged(evt.newValue as so_AssetValidationSettings));

        /*// Registers every toggle's event callback.
        foreach (Toggle toggle in m_toggleList)
        {
            Toggle thisToggle = toggle;
            toggle.RegisterValueChangedCallback(evt => OnToggleValueChanged(thisToggle, evt.newValue));
        }*/

        // TODO
        // Registers every toggle's event callback.
        foreach (SettingsBase setting in m_settings._settingsList)
        {
            Toggle thisToggle = setting._uiVisuals._toggle;
            setting._uiVisuals._toggle.RegisterValueChangedCallback(evt => OnToggleValueChanged(thisToggle, evt.newValue));
        }

        m_validateButton.clicked += ValidateSelection;

        Selection.selectionChanged += UpdateSelectedAssetsList;
    }

    // NOTE: OLD IMPLEMENTATION!!!

    // /// <summary>
    // /// Checks toggled toggles for general asset validation.
    // /// </summary>
    // /// <param name="asset">Asset to be validated.</param>
    // private void ValidationGeneral(Object asset)
    // {
    //     if (m_toggleValidation._fileSize.value)
    //     {
    //         bool result = ValidateGeneral.IsFileSizeValid(asset, m_settings._generalFileSizeSettings._sizeUnit, m_settings._generalFileSizeSettings._fileSize);
    //         Debug.Log(result ? "Less than file max" : "More than file max");
    //     }
    // }
    //
    // /// <summary>
    // /// Checks toggled toggles for texture asset validation.
    // /// </summary>
    // /// <param name="texture">Texture asset to be validated.</param>
    // private void ValidationTexture2D(Texture2D texture)
    // {
    //     if (m_toggleValidation._isPowerOfTwo.value)
    //     {
    //         ValidateTexture2D.IsTexturePowerOfTwo(texture);
    //     }
    //
    //     if (m_toggleValidation._textureDimensions.value)
    //     {
    //         ValidateTexture2D.IsTextureDimensionsValid(texture, m_settings._textureSizeSettings._textureSize);
    //     }
    // }
    //
    // /// <summary>
    // /// Checks toggled toggles for audio clip asset validation.
    // /// </summary>
    // /// <param name="audioClip">AudioClip asset to be validated.</param>
    // private void ValidationAudioClip(AudioClip audioClip)
    // {
    //     if (m_toggleValidation._audioExample.value)
    //     {
    //         // TODO: Audio validation
    //     }
    // }
    //
    // /// <summary>
    // /// Checks toggled toggles for mesh asset validation.
    // /// </summary>
    // /// <param name="mesh">Mesh asset to be validated.</param>
    // private void ValidationMesh(Mesh mesh)
    // {
    //     if (m_toggleValidation._meshExample.value)
    //     {
    //         // TODO: Mesh validation
    //     }
    // }
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

/// <summary>
///
/// </summary>
public struct UIVisuals
{
    public Toggle _toggle;
    public VisualElement _resultBox;
    public Label _resultLabel;
}