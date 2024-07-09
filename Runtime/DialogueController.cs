using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

        public UnityEvent<Dialogue> OnDialogueStart;
        public UnityEvent<Dialogue> OnDialogueEnd;

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
            OnDialogueStart.Invoke(dialogue);
            StartCoroutine(DialogCoroutine(dialogue));
        }

        [Obsolete("StartDialog method is obsolete. Use StartDialogCoroutine instead")]
        public virtual IEnumerator StartDialog(Dialogue dialogue)
        {
            Debug.LogWarning("StartDialog method is obsolete. Use StartDialogCoroutine instead");
            OnDialogueStart.Invoke(dialogue);
            return DialogCoroutine(dialogue);
        }

        protected virtual IEnumerator DialogCoroutine(Dialogue dialogue)
        {
            windowManager.ToggleDialogWindow(true);

            windowManager.ShowDialogBox();

            foreach (var line in dialogue.lines)
            {
                yield return ShowLineAndWaitForInput(line);
            }

            if (dialogue.choices.Count == 0 || choiceController == null)
            {
                EndDialog(dialogue);
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
                EndDialog(dialogue);
                yield break;
            }
            
            StartCoroutine(DialogCoroutine(selected.responseDialog));
        }

        public virtual void EndDialog(Dialogue dialogue = null)
        {
            StopAllCoroutines();
            windowManager.ToggleDialogWindow(false);
            speakerController?.UpdateSpeaker(null);
            OnDialogueEnd.Invoke(dialogue);
        }

        protected virtual IEnumerator ShowLineAndWaitForInput(Line line)
        {
            speakerController?.UpdateSpeaker(line.speaker);

            yield return writer.WriteByCharacter(line.content);
            yield return input.WaitForInput();
        }
    }
}