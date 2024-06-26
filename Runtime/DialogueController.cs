using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dialog
{
    [RequireComponent(
        typeof(IDialogWindowManager),
        typeof(IDialogInput),
        typeof(IWriter))]
    public class DialogueController : MonoBehaviour, IDialogueController
    {
        protected IDialogWindowManager windowManager;
        protected IDialogInput input;
        protected IWriter writer;
        protected ISpeakerController speakerController;
        protected IChoiceController choiceController;

        public UnityEvent OnDialogueStart = new();
        public UnityEvent OnDialogueEnd = new();

        protected virtual void Awake()
        {
            if (!TryGetComponent(out windowManager))
            {
                throw new ArgumentNullException("windowManager",
                    "Couldn't find IDialogWindowManager attached to the game object");
            }

            if (!TryGetComponent(out input))
            {
                throw new ArgumentNullException("input",
                    "Couldn't find IDialogWindowManager attached to the game object");
            }

            if (!TryGetComponent(out writer))
            {
                throw new ArgumentNullException("writer",
                    "Couldn't find IWriter attached to the game object");
            }

            speakerController = GetComponentInChildren<ISpeakerController>(true);
            choiceController = GetComponentInChildren<IChoiceController>(true);
        }

        protected virtual void Update()
        {
            if (input.GetInput())
            {
                writer.SkipLine();
            }
        }

        public virtual void StartDialogCoroutine(Dialogue dialogue)
        {
            StartCoroutine(DialogCoroutine(dialogue));
        }

        [Obsolete("StartDialog method is obsolete. Use StartDialogCoroutine instead")]
        public virtual IEnumerator StartDialog(Dialogue dialogue)
        {
            Debug.LogWarning("StartDialog method is obsolete. Use StartDialogCoroutine instead");
            return DialogCoroutine(dialogue);
        }

        protected virtual IEnumerator DialogCoroutine(Dialogue dialogue)
        {
            windowManager.ToggleDialogWindow(true);

            windowManager.ShowDialogBox();

            OnDialogueStart.Invoke();

            foreach (var line in dialogue.lines)
            {
                yield return ShowLineAndWaitForInput(line);
            }

            if (dialogue.choices.Count == 0 || choiceController == null)
            {
                EndDialog();
                yield break;
            }

            windowManager.ShowChoicesBox();

            speakerController?.UpdateSpeaker(null);

            choiceController.AskChoices(dialogue);
            yield return choiceController.WaitForSelection();

            var selected = choiceController.selectedChoice;

            windowManager.ShowDialogBox();

            yield return ShowLineAndWaitForInput(selected.line);

            if (!selected.responseDialog)
            {
                EndDialog();
                yield break;
            }
            
            StartCoroutine(DialogCoroutine(selected.responseDialog));
        }

        public virtual void EndDialog()
        {
            StopAllCoroutines();
            windowManager.ToggleDialogWindow(false);
            speakerController?.UpdateSpeaker(null);
            OnDialogueEnd.Invoke();
        }

        protected virtual IEnumerator ShowLineAndWaitForInput(Line line)
        {
            speakerController?.UpdateSpeaker(line.speaker);

            yield return writer.WriteByCharacter(line.content);
            yield return input.WaitForInput();
        }
    }
}