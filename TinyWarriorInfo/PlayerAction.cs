using System;

namespace TinyWarriorInfo
{
        [Serializable]
        public class PlayerAction
        {
                public int PlayerIndex { get; set; }
                public float DirectionX { get; set; }
                public float DirectionY { get; set; }
                public float LocationX { get; set; }
                public float LocationY { get; set; }
                public bool IsMeleeAttack { get; set; }
        }
}
