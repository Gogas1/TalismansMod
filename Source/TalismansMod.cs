using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using NineSolsAPI;
using TalismansMod.UI;
using UnityEngine;

namespace TalismansMod;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class TalismansMod : BaseUnityPlugin {

    private Harmony harmony = null!;

    public static TalismansMod Instance { get; private set; } = null!;
    internal UIController UIController { get; private set; } = null!;
    internal bool IsTalismanBlockingEnabled { get; private set; }

    private ConfigEntry<bool> isTalismanBlockingEnabled = null!;
    

    private void Awake() {
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);
        Instance = this;
        UIController = new UIController();

        isTalismanBlockingEnabled = Config.Bind(
            "Main",
            "Talisman is blocked",
            true,
            "Talisman is blocked");
        isTalismanBlockingEnabled.SettingChanged += (_, _) => {
            IsTalismanBlockingEnabled = isTalismanBlockingEnabled.Value;
        };
        IsTalismanBlockingEnabled = isTalismanBlockingEnabled.Value;

        // Load patches from any class annotated with @HarmonyPatch
        harmony = Harmony.CreateAndPatchAll(typeof(TalismansMod).Assembly);

        
    }

    private void OnDestroy() {
        // Make sure to clean up resources here to support hot reloading

        harmony.UnpatchSelf();
    }
}