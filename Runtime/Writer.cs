using System.Collections;
using UnityEngine;

namespace Dialog
{
    public class Writer : MonoBehaviour, IWriter
    {
        [SerializeField] private TMPro.TMP_Text textField;

        [SerializeField] private float typingSpeedInSeconds;

        private bool skip;

        public void SkipLine()
        {
            skip = true;
        }

        public IEnumerator WriteByCharacter(string text)
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