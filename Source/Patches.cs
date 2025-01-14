using HarmonyLib;
using System;
using UnityEngine;

namespace TalismansMod;

[HarmonyPatch]
public class Patches {
    private static readonly AccessTools.FieldRef<PlayerFooExplodeState, int> ChargingCount = AccessTools.FieldRefAccess<PlayerFooExplodeState, int>("chargingCount");

    private static int AutoStyleCounter = 0;
    private static int ControlStyleCounter = 0;

    [HarmonyPatch(typeof(PlayerFooExplodeState), "ExplodeCharge")]
    [HarmonyPrefix]
    public static void ExplodeChargePatchPrefix(PlayerFooExplodeState __instance) {
        int chargingCount = ChargingCount.Invoke(__instance);
        int actualCount = (float)chargingCount <= Player.i.chiContainer.Value ? chargingCount : (int)Player.i.chiContainer.Value;

        ControlStyleCounter += Math.Max(0, actualCount - 2);
    }

    [HarmonyPatch(typeof(PlayerFooExplodeState), "ExplodeCharge")]
    [HarmonyPrefix]
    public static void ExplodeChargePatchPostfix(PlayerFooExplodeState __instance) {
        if(ControlStyleCounter >= 3 && Player.i.mainAbilities.FooExplodeConsecutiveStyle.IsActivated && Player.i.mainAbilities.FooExplodeConsecutiveStyleUpgrade.IsActivated) {
            for(int i = ControlStyleCounter; i >= 3; i -= 3) {
                foreach (FooDeposit fooDeposit in SingletonBehaviour<FooManager>.Instance.deposits) {
                    Player.i.fooExplodeLootSpawnerAmmoForUltimateSkill.Spawn(fooDeposit.transform.position);
                }
                ControlStyleCounter -= 3;
            }
        }
    }

    [HarmonyPatch(typeof(FooManager), "ExplodeWithDealer")]
    [HarmonyPostfix]
    public static void FooExplodePatch() {
        if (Player.i.mainAbilities.FooExplodeAutoStyle.IsActivated && Player.i.mainAbilities.FooExplodeAutoStyleUpgrade.IsActivated) {
            AutoStyleCounter++;
            if(AutoStyleCounter >= 3) {
                foreach (FooDeposit fooDeposit in SingletonBehaviour<FooManager>.Instance.deposits) {
                    Player.i.fooExplodeLootSpawnerAmmoForUltimateSkill.Spawn(fooDeposit.transform.position);
                }
                AutoStyleCounter -= 3;
            }
        }

        System.Random random = new System.Random();
        string path = "GameCore(Clone)/RCG LifeCycle/UIManager/GameplayUICamera/UI-Canvas/[Tab] MenuTab/CursorProvider/Menu Vertical Layout/Panels/PlayerStatus Panel/Description Provider/LeftPart/PlayerStatusSelectableButton_ControlStyle";
        GameObject talismanSelectorGO = GameObject.Find(path);

        var selectorComp = talismanSelectorGO.GetComponent<CollectionRotateSelectorButton>();
        if (selectorComp != null) {
            var collection = selectorComp.collection;
            int talismansNum = collection.AcquiredCount;
            int variantsNum = random.Next(1, talismansNum);
            for (int i = 0; i < variantsNum; i++) {
                collection.Next();
            }

            selectorComp.UpdateView();
            TalismansMod.Instance.UIController.UpdateTalisman(selectorComp.image.sprite);
        }
    }

    [HarmonyPatch(typeof(CollectionRotateSelectorButton), "SubmitImplementation")]
    [HarmonyPrefix]
    public static bool FooSelectionPatchPrefix(CollectionRotateSelectorButton __instance) {
        if (__instance.gameObject.name == "PlayerStatusSelectableButton_ControlStyle" && TalismansMod.Instance.IsTalismanBlockingEnabled) {
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(CollectionRotateSelectorButton), "SubmitImplementation")]
    [HarmonyPostfix]
    public static void FooSelectionPatchPostfix(CollectionRotateSelectorButton __instance) {
        if (__instance.gameObject.name == "PlayerStatusSelectableButton_ControlStyle") {
            TalismansMod.Instance.UIController.UpdateTalisman(__instance.image.sprite);
        }
    }
}