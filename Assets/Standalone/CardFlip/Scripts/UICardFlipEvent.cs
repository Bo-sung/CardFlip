using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Standalone.CardFlip
{
    public class UICardFlipEvent : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text titleText;
        [SerializeField] private Text subTitleText;
        [SerializeField] private Text chanceText;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject losePanel;

        [Header("Card Grid")]
        [SerializeField] private Transform cardGridRoot;
        [SerializeField] private GameObject cardPrefab;

        private CardFlipPresenter _presenter;
        private List<CardFlipCard> _cards = new List<CardFlipCard>();

        public void Initialize(CardFlipPresenter presenter, List<CardFlipData> cardData)
        {
            _presenter = presenter;

            // 그리드 레이아웃 설정 (코드로 자동 구성)
            SetupGridLayout();

            // UI 초기화
            UpdateUI();
            winPanel.SetActive(false);
            losePanel.SetActive(false);

            // 카드 프리팹 생성 및 배치
            foreach (Transform child in cardGridRoot)
            {
                Destroy(child.gameObject);
            }
            _cards.Clear();

            for (int i = 0; i < cardData.Count; i++)
            {
                GameObject go = Instantiate(cardPrefab, cardGridRoot);
                CardFlipCard card = go.GetComponent<CardFlipCard>();
                card.Initialize(i, cardData[i], OnCardClicked);
                _cards.Add(card);
            }

            // 프레젠터 이벤트 구독 (이름 있는 메서드 연결)
            _presenter.OnGameStateChanged += HandleGameStateChanged;
            _presenter.OnChanceChanged += HandleChanceChanged;
            _presenter.OnFlipResult += HandleFlipResult;

            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(OnClickClose);
        }

        private void OnClickClose()
        {
            Application.Quit();
        }

        private void HandleChanceChanged(int count)
        {
            UpdateUI();
        }

        private void SetupGridLayout()
        {
            GridLayoutGroup grid = cardGridRoot.GetComponent<GridLayoutGroup>();
            if (grid == null) grid = cardGridRoot.gameObject.AddComponent<GridLayoutGroup>();

            // 16장(4x4) 또는 12장(4x3)에 적합한 기본 설정
            grid.cellSize = new Vector2(150, 200);     // 카드 크기
            grid.spacing = new Vector2(20, 20);       // 카드 간격
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.childAlignment = TextAnchor.MiddleCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 4;                 // 4열로 고정
        }

        private void OnCardClicked(int index)
        {
            // 프리젠터에게 클릭 통보 (결과는 HandleFlipResult 이벤트를 통해 수신)
            _presenter.FlipCard(index);
        }

        private void HandleFlipResult(bool isMatch, List<int> matchedIndices)
        {
            if (matchedIndices == null) return;

            if (isMatch)
            {
                for (int i = 0; i < matchedIndices.Count; i++)
                {
                    int idx = matchedIndices[i];
                    if (idx >= 0 && idx < _cards.Count)
                        _cards[idx].SetMatched();
                }
            }
            else
            {
                // 매칭 실패 시 잠깐 보여줬다가 다시 뒤집기
                StartCoroutine(ResetUnmatchedCards(matchedIndices));
            }
        }

        private System.Collections.IEnumerator ResetUnmatchedCards(List<int> indices)
        {
            yield return new WaitForSeconds(GameSettings.FlipDuration + 0.5f);

            if (indices != null)
            {
                for (int i = 0; i < indices.Count; i++)
                {
                    int idx = indices[i];
                    if (idx >= 0 && idx < _cards.Count)
                        _cards[idx].ResetCard();
                }
            }
        }

        private void UpdateUI()
        {
            if (chanceText != null)
                chanceText.text = string.Format("남은 기회: {0}", _presenter.RemainingChances);
        }

        private void HandleGameStateChanged(GameState state)
        {
            if (state == GameState.Win) winPanel.SetActive(true);
            else if (state == GameState.Lose) losePanel.SetActive(true);
        }

        private void OnDestroy()
        {
            if (_presenter != null)
            {
                _presenter.OnGameStateChanged -= HandleGameStateChanged;
                _presenter.OnChanceChanged -= HandleChanceChanged;
                _presenter.OnFlipResult -= HandleFlipResult;
            }
        }
    }
}
