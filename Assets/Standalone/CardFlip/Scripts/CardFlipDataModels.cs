using System;
using System.Collections.Generic;

namespace Standalone.CardFlip
{
    [Serializable]
    public enum GameState
    {
        Playing,
        Win,
        Lose
    }

    [Serializable]
    public enum RewardType
    {
        None = 0,
        Item = 1,
        Currency = 2,
        Agent = 3
    }

    [Serializable]
    public struct RewardData
    {
        public RewardType type;
        public int id;
        public int count;

        public RewardData(RewardType type, int id, int count)
        {
            this.type = type;
            this.id = id;
            this.count = count;
        }
    }

    [Serializable]
    public class CardFlipData
    {
        public int id;
        public int groupId;
        public int groupIndex;
        public RewardData reward;
        public string cardName;

        public CardFlipData() { }

        public CardFlipData(int id, int groupId, int groupIndex, RewardData reward, string cardName = "")
        {
            this.id = id;
            this.groupId = groupId;
            this.groupIndex = groupIndex;
            this.reward = reward;
            this.cardName = cardName;
        }
    }

    public static class GameSettings
    {
        public const int CardPairCount = 8; // 8종류의 카드 쌍 (총 16장)
        public const int TotalTryLimit = 20; // 16장인 경우 기회를 조금 더 늘림
        public const float FlipDuration = 0.4f;
    }
}
