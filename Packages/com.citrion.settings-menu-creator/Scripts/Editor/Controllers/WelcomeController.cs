using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CitrioN.Common.Editor
{
  [CreateAssetMenu(fileName = "WelcomeController_",
                   menuName = "CitrioN/Common/ScriptableObjects/VisualTreeAsset/Controller/Welcome")]
  public class WelcomeController : ScriptableVisualTreeAssetController
  {
    private const string DISCORD_LINK_LABEL_CLASS = "label__discord-link";
    private const string DOCUMENTATION_LINK_LABEL_CLASS = "label__documentation-link";

    public override void Setup(VisualElement root)
    {
      var documentationLinkLabel = root.Q<Label>(className: DOCUMENTATION_LINK_LABEL_CLASS);
      if (documentationLinkLabel != null)
      {
        documentationLinkLabel.UnregisterCallback<ClickEvent>(OnDocumentationLinkClicked);
        documentationLinkLabel.RegisterCallback<ClickEvent>(OnDocumentationLinkClicked);
      }

      var discordLinkLabel = root.Q<Label>(className: DISCORD_LINK_LABEL_CLASS);
      if (discordLinkLabel != null)
      {
        discordLinkLabel.UnregisterCallback<ClickEvent>(OnDiscordLinkClicked);
        discordLinkLabel.RegisterCallback<ClickEvent>(OnDiscordLinkClicked);
      }
    }

    private void OnDocumentationLinkClicked(ClickEvent evt)
    {
      string path = "Packages/com.citrion.settings-menu-creator/Documentation";
      EditorUtilities.PingObjectAtPath(path);
    }

    private void OnDiscordLinkClicked(ClickEvent evt)
    {
      Application.OpenURL("https://discord.gg/3Cx5SB8pNR");
    }
  }
}