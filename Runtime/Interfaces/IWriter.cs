using System.Collections;

namespace Dialog
{
    public interface IWriter
    {
        /// <summary>
        /// Skip writing and show the entire line
        /// </summary>
        void SkipLine();

        /// <summary>
        /// Write a text, line by line
        /// </summary>
        /// <param name="text">Text to write</param>
        /// <returns></returns>
        IEnumerator WriteByCharacter(string text);
    }
}