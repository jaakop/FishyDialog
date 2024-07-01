using System.Collections;
using UnityEngine;

namespace Dialog
{
    public class Writer : MonoBehaviour, IWriter
    {
        [SerializeField] protected TMPro.TMP_Text textField;

        [SerializeField] protected float typingSpeedInSeconds;

        protected bool skip;

        public virtual void SkipLine()
        {
            skip = true;
        }

        public virtual IEnumerator WriteByCharacter(string text)
        {
            skip = false;
            textField.text = "";
            foreach (var letter in text)
            {
                if (skip)
                {
                    textField.text = text;
                    yield return new WaitForSeconds(typingSpeedInSeconds);
                    break;
                }

                textField.text += letter;
                
                if (letter != ' ')
                    yield return new WaitForSeconds(typingSpeedInSeconds);
            }
        }
    }
}