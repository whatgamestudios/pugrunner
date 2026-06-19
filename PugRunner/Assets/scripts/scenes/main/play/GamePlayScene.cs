// Copyright (c) Whatgame Studios 2024 - 2025
// Attach this script to a new empty GameObject called "GameManager" in GamePlayScene.
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PugRunner {

    public class GamePlayScene : MonoBehaviour {
        private const string OBSTACLES_RESOURCE_PATH = "obstacles";
        private const string GAME_PANEL_NAME = "GamePanel";

        public float minSpawnInterval = 0.6f;
        public float maxSpawnInterval = 1.6f;
        public float minSpeed = 400f;
        public float maxSpeed = 900f;
        public float obstacleHeight = 200f;

        private class MovingObstacle {
            public RectTransform rect;
            public float speed;
        }

        private RectTransform gamePanel;
        private Sprite[] obstacleSprites;
        private readonly List<MovingObstacle> activeObstacles = new List<MovingObstacle>();

        void Start() {
            AuditLog.Log("GamePlay scene");

            gamePanel = findGamePanel();
            obstacleSprites = Resources.LoadAll<Sprite>(OBSTACLES_RESOURCE_PATH);

            if (gamePanel == null || obstacleSprites == null || obstacleSprites.Length == 0) {
                AuditLog.Log("GamePlayScene: missing GamePanel or obstacle sprites, obstacle spawning disabled");
                return;
            }

            StartCoroutine(spawnObstaclesLoop());
        }

        void Update() {
            if (gamePanel == null) {
                return;
            }
            moveObstacles();
        }

        private RectTransform findGamePanel() {
            Transform found = transform.Find(GAME_PANEL_NAME);
            if (found != null) {
                return found as RectTransform;
            }

            GameObject panelObject = GameObject.Find(GAME_PANEL_NAME);
            return panelObject != null ? panelObject.GetComponent<RectTransform>() : null;
        }

        private IEnumerator spawnObstaclesLoop() {
            while (true) {
                spawnRandomObstacle();
                yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            }
        }

        private void spawnRandomObstacle() {
            Sprite sprite = obstacleSprites[Random.Range(0, obstacleSprites.Length)];
            float width = obstacleHeight * sprite.rect.width / sprite.rect.height;

            GameObject obstacleObject = new GameObject(sprite.name, typeof(RectTransform));
            obstacleObject.transform.SetParent(gamePanel, false);

            Image image = obstacleObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;

            RectTransform rect = obstacleObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(width, obstacleHeight);
            rect.anchoredPosition = new Vector2(gamePanel.rect.width + width / 2f, 0f);

            activeObstacles.Add(new MovingObstacle { rect = rect, speed = Random.Range(minSpeed, maxSpeed) });
        }

        private void moveObstacles() {
            for (int i = activeObstacles.Count - 1; i >= 0; i--) {
                MovingObstacle obstacle = activeObstacles[i];

                obstacle.rect.anchoredPosition += Vector2.left * obstacle.speed * Time.deltaTime;

                float leftEdge = -obstacle.rect.sizeDelta.x / 2f;
                if (obstacle.rect.anchoredPosition.x < leftEdge) {
                    Destroy(obstacle.rect.gameObject);
                    activeObstacles.RemoveAt(i);
                }
            }
        }
    }
}
