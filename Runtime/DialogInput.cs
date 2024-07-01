using System.Collections;
using System.Linq;
using UnityEngine;

namespace Dialog
{
    public class DialogInput : MonoBehaviour, IDialogInput
    {
        [SerializeField] protected KeyCode[] listenedKeys;

        public virtual bool GetInput()
        {
            return listenedKeys.Any(key => Input.GetKeyDown(key)) || Input.GetMouseButtonDown(0);
        }

        public virtual IEnumerator WaitForInput()
        {
            while (true)
            {
                if (GetInput())
                {
                    break;
                }
                yield return null;
            }
        }
    }
}