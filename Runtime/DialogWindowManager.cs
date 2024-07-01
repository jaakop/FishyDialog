using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dialog
{
    public class DialogWindowManager : MonoBehaviour, IDialogWindowManager
    {
        [SerializeField] protected GameObject box;
        [SerializeField] protected GameObject dialogueBox;
        [SerializeField] protected GameObject choiceBox;

        public virtual void ToggleDialogWindow(bool visible)
        {
            box.SetActive(visible);
        }

        public virtual void ShowDialogBox()
        {
            dialogueBox.SetActive(true);
            choiceBox.SetActive(false);
        }

        public virtual void ShowChoicesBox()
        {
            dialogueBox.SetActive(false);
            choiceBox.SetActive(true);
        }
    }
}