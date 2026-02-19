using System;
using System.Collections.Generic;
using System.Linq;

namespace Standalone.CardFlip
{
    public class CardFlipPresenter
    {
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnChanceChanged;
        public event Action<bool, List<int>> OnFlipResult; // 결과 통보용 이벤트 추가

        private List<CardFlipData> _cardPool;
        private int[] _shuffledIndices;
        private int _remainingChances;
        private int _firstSelectedIndex = -1;
        private int _matchedPairCount = 0;
        private GameState _currentState = GameState.Playing;

        public int RemainingChances => _remainingChances;
        public GameState CurrentState => _currentState;

        public void Initialize(List<CardFlipData> cardPool)
        {
            _cardPool = cardPool;
            _remainingChances = GameSettings.TotalTryLimit;
            _matchedPairCount = 0;
            _firstSelectedIndex = -1;
            _currentState = GameState.Playing;

            // 이미 매니저에서 섞인 데이터의 groupIndex를 매핑 (LINQ Select 제거)
            _shuffledIndices = new int[_cardPool.Count];
            for (int i = 0; i < _cardPool.Count; i++)
            {
                _shuffledIndices[i] = _cardPool[i].groupIndex;
            }

            OnChanceChanged?.Invoke(_remainingChances);
            OnGameStateChanged?.Invoke(_currentState);
        }

        public void FlipCard(int cardIndex)
        {
            if (_currentState != GameState.Playing || _remainingChances <= 0) return;

            // 첫 번째 선택
            if (_firstSelectedIndex == -1)
            {
                _firstSelectedIndex = cardIndex;
                return;
            }

            // 두 번째 선택 (같은 카드 클릭 방지)
            if (_firstSelectedIndex == cardIndex) return;

            _remainingChances--;
            OnChanceChanged?.Invoke(_remainingChances);

            bool isMatch = _shuffledIndices[_firstSelectedIndex] == _shuffledIndices[cardIndex];
            List<int> indices = new List<int>();
            indices.Add(_firstSelectedIndex);
            indices.Add(cardIndex);

            if (isMatch)
            {
                _matchedPairCount++;
                OnFlipResult?.Invoke(true, indices);
            }
            else
            {
                OnFlipResult?.Invoke(false, indices);
            }

            _firstSelectedIndex = -1;

            CheckGameState();
        }

        private void CheckGameState()
        {
            int requiredPairs = _cardPool.Count / 2;
            if (_matchedPairCount == requiredPairs)
            {
                _currentState = GameState.Win;
                OnGameStateChanged?.Invoke(_currentState);
            }
            else if (_remainingChances <= 0)
            {
                _currentState = GameState.Lose;
                OnGameStateChanged?.Invoke(_currentState);
            }
        }

        public CardFlipData GetCardData(int dataIndex)
        {
            if (dataIndex >= 0 && dataIndex < _cardPool.Count)
                return _cardPool[dataIndex];
            return null;
        }
    }
}
