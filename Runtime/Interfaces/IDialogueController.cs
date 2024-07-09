using System;
using System.Collections;

namespace Dialog
{
    public interface IDialogueController
    {
        /// <summary>
        /// Start a dialogue
        /// </summary>
        /// <param name="dialogue">Dialogue to start</param>
        void StartDialogCoroutine(Dialogue dialogue);

        /// <summary>
        /// Start a dialogue
        /// </summary>
        /// <param name="dialogue">Dialogue to start</param>
        /// <returns></returns>
        [Obsolete("StartDialog method is obsolete. Use StartDialogCoroutine instead")]
        IEnumerator StartDialog(Dialogue dialogue);

        /// <summary>
        /// Ends any dialog currently on going
        /// </summary>
        /// <param name="dialogue">Dialogue that ended, null if unknown</param>
        void EndDialog(Dialogue dialogue = null);
    }
}