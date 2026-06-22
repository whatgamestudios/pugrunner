// Copyright (c) Whatgame Studios 2026
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PugRunner {

    public class RunnerControl : MonoBehaviour {
        private const string RUNNER_RESOURCE_PATH = "runner";

        private const long TIME_TO_CHANGE = 300;

        public RectTransform gamePanel;

        private RectTransform gamePanelRect;

        private Sprite[] runnerSprites;
        int activeRunnerSprite;

        private readonly List<RectTransform> runnerObjects = new List<RectTransform>();

        private float scale = 1.0f;

        private DateTime timeOfLastChange;

        void Start() {
            AuditLog.Log("RunnerControl");

            runnerSprites = Resources.LoadAll<Sprite>(RUNNER_RESOURCE_PATH);
            activeRunnerSprite = 0;

            if (gamePanel != null)
            {
                gamePanelRect = gamePanel.GetComponent<RectTransform>();
            }

            if (gamePanelRect == null || runnerSprites == null || runnerSprites.Length == 0) {
                AuditLog.Log("RunnerControl: missing GamePanel or runner sprites");
                return;
            }

            createRunnerObjects();
        }

        // TODO this moves at the update rate, rather than at a fixed rate for all game players.
        void Update() {
            if (gamePanelRect == null) {
                return;
            }

            DateTime now = DateTime.Now;
            if ((now - timeOfLastChange).TotalMilliseconds > TIME_TO_CHANGE) {
                timeOfLastChange = now;

                // Switch which sprit is active.
                runnerObjects[activeRunnerSprite].gameObject.SetActive(false);
                activeRunnerSprite = (activeRunnerSprite + 1) % runnerObjects.Count;
                runnerObjects[activeRunnerSprite].gameObject.SetActive(true);
            }


        }

        private void createRunnerObjects() {
            for (int i = 0; i < runnerSprites.Length; i++)
            {
                Sprite sprite = runnerSprites[i];
                float width = scale * sprite.rect.width;
                float height = scale * sprite.rect.height;

                GameObject runnerObject = new GameObject(sprite.name, typeof(RectTransform));
                runnerObject.transform.SetParent(gamePanelRect, false);

                Image image = runnerObject.AddComponent<Image>();
                image.sprite = sprite;
                image.preserveAspect = true;

                RectTransform rect = runnerObject.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.sizeDelta = new Vector2(width, height);
                rect.anchoredPosition = new Vector2(gamePanelRect.rect.width / 2f, gamePanelRect.rect.height / 2f);

                rect.gameObject.SetActive(false);

                runnerObjects.Add( rect );
            }
        }
    }
}
