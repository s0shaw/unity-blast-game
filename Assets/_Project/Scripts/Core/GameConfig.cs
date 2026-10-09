using UnityEngine;

namespace GemBlast.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "GemBlast/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [Header("Board Settings")]
        [Range(2, 10)]
        public int rows = 10;

        [Range(2, 10)]
        public int columns = 8;

        [Range(1, 6)]
        public int colorCount = 6;

        [Header("Match Rules")]
        [Min(2)]
        public int minMatchSize = 2;

        [Header("Icon Thresholds")]
        public int thresholdA = 4;
        public int thresholdB = 7;
        public int thresholdC = 9;

        [Header("Gameplay")]
        public int maxMoves = 20;

        [Header("Scoring")]
        [Tooltip("Points = group size squared x this value")]
        public int pointsPerBlockSquared = 10;

        [Tooltip("Groups this large (or larger) count as a big blast")]
        public int bigBlastSize = 4;

        [Header("Timing (seconds)")]
        public float blastDelay = 0.3f;
        public float settleDelay = 0.5f;

        private void OnValidate()
        {
            if (thresholdB <= thresholdA)
                thresholdB = thresholdA + 1;
            if (thresholdC <= thresholdB)
                thresholdC = thresholdB + 1;
        }
    }
}
