namespace Dialog
{
    public interface IDialogWindowManager
    {
        /// <summary>
        /// Toggle dialog window on and off
        /// </summary>
        /// <param name="visible">On or Off</param>
        void ToggleDialogWindow(bool visible);

        /// <summary>
        /// Shows the dialog box and hides choices
        /// </summary>
        void ShowDialogBox();

        /// <summary>
        /// Shows choices and hides dialog
        /// </summary>
        void ShowChoicesBox();
    }
}