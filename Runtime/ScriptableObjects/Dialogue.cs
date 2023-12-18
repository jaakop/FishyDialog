using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dialog
{

    [CreateAssetMenu(fileName = "Data", menuName = "Dialog/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public List<Line> lines;
        public List<Choice> choices;
    }

    [Serializable]
    public class Line
    {
        public Actor speaker;
        public string content;
    } 
}
