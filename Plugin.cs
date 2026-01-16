using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using TMPro;
using UnityEngine.UI;

namespace Cutscenes
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("me.ytarame.Multiplayer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInProcess("Project Arrhythmia.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        

        internal static ConfigEntry<KeyboardShortcut> key;
        internal static ConfigEntry<float> glitchIntensity;
        internal static ConfigEntry<bool> afterRestart;

        internal static TextMeshProUGUI SkipLabel, RewindIcon;
        internal static Toggle toggle;
        internal static TMP_InputField checkpointName;


        internal static bool restarted = false;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            bool en = Multiplayer.Enabled;
            if(en)
                Logger.LogInfo("Multiplayer is enabled");
            
            key = Config.Bind("General", "Key", new KeyboardShortcut(UnityEngine.KeyCode.K), "The key that skips the cutscene when pressed.");
            glitchIntensity = Config.Bind("General", "Glitch", 0.3f, "The intensity of glitch effect on rewinding [0.00-1.00].");
            afterRestart = Config.Bind("General", "AfterRestart", false, "Rewind the cutscenes automatically after first restart.");

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Patcher));
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(EditorPatcher));

            Logger.LogInfo("Plugin is loaded!");
        }
    }
}