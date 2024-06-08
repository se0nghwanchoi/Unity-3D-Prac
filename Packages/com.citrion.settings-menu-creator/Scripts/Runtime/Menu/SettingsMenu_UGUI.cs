using CitrioN.Common;
using CitrioN.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CitrioN.SettingsMenuCreator
{
  [SkipObfuscationRename]
  [AddComponentMenu("CitrioN/Settings Menu Creator/Settings Menu (UGUI)")]
  public class SettingsMenu_UGUI : CanvasUIPanel
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
    [Tooltip("The prefab that should be instantiated and attached when this panel is initialized.")]
    protected GameObject menuTemplate;

    [SerializeField]
    [Tooltip("The SettingsCollection to use")]
    protected SettingsCollection settingsCollection;

    //[SerializeField]
    protected SettingsCollection runtimeSettings;

    protected RectTransform root;

    private List<RectTransform> inputElements = new List<RectTransform>();

    protected Dictionary<RectTransform, SettingHolder> settingObjects = new Dictionary<RectTransform, SettingHolder>();

    protected int previousQualityLevel;

    protected Coroutine qualityLevelCoroutine;

    public GameObject MenuTemplate { get => menuTemplate; set => menuTemplate = value; }

    public SettingsCollection SettingsTemplate { get => settingsCollection; set => settingsCollection = value; }

    public RectTransform Root { get => root; protected set => root = value; }
    public List<RectTransform> InputElements { get => inputElements; set => inputElements = value; }
    public SettingsCollection RuntimeSettings { get => runtimeSettings; set => runtimeSettings = value; }
    public Dictionary<RectTransform, SettingHolder> SettingObjects { get => settingObjects; set => settingObjects = value; }

#if UNITY_EDITOR
    [SerializeField]
    private bool autoRefreshMenu = false;
#endif

#if UNITY_EDITOR
    private int dirtyCount = 0;
    private int currentDirtyCount = 0;
#endif
    #endregion

    protected virtual void OnDestroy()
    {
      UnregisterEvents();
    }

    protected virtual void Update()
    {
#if UNITY_EDITOR
      if (autoRefreshMenu)
      {
        RefreshIfDirtyChanged();
      }
#endif
    }

    protected override void Init()
    {
      CreateAndAttachMenuHierarchy();
      RegisterEvents();
      InitializeSettings();
      if (runtimeSettings != null)
      {
        //runtimeSettings.SaveSettings(isDefault: true);


        // TODO Is this still required because there is a fallback now?
        //if (runtimeSettings.InputElementProviders_UGUI == null)
        //{
        //  ConsoleLogger.LogWarning($"An {nameof(InputElementProviders_UGUI)} reference is " +
        //                           $"required for input elements to be created.");
        //  return;
        //}
      }
      else
      {
        ConsoleLogger.LogWarning($"A {nameof(SettingsCollection)} reference is required for " +
                                 $"a menu to be created.");
        return;
      }
      settingObjects.Clear();
      base.Init();
      root = Canvas?.GetComponent<RectTransform>();
      //Root = RootObject.GetComponent<RectTransform>();
      AddElements(loadOnCreate);
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

    //private void OnUnitySettingChangedWithValue(string settingType, object newValue)
    //{
    //  ConsoleLogger.Log($"Changed Unity Settings: {settingType} ({newValue})", Common.LogType.Debug);

    //  if (settingType == qualityLevelSettingTypeName)
    //  {
    //    UpdateAllSettingFields();
    //  }

    //  //if (runtimeSettings?.Settings != null)
    //  //{
    //  //  foreach (var settingHolder in runtimeSettings.Settings)
    //  //  {
    //  //    var setting = settingHolder.Setting;
    //  //    if (setting != null && setting is QualitySetting qualitySetting)
    //  //    {
    //  //      qualitySetting.unitySetting?.OnAnyUnitySettingChanged(settingType, newValue);
    //  //    }
    //  //  }
    //  //}
    //}

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

    private void CreateAndAttachMenuHierarchy()
    {
      if (menuTemplate != null && Canvas != null)
      {
        var menu = Instantiate(menuTemplate, Canvas.transform);
        // We need to update the local scale
        // because Unity sets it to 0 for some reason
        menu.transform.localScale = Vector3.one;
        //Instantiate(menuTemplate, RootObject.transform);
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

    protected void AddElements(bool loadOnCreate)
    {
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

      // TODO Implement with working save/load mechanic not interfering
      //#if UNITY_EDITOR
      //      if (ApplicationQuitListener.isQuitting && runtimeSettings != null)
      //      {
      //        // Restore the original values from before play mode started
      //        // Mostly relevant for post processing settings because they are
      //        // otherwise not reverted on the profile
      //        //EditorApplication.delayCall += runtimeSettings.RestoreStartValues;
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

    protected void SetupMenuNavigation()
    {
      if (inputElements.Count > 0)
      {
        Selectable firstSelectable = null;

        var selectableElements = inputElements.Where(elem => elem.GetComponentInChildren<Selectable>() != null)
                                              .Select(elem => elem.GetComponentInChildren<Selectable>()).ToList();

        //var elementsCount = inputElements.Count;
        var elementsCount = selectableElements.Count;

        for (int i = elementsCount - 1; i >= 0; i--)
        {
          var selectable = selectableElements[i];
          //var element = inputElements[i];
          //var selectable = element != null ? element.GetComponentInChildren<Selectable>() : null;
          //if (selectable != null)
          {
            firstSelectable = selectable;

            //var previousElement = i > 0 ? inputElements[i - 1] : inputElements[elementsCount - 1];
            //var previousSelectable = previousElement.GetComponentInChildren<Selectable>();
            var previousElement = i > 0 ? selectableElements[i - 1] : selectableElements[elementsCount - 1];
            Selectable previousSelectable = previousElement;

            //var nextElement = i < elementsCount - 1 ? inputElements[i + 1] : inputElements[0];
            //var nextSelectable = nextElement.GetComponentInChildren<Selectable>();
            var nextElement = i < elementsCount - 1 ? selectableElements[i + 1] : selectableElements[0];
            var nextSelectable = nextElement;

            var navigation = selectable.navigation;
            navigation.mode = Navigation.Mode.Explicit;

            //navigation.selectOnLeft = null;// selectable.FindSelectableOnLeft();
            //navigation.selectOnRight = null;// selectable.FindSelectableOnRight();
            navigation.selectOnUp = previousSelectable;
            navigation.selectOnDown = nextSelectable;

            selectable.navigation = navigation;
          }
        }

        if (firstSelectable != null)
        {
          EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
          //break;
        }
      }
    }

    protected void UpdateSettingsFields(bool initialize)
    {
      if (Root != null && runtimeSettings != null)
      {
        if (runtimeSettings.InputElementProviders_UGUI == null)
        {
          ConsoleLogger.LogWarning($"No {nameof(InputElementProviderCollection_UGUI)} reference assigned!");
          return;
        }
        foreach (var s in runtimeSettings.Settings)
        {
          if (s == null) { continue; }
          var elem = s.FindElement_UGUI(Root, runtimeSettings);
          if (elem == null)
          {
            elem = s.CreateElement_UGUI(Root, runtimeSettings);
          }
          if (elem != null)
          {
            InputElements.AddIfNotContains(elem);
            settingObjects.AddOrUpdateDictionaryItem(elem, s);
            s.InitializeElement_UGUI(elem, runtimeSettings, initialize);
            if (initialize)
            {

            }
          }
          else
          {
            ConsoleLogger.LogWarning($"Unable to find or create an input element for setting: " +
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

    // TODO 
    [ContextMenu("Remove Input Elements")]
    protected void RemoveElements()
    {
      for (int i = 0; i < InputElements.Count; i++)
      {
        DestroyImmediate(InputElements[i].gameObject);
      }

      InputElements.Clear();
      settingObjects.Clear();
    }

    [ContextMenu("Refresh Settings")]
    public void RefreshSettings()
    {
      bool load = loadOnCreate && runtimeSettings == null;
      RefreshSettings(load);
    }

    public void RefreshSettings(bool load)
    {
      RemoveElements();
      if (runtimeSettings != null)
      {
        var currentRuntimeSettings = runtimeSettings;
        InitializeSettings();
        runtimeSettings.startValues = currentRuntimeSettings.startValues;
        runtimeSettings.pendingSettingChanges = currentRuntimeSettings.pendingSettingChanges;
        runtimeSettings.activeSettingValues = currentRuntimeSettings.activeSettingValues;
      }
      UpdateSettingsFields(true);
      //AddElements(load);

      // TODO Remove/Refactor why OnPanelOpened is called?!
      OnPanelOpened();
    }

#if UNITY_EDITOR
    private void RefreshIfDirtyChanged()
    {
      if (runtimeSettings == null) { return; }

      dirtyCount = EditorUtility.GetDirtyCount(runtimeSettings);
      if (currentDirtyCount < dirtyCount)
      {
        currentDirtyCount = dirtyCount;
        RefreshSettings();
      }
    }
#endif
  }
}