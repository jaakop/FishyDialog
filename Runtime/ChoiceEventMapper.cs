using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Dialog
{
    public class ChoiceEventMapper : MonoBehaviour
    {
        [SerializeField] private Mapping[] mappings;

        void Awake()
        {
            foreach (var map in mappings)
            {
                map.choice.callback = map.callback;
            }
        }

        [Serializable]
        private struct Mapping
        {
            public Choice choice;
            public UnityEvent callback;
        }
    }

}