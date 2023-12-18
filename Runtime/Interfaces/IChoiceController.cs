using System.Collections;
using System.Collections.Generic;

namespace Dialog
{
    public interface IChoiceController
    {
        /// <summary>
        /// Choice object, that the user has clicked on
        /// </summary>
        Choice selectedChoice { get; }

        /// <summary>
        /// Ask the user the dialog choices
        /// </summary>
        /// <param name="dialogue">The dialog</param>
        void AskChoices(Dialogue dialogue);

        /// <summary>
        /// Wait for user to click one of the choices
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForSelection();
    }
}