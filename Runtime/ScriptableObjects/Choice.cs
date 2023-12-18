using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Dialog
{
    [CreateAssetMenu(fileName = "Data", menuName = "Dialog/Choice")]
    public class Choice : ScriptableObject
    {
        public Line line;
        public string displayText;
        public Dialogue responseDialog;

        [HideInInspector]
        public UnityEvent callback;
    }

}