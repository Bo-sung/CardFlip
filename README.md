# CardFlip

**트윈(tween) 연출 처리 역량을 보여주기 위한 초단기 포트폴리오 프로젝트**입니다. 카드 짝 맞추기(메모리 매칭) 미니게임을 소재로, iTween 기반의 카드 플립·결과 패널 연출과 애니메이션 중 입력 제어를 구현했습니다. 짧은 제작 기간에도 MVP(Model-View-Presenter) 구조로 게임 로직과 UI를 분리했습니다.

## 기술 스택

- **엔진**: Unity 6000.3.7f1 (Unity 6), Universal Render Pipeline (URP 17.3.0)
- **언어**: C#
- **UI**: uGUI (GridLayoutGroup 기반 카드 그리드), TextMesh Pro
- **애니메이션**: iTween (Pixelplacement 플러그인)
- **기타**: Unity Input System

## 게임 규칙

- 8종류의 카드 쌍, 총 16장(4x4 그리드)
- 두 장을 뒤집어 같은 짝이면 매칭 성공, 아니면 다시 뒤집힘
- 시도 기회는 총 20회 — 모든 쌍을 맞추면 승리(Win), 기회 소진 시 패배(Lose)

## 구현된 기능

### 게임 로직 (Presenter)
- `CardFlipPresenter`: MonoBehaviour에 의존하지 않는 순수 C# 프리젠터
  - 1차/2차 카드 선택 상태 관리, 같은 카드 중복 클릭 방지
  - `groupIndex` 비교를 통한 짝 매칭 판정
  - 남은 기회 차감 및 Win/Lose 상태 판정
  - `OnGameStateChanged` / `OnChanceChanged` / `OnFlipResult` 이벤트로 UI에 결과 통보 (View와 로직 분리)

### 카드 데이터
- `CardFlipManager`: 8쌍(16장)의 카드 데이터를 생성 후 Fisher-Yates 셔플로 배치
- `CardFlipData` / `RewardData`: 카드 ID, 그룹, 보상(아이템/재화/에이전트 타입, 수량) 데이터 모델
- `GameSettings`: 카드 쌍 수, 시도 제한, 플립 시간 등 상수 설정 분리

### 카드 연출 (View)
- `CardFlipCard`: iTween `RotateTo` 2단계(0→90도에서 앞/뒷면 교체 후 90→0도) 플립 애니메이션
  - 애니메이션 중 클릭 입력 차단(`_isAnimating`), 이미 뒤집힌/매칭된 카드 클릭 무시
- 매칭 실패 시 일정 시간(`FlipDuration + 0.5s`) 후 자동으로 다시 뒤집기

### UI
- `UICardFlipEvent`: 카드 프리팹 동적 생성·배치, GridLayoutGroup을 코드로 자동 구성(4열 고정), 남은 기회 텍스트 갱신, 승리/패배 패널 전환, 프리젠터 이벤트 구독/해제(OnDestroy)
- `UIResultPanel`: 결과 패널 등장 연출 — 타이틀 → 재시작 버튼 → 종료 버튼 순으로 iTween `ScaleTo`(easeOutBack) 순차 확대 애니메이션, 재시작(씬 리로드)/종료 버튼 처리

## 폴더 구조

```
Assets/
├── Standalone/CardFlip/Scripts/   # 게임 핵심 스크립트
│   ├── CardFlipManager.cs         # 게임 시작, 카드 데이터 생성/셔플
│   ├── CardFlipPresenter.cs       # 매칭 판정·게임 상태 로직 (순수 C#)
│   ├── CardFlipDataModels.cs      # 데이터 모델, enum, 게임 설정 상수
│   ├── CardFlipCard.cs            # 개별 카드 View (플립 애니메이션)
│   ├── UICardFlipEvent.cs         # 메인 게임 UI (카드 그리드, 기회 표시)
│   └── UIResultPanel.cs           # 승패 결과 패널 연출
├── CardFlipCard.prefab            # 카드 프리팹
├── Scenes/SampleScene.unity       # 게임 씬
├── Plugins/Pixelplacement/iTween/ # iTween 애니메이션 플러그인
├── Settings/                      # URP 렌더 파이프라인 설정 (PC/Mobile)
└── TextMesh Pro/                  # TMP 리소스
```
