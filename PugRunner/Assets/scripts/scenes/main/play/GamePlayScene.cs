// Copyright (c) Whatgame Studios 2024 - 2025
// Attach this script to a new empty GameObject called "GameManager" in GamePlayScene.
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PugRunner {

    public class GamePlayScene : MonoBehaviour {
        private const string OBSTACLES_RESOURCE_PATH = "terrain";

        public RectTransform gamePanel;

        private class MovingObstacle {
            public RectTransform rect;
        }

        private RectTransform gamePanelRect;
        private float speed = 400f;
        private float scale = 1.0f;


        private Sprite[] obstacleSprites;
        private readonly List<MovingObstacle> activeObstacles = new List<MovingObstacle>();

        void Start() {
            AuditLog.Log("GamePlay scene");

            obstacleSprites = Resources.LoadAll<Sprite>(OBSTACLES_RESOURCE_PATH);

            if (gamePanel != null)
            {
                gamePanelRect = gamePanel.GetComponent<RectTransform>();
            }

            if (gamePanelRect == null || obstacleSprites == null || obstacleSprites.Length == 0) {
                AuditLog.Log("GamePlayScene: missing GamePanel or obstacle sprites, obstacle spawning disabled");
                return;
            }
        }

        // TODO this moves at the update rate, rather than at a fixed rate for all game players.
        void Update() {
            if (gamePanelRect == null) {
                return;
            }
            moveObstacles();

            if (needAnotherObstacle()) 
            {
                spawnRandomObstacle();
            }
        }

        /**
         * Randomly choose another graphic for the terrain. Locate the new obstacle
         * to the far right of the screen, aligned with the more recent active 
         * obstacle.
         */
        private void spawnRandomObstacle() {
            Sprite sprite = obstacleSprites[Random.Range(0, obstacleSprites.Length)];
            float width = scale * sprite.rect.width;
            float height = scale * sprite.rect.height;

            GameObject obstacleObject = new GameObject(sprite.name, typeof(RectTransform));
            obstacleObject.transform.SetParent(gamePanelRect, false);

            Image image = obstacleObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;

            RectTransform rect = obstacleObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(rightMostEdgeActive() + width / 2f, 0f);

            activeObstacles.Add(new MovingObstacle { rect = rect });
        }

        /**
         * Move all active obstacles to the left at the speed.
         */
        private void moveObstacles() {
            for (int i = activeObstacles.Count - 1; i >= 0; i--) {
                MovingObstacle obstacle = activeObstacles[i];

                obstacle.rect.anchoredPosition += Vector2.left * speed * Time.deltaTime;

                float leftEdge = -obstacle.rect.sizeDelta.x / 2f;
                if (obstacle.rect.anchoredPosition.x < leftEdge) {
                    Destroy(obstacle.rect.gameObject);
                    activeObstacles.RemoveAt(i);
                }
            }
        }


        /**
         * Return true if the right most edge of an obstacle is within the right 
         * border of the game panel.
         */
        private bool needAnotherObstacle() {
            if (activeObstacles.Count == 0) 
            {
                return true;
            }

            float rightActive = rightMostEdgeActive();
            AuditLog.Log($"Right Active: {rightActive}, game width: {gamePanelRect.rect.width}");

            return rightActive < gamePanelRect.rect.width;
        }

        /**
         * Return the X offset of the right side of the last obstacle that has been generated.
         */
        private float rightMostEdgeActive() 
        {
            // If there are no obstacles, then fake that the right most edge is to the left of the game object.
            if (activeObstacles.Count == 0)
            {
                return 0.0f;
            }
            MovingObstacle rightMostObstacle = activeObstacles[activeObstacles.Count - 1];
            return rightMostObstacle.rect.anchoredPosition.x + rightMostObstacle.rect.rect.width / 2.0f;
        }

    }
}
