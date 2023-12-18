using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialog
{
    public class ChoiceRenderer : MonoBehaviour, IChoiceRenderer
    {
        [SerializeField] private Transform choicesHolder;
        [SerializeField] private GameObject buttonPrefab;

        public void RenderChoices(IEnumerable<Choice> choices, Action<Choice> callback)
        {
            foreach (Transform child in choicesHolder)
            {
                Destroy(child.gameObject);
            }
            foreach (Choice choice in choices)
            {
                var button = Instantiate(buttonPrefab, choicesHolder);
                button.GetComponentInChildren<TMP_Text>().text = choice.displayText; 

                button.GetComponent<Button>().onClick.AddListener(() => callback(choice));
            }
        }
    }
}