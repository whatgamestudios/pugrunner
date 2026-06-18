// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace PugRunner {

    public class MenuScene : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Menu screen");
        }


        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Play")
            {
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"Menu: Unknown button {buttonText}");
                return;
            }
        }
    }
}
