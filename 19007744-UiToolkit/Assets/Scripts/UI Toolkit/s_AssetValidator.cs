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
        // Instantiate m_settings to avoid Null Reference errors
        m_settings = ScriptableObject.CreateInstance<so_AssetValidationSettings>();

        LinkUXML();
        LinkEvents();

        // Initialise the colours of the visual elements when enabling the validator.
        foreach (SettingsBase setting in m_settings._settingsList)
        {
            UpdateResultsVisuals(setting._uiVisuals._toggle);
        }
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
    /// <returns>
    /// <c>backgroundImage</c> for asset preview.
    /// </returns>
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

        if (m_settings._generalFileSizeSettings._uiVisuals._resultBox == null)
        {
            Debug.LogError("NO VISUAL ELEMENTS SET!");
            return;
        }

        so_AssetValidationSettings results = m_settings;
        foreach (Object asset in m_selectedAssets)
        {
            results = AssetValidator.ValidationMethods.ValidateGeneral.ValidationGeneral(asset, results);

            foreach (var type in AssetValidator.Constants.Constants.TYPESTOCHECK)
            {
                if (type.IsInstanceOfType(asset) &&
                    AssetValidator.Constants.Constants.TYPEMETHODASSIGNMENT.TryGetValue(type, out var validationMethod))
                {
                    results = validationMethod?.Invoke(asset, results);
                }
            }
        }

        m_settings = results;

        // Error check for null
        if (m_settings != null)
        {
            foreach (SettingsBase setting in m_settings._settingsList)
            {
                // If the toggle was enabled...
                if (setting._uiVisuals._toggle.value)
                {
                    switch (setting._result)
                    {
                        // Asset validation passed!
                        case true:
                        {
                            setting._uiVisuals._resultBox.style.backgroundColor = m_settings._passed;
                            setting._uiVisuals._resultLabel.style.color = m_settings._passed;
                            break;
                        }
                        // Asset validation failed!
                        case false:
                        {
                            if (m_settings._logColours.TryGetValue(setting._logLevel, out Color color))
                            {
                                setting._uiVisuals._resultBox.style.backgroundColor = color;
                                setting._uiVisuals._resultLabel.style.color = color;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Bind object field changing event to <c>m_settings</c> member variable.
    /// </summary>
    /// <param name="settings">Scriptable Object <c>Asset Validator Settings</c>.</param>
    /// <seealso cref="m_settings"/>
    private void OnObjectFieldValueChanged(so_AssetValidationSettings settings)
    {
        m_settings = ReassignVisualElements(m_settings, settings);
    }


    /// <summary>
    /// Event handler when a settings toggle value changes.
    /// Will update Visual Elements associated with toggle based on state.
    /// </summary>
    /// <param name="toggle">Settings toggle whose value changed.</param>
    /// <param name="isOn">New value of toggle.</param>
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
    /// Update visual elements (Label and Result Box) associated with toggle based on toggle state.
    /// </summary>
    /// <param name="toggle">Toggle with visual elements to update.</param>
    private void UpdateResultsVisuals(Toggle toggle)
    {
        Label resultLabel = toggle.Q<Label>(AssetValidator.Constants.Constants.LABEL_RESULT);
        VisualElement resultBox = toggle.Q<VisualElement>(AssetValidator.Constants.Constants.VE_RESULT);

        // Change the color of the label when the Toggle is enabled
        if (resultLabel != null)
        {
            resultLabel.style.color = toggle.value ?
                AssetValidator.Constants.Constants.DEFAULT_TEXT_COLOUR :
                AssetValidator.Constants.Constants.DISABLED_TEXT_COLOUR;
        }

        // Change the color of the VisualElement when the Toggle is enabled
        if (resultBox != null)
        {
            resultBox.style.backgroundColor = toggle.value ?
                AssetValidator.Constants.Constants.ENABLED_RESULT_BOX_COLOUR :
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

        // Loops through list of settings available, and assigns Toggle and Visual Elements
        for (int i = 0; i < m_settings._settingsList.Count; ++i)
        {
            // Assigned using static string list declared in AssetValidator.Constants.Constants. Update this list with
            // further strings and the logic should automatically handle this.
            m_settings._settingsList[i]._uiVisuals =
                AssignResultsVisualElements(root, AssetValidator.Constants.Constants.LIST_T_SETTINGS[i]);
        }

        m_validateButton = root.Q<Button>(AssetValidator.Constants.Constants.BUTTON_VALIDATE);

    }

    /// <summary>
    /// Reassigns visual elements from old settings to new settings.
    /// </summary>
    /// <param name="oldSettings">Old settings containing visual elements.</param>
    /// <param name="newSettings">New settings that require visual elements to be set.</param>
    private so_AssetValidationSettings ReassignVisualElements(so_AssetValidationSettings oldSettings, so_AssetValidationSettings newSettings)
    {
        // Make a copy of all the data
        newSettings = oldSettings;

        // Reassign the visual elements as these don't get passed through.
        for (var i = 0; i < newSettings._settingsList.Count; ++i)
        {
            newSettings._settingsList[i]._uiVisuals = oldSettings._settingsList[i]._uiVisuals;
        }

        return newSettings;
    }

    /// <summary>
    /// Assigns visual elements associated with toggle in UI toolkit.
    /// </summary>
    /// <param name="root">Root visual element containing UI elements.</param>
    /// <param name="toggleName">Toggle UI element name.</param>
    /// <returns>
    /// Assigned visual elements from toggle.
    /// </returns>
    private UIVisuals AssignResultsVisualElements(VisualElement root, string toggleName)
    {
        UIVisuals uiVisuals = new UIVisuals();

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

        // Registers every toggle's event callback.
        foreach (SettingsBase setting in m_settings._settingsList)
        {
            Toggle thisToggle = setting._uiVisuals._toggle;
            setting._uiVisuals._toggle.RegisterValueChangedCallback(evt => OnToggleValueChanged(thisToggle, evt.newValue));
        }

        m_validateButton.clicked += ValidateSelection;

        Selection.selectionChanged += UpdateSelectedAssetsList;
    }
}

/// <summary>
/// Visual Elements that are associated together.
/// </summary>
public struct UIVisuals
{
    public Toggle _toggle;
    public VisualElement _resultBox;
    public Label _resultLabel;
}