using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Standalone.CardFlip
{
    public class UIResultPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject panel;
        [SerializeField] private RectTransform titleTransform;
        [SerializeField] private RectTransform restartBtnTransform;
        [SerializeField] private RectTransform closeBtnTransform;

        [Header("Buttons")]
        [SerializeField] private Button RestartBtn;
        [SerializeField] private Button CloseBtn;

        [Header("Animation Settings")]
        [SerializeField] private float animTime = 0.5f;

        private void Awake()
        {
            RestartBtn.onClick.AddListener(OnRestartClicked);
            CloseBtn.onClick.AddListener(OnCloseClicked);

            // 초기 상태: 모두 크기 0
            ResetUI();
        }

        private void OnEnable()
        {
            ShowPanel();
        }

        private void ResetUI()
        {
            if (titleTransform != null) titleTransform.localScale = Vector3.zero;
            if (restartBtnTransform != null) restartBtnTransform.localScale = Vector3.zero;
            if (closeBtnTransform != null) closeBtnTransform.localScale = Vector3.zero;
        }

        public void ShowPanel()
        {
            if (panel == null) return;

            panel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(ShowSequenceRoutine());
        }

        private System.Collections.IEnumerator ShowSequenceRoutine()
        {
            // 초기화
            ResetUI();

            // 1단계: 타이틀 확대 등장
            if (titleTransform != null)
            {
                iTween.ScaleTo(titleTransform.gameObject, iTween.Hash(
                    "x", 1f, "y", 1f, "z", 1f,
                    "time", animTime,
                    "easetype", iTween.EaseType.easeOutBack,
                    "ignoretimescale", true
                ));
                yield return new WaitForSeconds(animTime);
            }

            // 2단계: 재시작 버튼 등장
            if (restartBtnTransform != null)
            {
                iTween.ScaleTo(restartBtnTransform.gameObject, iTween.Hash(
                    "x", 1f, "y", 1f, "z", 1f,
                    "time", animTime,
                    "easetype", iTween.EaseType.easeOutBack,
                    "ignoretimescale", true
                ));
                yield return new WaitForSeconds(animTime);
            }

            // 3단계: 종료 버튼 등장
            if (closeBtnTransform != null)
            {
                iTween.ScaleTo(closeBtnTransform.gameObject, iTween.Hash(
                    "x", 1f, "y", 1f, "z", 1f,
                    "time", animTime,
                    "easetype", iTween.EaseType.easeOutBack,
                    "ignoretimescale", true
                ));
            }
        }

        private void OnRestartClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnCloseClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}
