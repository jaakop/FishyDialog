using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Dialog
{
    [RequireComponent(typeof(IChoiceRenderer))]
    public class ChoiceController : MonoBehaviour, IChoiceController
    {
        private IChoiceRenderer choiceRenderer;

        public Choice selectedChoice { get; private set; }

        private void Awake()
        {
            if (!TryGetComponent(out choiceRenderer))
            {
                throw new ArgumentNullException("choiceRenderer",
                    "Couldn't find IChoiceRenderer attached to the game object");
            }
        }

        public void AskChoices(Dialogue dialogue)
        {
            selectedChoice = null;
            choiceRenderer.RenderChoices(dialogue.choices, (choice) =>
            {
                selectedChoice = choice;
                choice.callback.Invoke();
            });
        }

        public IEnumerator WaitForSelection()
        {
            while (!selectedChoice)
            {
                yield return null;
            }
        }
    }

}