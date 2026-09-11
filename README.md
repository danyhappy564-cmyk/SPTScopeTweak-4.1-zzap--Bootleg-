### ⚠️ IMPORTANT NOTICE / DISCLAIMER

**Original Author:** pein (peinwastaken)
**Original Repository:** SPTScopeTweak
**Original Link:** https://github.com/peinwastaken/SPTScopeTweak
**License:** See upstream repository
**This Port By:** R_F (danyhappy564-cmyk) — unofficial, AI-assisted port. Not affiliated with or endorsed by the original author.

1. **Reflection & Take-Downs:** I deeply reflect on the ECOT incident. As an AI-assisted "vibe coder," I will immediately delete files if the original authors ask.
2. **No Re-Distribution:** These ported builds are unverified, temporary fixes. Please do NOT re-upload or share them anywhere else.
3. **Do Not Pester Original Authors:** Never report bugs or pester original modders regarding issues from my unofficial ports.
4. **Full Credit & Respect:** I will always credit original creators on GitHub and prioritize their decisions above all else.
5. **Support Original Creators:** Instead of using my ports, please visit the original authors' Forge pages to leave kind words or tips.

---

# SPT Scope Tweaks (fork)

> **원작자 · 원본 레포**
> **pein** (`peinwastaken`) — https://github.com/peinwastaken/SPTScopeTweak
>
> 이 레포는 위 원작의 **포크**입니다. 기능은 그대로고, **SPT 4.1에서 빌드·동작하도록
> 포팅**한 것이 전부입니다.

배율경의 **아이릴리프(eye relief, 접안 여유)** 크기를 F12에서 배율로 조절하는
플러그인입니다. 값을 올리면 조준했을 때 스코프 안쪽 원(사출동공)이 더 크게 보여
검은 테두리가 줄어듭니다. 기본값 1.4배, 범위 0.1~5배.

현재 기준 **SPT 4.1**.

---

## 4.1 포팅에서 바뀐 것

### 소스는 손댈 게 없었습니다

4.1은 클라이언트를 역난독화해서 배포하지만, **이 모드가 쓰는 이름은 전부 원래부터
실명이었습니다.** 소스에 등장하는 식별자 68개를 SPT 4.1 wiki 의 4.0→4.1 대응표에 전수
대조했고 **걸린 게 하나도 없습니다.**

| 쓰는 것 | 4.1 상태 |
|---|---|
| `EFT.CameraControl.OpticSight` (`Awake`, `LensRenderer`) | 대응표에 없음 = 그대로 |
| `EFT.Player.OnGameSessionEnd` | 그대로 (4.1 실기 로그에서 확인) |
| 셰이더 키워드 `_Scales` / `_Shifts` / `_ShiftDirection` | 머티리얼 프로퍼티라 역난독화와 무관 |

검증으로 **리네임 전(4.0 원본)과 리네임 후(4.1 형태) 어셈블리 양쪽으로 빌드**해서 둘 다
통과하는 걸 확인했습니다 — 이 모드가 4.0/4.1 이름 변경에 영향받지 않는다는 뜻입니다.

### 문제는 빌드 설정이었습니다

- `net472` → `netstandard2.1` (SPT 4.1 클라 플러그인 기준)
- **참조 경로가 `..\..\` 하드코딩** — 레포가 SPT 설치 폴더 정확히 두 단계 안에 있을 때만
  풀립니다. `SptRoot` 로 대체 (기본값 `E:\SPT 4.1`, `-p:SptRoot=...` 또는 환경변수로
  덮어쓰기). 기존 `SPTPath` 도 별칭으로 계속 동작합니다
- **BepInEx / UnityEngine 을 NuGet 에서 받고 있었습니다.** `BepInEx.Core` 는
  `nuget.bepinex.dev` 라 네트워크에 따라 아예 닿지 않고, `UnityEngine.Modules` 는
  **2019.4.39** 로 고정돼 있었는데 EFT 0.16 이 도는 런타임은 Unity 2022 입니다. 설치본에는
  게임이 실제로 로드할 바로 그 어셈블리가 있으니 거기서 참조하도록 교체
- `CopyToSPT` 가 `'$(SPTPath)' != ''` 조건이라 **그냥 빌드하면 아무 데도 설치가 안 됐습니다.**
  `SptRoot` 는 항상 값이 있으므로 이제 기본 동작합니다
- `SptRoot` 가 SPT 설치본이 아니면 "타입을 찾을 수 없음" 수십 줄 대신 **이유를 말하는
  에러 하나**로 실패합니다

### 버그 하나 같이 고쳤습니다

`OnGameEndedPatch` 클래스가 있는데 **`Plugin.Awake` 에서 한 번도 `Enable()` 되지
않았습니다.** 그래서 `OpticMaterialData` 딕셔너리가 세션 내내 비워지지 않고, 이미
파괴된 `OpticSight` 까지 계속 들고 있었습니다. F12 에서 배율을 바꿀 때마다 그 전부를
순회하고요. 원래 그러라고 있는 패치라 등록만 해줬습니다.

## 빌드

```
dotnet build SPTScopeTweaks.csproj -c Release
dotnet build SPTScopeTweaks.csproj -c Release -p:"SptRoot=D:\내 SPT 경로"
```

빌드하면 `$(SptRoot)\BepInEx\plugins\SPTScopeTweaks\` 로 자동 설치됩니다.

---

## 확인한 것 / 확인 못 한 것

| | 상태 |
|---|---|
| 리네임 필요 여부 | **확인** — 식별자 68개 전수 대조, 대상 0개 |
| 4.0 원본 / 4.1 형태 어셈블리 양쪽 빌드 | **둘 다 통과** |
| 실제 4.1 `Assembly-CSharp.dll` 로 컴파일 | **못 함** — 이 작업 환경에 4.1 클라이언트 어셈블리가 없습니다 |
| 인게임 검증 | **안 함** |
