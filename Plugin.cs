using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;

namespace Cutscenes
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Project Arrhythmia.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        

        internal static ConfigEntry<KeyboardShortcut> key;
        internal static ConfigEntry<float> glitchIntensity;

        internal static TextMeshProUGUI SkipLabel, RewindIcon;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            key = Config.Bind("General", "Key", new KeyboardShortcut(UnityEngine.KeyCode.K), "The key that skips the cutscene when pressed.");
            glitchIntensity = Config.Bind("General", "Glitch", 0.3f, "The intensity of glitch effect on rewinding [0.00-1.00].");

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patcher));

            Logger.LogInfo("Plugin is loaded!");
        }
    }
}