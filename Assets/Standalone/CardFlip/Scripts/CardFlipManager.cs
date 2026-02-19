using UnityEngine;
using System.Collections.Generic;

namespace Standalone.CardFlip
{
    public class CardFlipManager : MonoBehaviour
    {
        [SerializeField] private UICardFlipEvent uiRoot;

        private CardFlipPresenter _presenter;

        private void Start()
        {
            StartGame();
        }

        public void StartGame()
        {
            // 1. 모의 데이터 생성 (8쌍, 총 16장)
            List<CardFlipData> mockData = GenerateCardData();

            // 2. 프리젠터 초기화 (프리젠터 내부에서 pool을 다시 섞지 않고 사용)
            if (_presenter == null) _presenter = new CardFlipPresenter();
            _presenter.Initialize(mockData);

            // 3. UI 초기화
            if (uiRoot != null)
            {
                uiRoot.Initialize(_presenter, mockData);
                uiRoot.gameObject.SetActive(true);
            }
        }

        private List<CardFlipData> GenerateCardData()
        {
            List<CardFlipData> cardData = new List<CardFlipData>();
            int groupId = 1;

            for (int i = 0; i < 8; i++)
            {
                // 보상 데이터 정의
                RewardData reward = new RewardData(RewardType.Item, 1000 + i, (i + 1) * 10);
                string cardName = $"카드 {i}"; // 이미지 경로 대신 표시할 텍스트

                // 한 쌍(2장)의 카드 데이터 생성
                cardData.Add(new CardFlipData(i * 2, groupId, i, reward, cardName));
                cardData.Add(new CardFlipData(i * 2 + 1, groupId, i, reward, cardName));
            }

            // LINQ OrderBy(람다) 대신 명시적인 셔플 알고리즘 시용
            ShuffleList(cardData);

            return cardData;
        }

        private void ShuffleList(List<CardFlipData> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = UnityEngine.Random.Range(0, n + 1);
                CardFlipData value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
