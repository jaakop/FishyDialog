using System;
using System.Collections.Generic;

namespace Dialog
{
    public interface IChoiceRenderer
    {
        /// <summary>
        /// Render an array of choices
        /// </summary>
        /// <param name="choices">Choices</param>
        /// <param name="callback">Callback, which is called when a choice is clicked</param>
        void RenderChoices(IEnumerable<Choice> choices, Action<Choice> callback);
    }
}