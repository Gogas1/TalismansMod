using HarmonyLib;
using NineSolsAPI;
using RCGMaker.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRule;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;
using ClipperLibClone;
using System.Collections;
using UnityEngine.SceneManagement;

namespace TalismansMod.UI {
    public class UIController {
        private CurrentTalismanUIContoller talismanUIController;

        public Action? OnModifiersRaisedAnimationDone;

        private GameObject bottomLeftPanel;

        public UIController() {
            bottomLeftPanel = CreateBottomLeftPanelObject();
            talismanUIController = bottomLeftPanel.AddChildrenComponent<CurrentTalismanUIContoller>("TalismanUI");
        }

        private void RestoreUI() {
            bottomLeftPanel = CreateBottomLeftPanelObject();
            talismanUIController = bottomLeftPanel.AddChildrenComponent<CurrentTalismanUIContoller>("TalismanUI");
        }

        public GameObject CreateBottomLeftPanelObject() {
            var canvas = NineSolsAPICore.FullscreenCanvas.transform;
            GameObject parentObject = new GameObject("BossChallenge_BottomLeftPanelUI");
            parentObject.transform.parent = canvas.transform;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            float width = canvasRect.rect.width;
            float height = canvasRect.rect.height;

            float coordsX = width / 13.71f;
            float coordsY = height / 4.15f;

            parentObject.transform.position = new Vector3(coordsX, coordsY);

            return parentObject;
        }      
        
        public void UpdateTalisman(Sprite sprite) {
            talismanUIController.UpdateCurrentTalisman(sprite);
        }

    }
}
