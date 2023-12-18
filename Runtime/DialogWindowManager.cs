using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialog
{
    public class DialogWindowManager : MonoBehaviour, IDialogWindowManager
    {
        [SerializeField] private GameObject box;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private GameObject choiceBox;

        public void ToggleDialogWindow(bool visible)
        {
            box.SetActive(visible);
        }

        public void ShowDialogBox()
        {
            dialogueBox.SetActive(true);
            choiceBox.SetActive(false);
        }

        public void ShowChoicesBox()
        {
            dialogueBox.SetActive(false);
            choiceBox.SetActive(true);
        }
    }
}