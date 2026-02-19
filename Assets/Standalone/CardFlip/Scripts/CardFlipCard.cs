using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Standalone.CardFlip
{
    public class CardFlipCard : MonoBehaviour
    {
        [SerializeField] private GameObject cardBack;
        [SerializeField] private GameObject cardFront;
        [SerializeField] private Text cardContentText; // 이미지 대신 텍스트로 대체
        [SerializeField] private Button cardButton;

        public int CardIndex { get; private set; }
        public bool IsFlipped { get; private set; }
        public bool IsMatched { get; private set; }

        private Action<int> _onClicked;
        private bool _isAnimating;

        public void Initialize(int index, CardFlipData data, Action<int> onClicked)
        {
            CardIndex = index;
            _onClicked = onClicked;
            IsFlipped = false;
            IsMatched = false;
            _isAnimating = false;

            // 텍스트 콘텐츠 적용
            if (data != null)
            {
                cardContentText.text = data.cardName; // 짝 맞추기 확인용 텍스트
            }

            cardBack.SetActive(true);
            cardFront.SetActive(false);
            transform.localRotation = Quaternion.identity;

            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(OnCardClicked);
        }

        private void OnCardClicked()
        {
            if (IsFlipped || IsMatched || _isAnimating) return;

            _onClicked?.Invoke(CardIndex);
            Flip(true);
        }

        public void Flip(bool toFront)
        {
            if (_isAnimating) return;
            StartCoroutine(FlipRoutine(toFront));
        }

        private IEnumerator FlipRoutine(bool toFront)
        {
            _isAnimating = true;

            // 1단계: 90도 회전 (옆면이 보이게)
            iTween.RotateTo(gameObject, iTween.Hash(
                "y", 90,
                "time", GameSettings.FlipDuration / 2f,
                "easetype", iTween.EaseType.linear,
                "islocal", true
            ));

            yield return new WaitForSeconds(GameSettings.FlipDuration / 2f);

            // 상태 전환
            IsFlipped = toFront;
            cardBack.SetActive(!toFront);
            cardFront.SetActive(toFront);

            // 2단계: 다시 0도로 회전하여 정면 표시
            iTween.RotateTo(gameObject, iTween.Hash(
                "y", 0,
                "time", GameSettings.FlipDuration / 2f,
                "easetype", iTween.EaseType.linear,
                "islocal", true
            ));

            yield return new WaitForSeconds(GameSettings.FlipDuration / 2f);

            _isAnimating = false;
        }

        public void SetMatched()
        {
            IsMatched = true;
        }

        public void ResetCard()
        {
            if (IsFlipped && !IsMatched)
            {
                Flip(false);
            }
        }
    }
}
