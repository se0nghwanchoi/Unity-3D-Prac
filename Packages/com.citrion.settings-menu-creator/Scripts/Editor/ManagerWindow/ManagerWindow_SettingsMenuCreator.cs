using CitrioN.Common.Editor;
using UnityEditor;

namespace CitrioN.SettingsMenuCreator.Editor
{
  public class ManagerWindow_SettingsMenuCreator : ManagerWindow
  {
    protected const string TITLE = "Settings Menu Creator";

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Manager")]
    public static ManagerWindow ShowWindow_SettingsMenuCreator()
    {
      return ManagerWindow.ShowWindow<ManagerWindow_SettingsMenuCreator>(TITLE);
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Hub/Welcome")]
    public static ManagerWindow ShowManagerTab_Welcome()
    {
      return ShowWindow<ManagerWindow_SettingsMenuCreator>(TITLE, "Welcome");
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Hub/Resources Generator")]
    public static ManagerWindow ShowManagerTab_ResourcesGenerator()
    {
      return ShowWindow<ManagerWindow_SettingsMenuCreator>(TITLE, "Resources Generator");
    }

    [MenuItem("Tools/CitrioN/Settings Menu Creator/Hub/Samples")]
    public static ManagerWindow ShowManagerTab_Samples()
    {
      return ShowWindow<ManagerWindow_SettingsMenuCreator>(TITLE, "Samples");
    }
  }
}