using UnityEditor;

namespace CitrioN.Common.Editor
{
  public static class ProjectUtilities
  {
    public static void ImportUnityPackageFromFilePath(string path, bool showDialog)
    {
      AssetDatabase.ImportPackage(path, showDialog);
    }

    public static void ImportUnityPackageFromFilePathWithDialog(string path)
    {
      AssetDatabase.ImportPackage(path, interactive: true);
    }

    public static void ImportUnityPackageFromFilePathWithoutDialog(string path)
    {
      AssetDatabase.ImportPackage(path, interactive: false);
    }
  }
}