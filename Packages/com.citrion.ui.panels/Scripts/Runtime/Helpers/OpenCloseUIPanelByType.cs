using UnityEngine;

namespace CitrioN.UI
{
  /// <summary>
  /// Allows the opening or closing of an <see cref="AbstractUIPanel"/> with the
  /// type of <see cref="panelTypeReference"/> and an optional panel name.
  /// </summary>
  public class OpenCloseUIPanelByType : MonoBehaviour
  {
    [Header("Settings")]
    [SerializeField]
    [Tooltip("Represents the type of the panel and\n" +
         "is NOT necessarily the panel that will be opened.\n" +
         "Implemented like this to allow the referencing of prefabs\n" +
         "to be used to determine the type instead of using\n" +
         "the fully qualified class name")]
    protected AbstractUIPanel panelTypeReference;

    [SerializeField]
    [Tooltip("Name of the panel to open. If specified only a panel\n" +
             "matching the type and name can be opened.")]
    protected string panelName = string.Empty;


    protected AbstractUIPanel GetPanel()
    {
      if (panelTypeReference == null) { return null; }

      AbstractUIPanel panel = null;

      // Check if no panel name was specified
      if (string.IsNullOrEmpty(panelName))
      {
        // Get the first panel of the desired type
        panel = UIPanelManager.GetPanel(panelTypeReference.GetType());
      }
      else
      {
        // Get all panels of the desired type
        var panels = UIPanelManager.GetPanels(panelTypeReference.GetType());
        // Check if no valid panels where found
        if (panels == null || panels.Count < 1) { return null; }
        // Get the first panel with the desired panel name
        panel = panels.Find(p => p.PanelName == panelName);
      }

      return panel;
    }

    public void OpenPanel()
    {
      var panel = GetPanel();
      if (panel != null) { panel.OpenNoParams(); }
    }

    public void ClosePanel()
    {
      var panel = GetPanel();
      if (panel != null) { panel.CloseNoParams(); }
    }
  }
}