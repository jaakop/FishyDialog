using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialog
{
    [CreateAssetMenu(fileName = "Data", menuName = "Dialog/Actor")]
    public class Actor : ScriptableObject
    {
        public Sprite portrait;
        public new string name;
    }

}