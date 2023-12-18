using System;
using System.Collections;
using UnityEngine;
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
        private IDialogWindowManager windowManager;
        private IDialogInput input;
        private IWriter writer;
        private ISpeakerController speakerController;
        private IChoiceController choiceController;

        private void Awake()
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

        void Update()
        {
            if (input.GetInput())
            {
                writer.SkipLine();
            }
        }

        public IEnumerator StartDialog(Dialogue dialogue)
        {
            windowManager.ToggleDialogWindow(true);

            windowManager.ShowDialogBox();

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
            
            StartCoroutine(StartDialog(selected.responseDialog));
        }

        public void EndDialog()
        {
            StopAllCoroutines();
            windowManager.ToggleDialogWindow(false);
            speakerController?.UpdateSpeaker(null);
        }

        private IEnumerator ShowLineAndWaitForInput(Line line)
        {
            speakerController?.UpdateSpeaker(line.speaker);

            yield return writer.WriteByCharacter(line.content);
            yield return input.WaitForInput();
        }
    }
}