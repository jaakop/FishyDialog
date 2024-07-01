using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Dialog
{
    [RequireComponent(typeof(IChoiceRenderer))]
    public class ChoiceController : MonoBehaviour, IChoiceController
    {
        protected IChoiceRenderer choiceRenderer;

        public Choice selectedChoice { get; private set; }

        protected virtual void Awake()
        {
            if (!TryGetComponent(out choiceRenderer))
            {
                throw new ArgumentNullException("choiceRenderer",
                    "Couldn't find IChoiceRenderer attached to the game object");
            }
        }

        public virtual void AskChoices(Dialogue dialogue)
        {
            selectedChoice = null;
            choiceRenderer.RenderChoices(dialogue.choices, (choice) =>
            {
                selectedChoice = choice;
                choice.callback.Invoke();
            });
        }

        public virtual IEnumerator WaitForSelection()
        {
            while (!selectedChoice)
            {
                yield return null;
            }
        }
    }

}