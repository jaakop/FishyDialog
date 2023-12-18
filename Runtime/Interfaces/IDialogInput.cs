using System.Collections;

namespace Dialog
{
    public interface IDialogInput
    {
        /// <summary>
        /// Check if any input is pressed
        /// </summary>
        /// <returns>If any input is pressed</returns>
        bool GetInput();

        /// <summary>
        /// Wait until an input is pressed
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitForInput();
    }
}