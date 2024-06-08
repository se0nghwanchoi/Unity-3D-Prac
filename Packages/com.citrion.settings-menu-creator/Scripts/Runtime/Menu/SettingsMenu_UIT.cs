using CitrioN.Common;
using CitrioN.UI.UIToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscationRename]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Settings Menu (UI Toolkit)")]
  public class SettingsMenu_UIT : UIT_Panel
  {
    #region Fields & Properties

    [Header("Settings Menu")]
    [Space(5)]

    [SerializeField]
    [Tooltip("Should the setting values be loaded when the menu elements are added/created?")]
    protected bool loadOnCreate = true;

    [SerializeField]
    [Tooltip("Should all pending changed be applied and saved when the menu is closed?")]
    protected bool applyAndSaveOnClose = true;

    [SerializeField]
    [Tooltip("The SettingsCollection to use")]
    protected SettingsCollection settingsCollection;

    //[SerializeField]
    protected SettingsCollection runtimeSettings;

    protected List<VisualElement> inputElements = new List<VisualElement>();

    protected Dictionary<VisualElement, SettingHolder> settingObjects = new Dictionary<VisualElement, SettingHolder>();

    protected List<VisualElement> selectableInputElements = new List<VisualElement>();

    protected int previousQualityLevel;

    protected Coroutine qualityLevelCoroutine;

    public SettingsCollection SettingsTemplate
    {
      get => settingsCollection;
      set => settingsCollection = value;
    }

    public List<VisualElement> InputElements { get => inputElements; set => inputElements = value; }

    public Dictionary<VisualElement, SettingHolder> SettingObjects
    {
      get => settingObjects;
      set => settingObjects = value;
    }

    #endregion

    protected virtual void OnDestroy()
    {
      UnregisterEvents();
    }

    protected override void Init()
    {
      RegisterEvents();
      InitializeSettings();
      //if (runtimeSettings != null)
      //{
      //  runtimeSettings.SaveSettings(isDefault: true);
      //}
      //else
      //{
      //  ConsoleLogger.LogWarning($"A {nameof(SettingsCollection)} reference is required for " +
      //                           $"a menu to be created.");
      //  return;
      //}
      settingObjects.Clear();
      base.Init();
    }

    protected void RegisterEvents()
    {
      //GlobalEventHandler.AddEventListener<Setting, string, SettingsCollection, object>
      //  (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnAnySettingChanged);
#if UNITY_2022_1_OR_NEWER
      QualitySettings.activeQualityLevelChanged += OnActiveQualityLevelChanged;
#endif
    }

    private void UnregisterEvents()
    {
      //GlobalEventHandler.RemoveEventListener<Setting, string, SettingsCollection, object>
      //  (SettingsMenuVariables.SETTING_VALUE_CHANGED_EVENT_NAME, OnAnySettingChanged);
#if UNITY_2022_1_OR_NEWER
      QualitySettings.activeQualityLevelChanged -= OnActiveQualityLevelChanged;
#endif
    }

    private void OnActiveQualityLevelChanged(int previousLevel, int newLevel)
    {
      UpdateAllSettingFields();
    }

    protected void InitializeSettings()
    {
      if (SettingsTemplate != null)
      {
        runtimeSettings = SettingsTemplate;
        //runtimeSettings = Instantiate(SettingsTemplate);
        runtimeSettings.Initialize();
        runtimeSettings.onSettingUpdated += OnSettingUpdated;
      }
    }

    private void OnSettingUpdated(string settingIdentifier)
    {
      UpdateSettingsField(settingIdentifier);
    }

    private void UpdateSettingsField(string settingIdentifier)
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.UpdateSettingField(Root, settingIdentifier);
    }

    [ContextMenu("Update All Input Elements")]
    private void UpdateAllSettingFields()
    {
      if (runtimeSettings == null || runtimeSettings.Settings == null) { return; }
      runtimeSettings.Settings.ForEach(s => UpdateSettingsField(s.Identifier));
    }

    protected override void AddElements()
    {
      base.AddElements();

      UpdateSettingsFields(true);
      LoadDefaultSettings();
      if (loadOnCreate)
      {
        LoadSettings();
      }
    }

    protected override void OnPanelOpened()
    {
      UpdateSettingsFields(false);

      base.OnPanelOpened();

#if !UNITY_2022_1_OR_NEWER
      if (qualityLevelCoroutine != null)
      {
        StopCoroutine(qualityLevelCoroutine);
      }

      qualityLevelCoroutine = StartCoroutine(DetectQualityLevelChange()); 
#endif

      //SetupMenuNavigation();
    }

    protected override void OnPanelClosed()
    {
      base.OnPanelClosed();

#if !UNITY_2022_1_OR_NEWER
      if (qualityLevelCoroutine != null)
      {
        StopCoroutine(qualityLevelCoroutine);
      }
#endif

      if (applyAndSaveOnClose)
      {
        ApplyPendingSettingChanges();
        SaveSettings();
      }

      //#if UNITY_EDITOR
      //      if (ApplicationQuitListener.isQuitting && runtimeSettings != null)
      //      {
      //        // Restore the original values from before play mode started
      //        // Mostly relevant for post processing settings because they are
      //        // otherwise not reverted on the profile
      //        runtimeSettings.RestoreStartValues();
      //      } 
      //#endif
    }

    protected IEnumerator DetectQualityLevelChange()
    {
      int currentQualityLevel;
      while (true)
      {
        currentQualityLevel = QualitySettings.GetQualityLevel();
        if (currentQualityLevel != previousQualityLevel)
        {
          OnActiveQualityLevelChanged(previousQualityLevel, currentQualityLevel);
          previousQualityLevel = currentQualityLevel;
        }
        yield return null;
      }
    }

    protected void UpdateSettingsFields(bool initialize)
    {
      if (Root != null && runtimeSettings != null)
      {
        if (runtimeSettings.InputElementProviders_UIT == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UIT)} reference assigned!");
          return;
        }
        foreach (var s in runtimeSettings.Settings)
        {
          if (s == null) { continue; }
          var elem = s.FindElement_UIToolkit(Root, runtimeSettings);
          if (elem == null)
          {
            elem = s.CreateElement_UIToolkit(Root, runtimeSettings);
          }
          if (elem != null)
          {
            inputElements.AddIfNotContains(elem);
            settingObjects.AddOrUpdateDictionaryItem(elem, s);
            s.InitializeElement_UIToolkit(elem, runtimeSettings, initialize);
            if (initialize)
            {

            }
          }
          else
          {
            ConsoleLogger.LogWarning($"Unable to find or create an input field for setting: " +
                         $"{s.Setting.RuntimeName.Bold()}", Common.LogType.Always);
          }
        }
      }
    }

    [SkipObfuscationRename]
    [ContextMenu("Apply Settings Changes")]
    public void ApplyPendingSettingChanges()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.ApplyPendingSettingsChanges();
    }

    [SkipObfuscationRename]
    public void PrintCurrentValues()
    {
      foreach (var item in runtimeSettings.Settings)
      {
        item.Setting.GetCurrentValues(runtimeSettings);
      }
    }

    [SkipObfuscationRename]
    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.SaveSettings();
    }

    [SkipObfuscationRename]
    [ContextMenu("Load Default Settings")]
    public void LoadDefaultSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.LoadSettings(isDefault: true, apply: true, forceApply: true);
    }

    [SkipObfuscationRename]
    [ContextMenu("Load Settings")]
    public void LoadSettings()
    {
      if (runtimeSettings == null) { return; }
      runtimeSettings.LoadSettings(isDefault: false, apply: true, forceApply: true);
    }

    [ContextMenu("Remove Input Elements")]
    protected void RemoveElements()
    {
      for (int i = 0; i < inputElements.Count; i++)
      {
        inputElements[i].RemoveFromHierarchy();
      }

      inputElements.Clear();
      settingObjects.Clear();
    }

    //[ContextMenu("Refresh Settings")]
    //protected void RefreshSettings()
    //{
    //  RemoveElements();
    //  InitializeSettings();
    //  AddElements();

    //  // TODO Remove/Refactor why OnPanelOpened is called?!
    //  OnPanelOpened();
    //}

    [ContextMenu("Refresh Settings")]
    public void RefreshSettings()
    {
      RemoveElements();
      bool load = loadOnCreate && runtimeSettings == null;
      if (runtimeSettings != null)
      {
        var currentRuntimeSettings = runtimeSettings;
        InitializeSettings();
        runtimeSettings.startValues = currentRuntimeSettings.startValues;
        runtimeSettings.pendingSettingChanges = currentRuntimeSettings.pendingSettingChanges;
        runtimeSettings.activeSettingValues = currentRuntimeSettings.activeSettingValues;
      }

      #region Add Elements
      base.AddElements();
      UpdateSettingsFields(true);
      LoadDefaultSettings();
      if (load)
      {
        LoadSettings();
      }
      #endregion

      // TODO Remove/Refactor why OnPanelOpened is called?!
      OnPanelOpened();
    }
  }
}
