using System;

namespace TinyWarriorInfo
{
        [Serializable]
        public class PlayerAction
        {
                public string PlayerIndex { get; set; }
                public float PositionX { get; set; }
                public float PositionY { get; set; }
                public float Horizontal { get; set; }
                public float Vertical { get; set; }
                public float Speed { get; set; }
                public float Health { get; set; }
                public bool IsMeleeAttack { get; set; }
                public bool IsRangedAttack { get; set; }
        }
}
