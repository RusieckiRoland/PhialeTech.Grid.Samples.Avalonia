# Implementation and verification — 2026-11-19

## Author requirement and implementation

Source: the author requested equivalent Avalonia examples in this repository after releasing Grid/Design and accepting the WPF examples on the Windows VM. The eight scenarios, their numbering, independent master/detail edits and shared toolbar behavior are author requirements carried over from the WPF work.

AI adapted the author's published WPF examples (source export 44cb14c) to native Avalonia views and lifecycle. It retained the application query/commit contracts, EF endpoints and synthetic databases. This is adaptation of the author's project, not a third-party repository implementation. Per-lesson README files explain the native composition and operation.

Grid 2.0.0-preview.12 and Design.Avalonia 0.1.0-preview.49 are public PackageReferences. No private component source checkout is required. The sixteen Windows Debug/Release outputs were compared with the public NuGet DLL payloads. No new component version was created for these examples.

## Accepted checks

- Windows: Debug and Release solution builds, zero warnings/errors.
- Windows: all five portable data runners passed.
- Windows: all eight native Avalonia scenario checks passed, including actual REST/EF/SQLite reads, buffered/immediate writes, a custom UserControl and independent master/detail saves with the shared toolbar.
- Linux (Ubuntu 22.04/WSL): all five portable data runners and all eight native headless scenario checks passed. These checks render real Avalonia controls; they do not measure a desktop GPU or driver.
- Windows desktop: all eight actual applications completed the --smoke startup check; the three REST servers were started for the check and only owned processes were stopped afterward.
- Screenshots of editing, snapshot review, custom detail and shared/separate master-detail bars were reviewed.

Evidence: ignored artifacts/verification and artifacts/screenshots; host orchestration logs in Phiale.Shell/artifacts-temp/avalonia-*.log. No manual exhaustive keyboard/drag/graphics-performance acceptance is claimed. No Avalonia Git publication is claimed.

The snapshot scenario was additionally extended to commit through the real REST endpoint, download a fresh snapshot and assert that its saved comment survived. This check passed on Windows and Linux. Both source SQLite databases retain the exact hashes of the published WPF seeds after all checks.

## Local shared splitter acceptance — 2026-11-19

At the splitter acceptance stage, the ignored LocalPackages.props selected protected Grid2.0.0-preview.12.splitter.2 and unobfuscated Design.Avalonia0.1.0-preview.50.splitter.2 from artifacts/local-feed. This is a local candidate, not a NuGet.org release. Every one of the8native application cases passed and16Debug/Release outputs match package DLLs. The actual demo01 window was opened and its Grid options panel resized using the shared Design Thumb. To explicitly build with the published dependencies instead, pass -p:UseLocalPhialePackages=false; fresh clones without the ignored file select Grid12/Design49.

The earlier public-package demo01 comparison used two actual desktop reloads of a million records: WPF8.19/8.32seconds, Avalonia8.13/8.10seconds. Both default to pages of100rows; the scrollbar test covered the current page, and Last navigated to999901–1000000 without an observed hang. This does not establish a statistically reliable performance advantage, unpaged-million scrolling acceptance or GPU/FPS performance. Detailed evidence stays in Shell/artifacts-temp/demo01-comparison-20261119.md. Component performance regression counters remain a separate open gate.


## Local Elements-menu acceptance — 2026-11-19

At the menu.2 acceptance stage, the ignored LocalPackages.props selected protected Grid2.0.0-preview.12.menu.2 and unobfuscated Design.Avalonia0.1.0-preview.50.menu.2. All8native application cases PASS, both solution configurations build with0warnings/0errors, and16client outputs match candidate-package DLL hashes (artifacts/verification/menu-protected/payloads.json). The actual demo01 was opened: the three-dot menu matches the WPF reference; Filter renders real input editors, Summaries opens its empty band, disabled read-only choices remain disabled, and toggling preserves the open popup. The intermediate menu.1 had a filter repaint omission caught by this desktop review; its logs are retained and it is not the final accepted candidate.

Source component checks: Windows/Linux Grid152 PASS/6SKIP, Styles24 PASS, Design19 PASS. WPF reference4 PASS; labels match in four languages, row height33DIP and EN/PL menu dimensions match. DE/FR text measurement differs by1DIP; identical pixels across rasterizers or DPI are not claimed. Full performance27/27 has21/14alarms vs first/previous; the independent component performance gate remains open. See Phiale.Grid AI_Start/Weryfikacja/Dziennik.md for ranges and limits. No Git, NuGet.org or website publication performed for these candidates. Published dependency defaults remain12/49 for fresh clones.


## Local corner acceptance — 2026-11-19

At the corner acceptance stage, LocalPackages.props selected protected Grid2.0.0-preview.12.menu.3 with unchanged unobfuscated Design.Avalonia50.menu.2. The author identified a5DIP white gap between the options button and the Symbol column. A native layout test reproduced15DIP vs20DIP; Grid Styles now stretches the corner button to the full cell in both directions. No outer demo margin or data-column geometry was changed. Windows/Linux Grid152/6 and Styles24 PASS; all8native application cases PASS;16Debug/Release payloads match the candidate nupkg. Fresh01-records.png reviewed without the gap. Builds have0warnings/errors. No publication performed.

The required performance series2026-11-19-elements-corner completed27/27 with551source files stable;19alarms against the first baseline and6against the preceding menu series, all six on Linux. Windows has no>=10% alarm vs that preceding series. These observations do not establish causation or close the historical performance gate; see the component verification diary and full ranges. The earlier menu-series comparison retained a minimized idle demo; this final series was started after confirming comparison demos were closed.


## Local status-bar acceptance — 2026-11-19

At the status-bar acceptance stage, LocalPackages.props selected protected Grid2.0.0-preview.12.status.1 and unchanged unobfuscated Design.Avalonia0.1.0-preview.50.menu.2. All8native scenarios PASS; Debug/Release solution builds have zero warnings/errors; all16client DLL payloads match packages (artifacts/verification/status-protected/payloads.json). Seven demos explicitly hide diagnostic counters as their WPF counterparts do; demo05 retains them. ShowStatusBar remains enabled, including when Paging is disabled. The fresh protected demo02 loading render was reviewed. Native component tests prove the60x4DIP progress track has a moving18DIP segment and reads do not await its animation. Windows/Linux Grid161 PASS/6SKIP, Styles24 PASS; WPF measurement reference1 PASS.

IMPORTANT: this working source uses the newly introduced Avalonia ShowDiagnosticStatus property and therefore requires the local status candidate. The earlier public-package acceptance above is historical. Existing public12/49 dependency defaults are not acceptance of the current working tree without LocalPackages.props. A future authorized public release must update package defaults together with source publication. No Git/NuGet.org/site publication performed.

Performance2026-11-19-avalonia-status-bar:27/27,554source files stable;16alarms vs first baseline and9vs preceding corner series. Examples: Windows Avalonia dense100k scroll+15.11%, scroll allocations+21.18%; Linux sparse1m scroll+10.20%. No controlled A/B establishes causation. The performance gate remains open; full ranges and limits are in Phiale.Grid AI_Start/Weryfikacja/Dziennik.md and the series report. No desktop GPU/FPS acceptance is claimed.


## Local scrollbar acceptance — 2026-11-19

At the first scrollbar acceptance stage, LocalPackages.props selected protected Grid2.0.0-preview.12.scroll.1 and unobfuscated Design.Avalonia0.1.0-preview.50.scroll.1. The author's WPF is the reference: transparent page-click targets, native drag,8DIP minimum thumb at large extent,17DIP Grid gutter and native bar, full content viewport for thumb proportions and LargeChange. Shared appearance lives in Design; Grid-specific sizing lives in Grid Styles. WPF/Core product behavior was not modified.

Windows/Linux: Design21, Grid162 PASS/6SKIP, Styles24 PASS; WPF reference2 PASS. All8native scenarios PASS, both configurations build with zero warnings/errors, all16DLL outputs match package payloads (artifacts/verification/scroll-protected/payloads.json). Fresh01/02renders were reviewed. Current source retains its local-candidate dependency; no public12/49 fallback acceptance is claimed. No publication performed.

Required performance2026-11-19-avalonia-scrollbar:27/27,554source files stable, no comparison demos running.17alarms vs first/10vs previous status-bar series; the gate remains open. Examples: Windows Avalonia dense10k navigation+14.03%, dense100k reload+13.77%, Linux dense10k first raster+32.03%; unchanged WPF dense100k first raster+122.62%. No controlled A/B establishes causation. Full sample ranges are in the component report and verification diary.


## Local scrollbar-header acceptance — 2026-11-19

The author identified that the vertical track still started beside the headers. The previous scroll.1 acceptance covered sizes and clicks, not the track's top coordinate. Grid2.0.0-preview.12.scroll.2 now places it below DataTopInset (header plus filter), clamps its height, and calculates vertical ViewportSize/LargeChange from the remaining content height. The header background extends over the reserved gutter as in WPF. Design remains unobfuscated50.scroll.1; Grid scroll.2 is protected. LocalPackages.props selects these candidates. No public-package/source release is claimed.

Windows/Linux Grid164 PASS/6SKIP and Styles24 PASS; two new WPF reference tests confirm the actual header/filter boundary. Regression tests cover filter toggling, hidden headers, both directions, end-of-data and current-row identity. All8native scenarios PASS, Debug/Release zero warnings/errors,16outputs match package payloads (artifacts/verification/scroll-top-protected/payloads.json). Fresh01and05renders were reviewed: the track starts below the header/filter. The Linux wrapper returned a trailing-CR shell error after successful tests; actual final TRX results, not its exit status, establish those test counts.

Performance2026-11-19-avalonia-scrollbar-header:27/27,554source files stable,18alarms vs first/11vs previous. Examples: Windows Avalonia dense10k scroll+45.66%, Linux dense100k scroll+42.07%. Cause is not established without controlled A/B; the performance gate remains open. Full ranges and scope are in the component report/verification diary. No Git/NuGet/site publication performed.


## Local whole-row scrolling acceptance — 2026-11-19

At the whole-row acceptance stage, LocalPackages.props selected protected Grid2.0.0-preview.12.rows.1; Design remains unobfuscated50.scroll.1. The author chose complete records at both viewport edges and accepted an empty bottom strip. Core snaps vertical offsets to row boundaries, leaves out a record that does not fit completely, and pads the scroll extent so the final record remains reachable. Small relative movements accumulate; keyboard navigation exposes complete targets, including unloaded identities. Actual viewport dimensions are preserved. Oversized records cannot be made physically visible without resizing; detail control areas retain their separate viewport behavior.

Windows: Core1019, WPF503 PASS/5SKIP, Avalonia165 PASS/6SKIP, Styles24 PASS. Linux: Core1019, Avalonia165/6, Styles24 PASS. All8Avalonia native scenarios and4WPF native runners pass on protected packages. Both sample solutions build Debug/Release without warnings/errors;32current client outputs across both repositories match nupkg payloads. Verification selects the8current projects per solution rather than obsolete binaries left by earlier numbering. Evidence: artifacts/verification/whole-rows-protected/payloads.json; host whole-rows logs. Fresh01render reviewed with complete first/last drawn records and an empty bottom strip.

Performance2026-11-19-whole-rows:27/27,555stable source files;18alarms vs first baseline/4vs previous. Linux sparse1m scroll+13.88%, Linux dense100k first raster+16.40%, WPF dense100k first raster+15.21%; cause not established without A/B. The performance gate stays open. Painting fewer complete rows in the same physical viewport changes the work done by the new policy. No Git/NuGet/site publication or VM acceptance claimed. Component documentation has a sealed4-language local draft.


## Current local data-preview scope acceptance — 2026-11-19

LocalPackages.props selects protected Grid2.0.0-preview.12.fade.1; Design remains unobfuscated50.scroll.1. Data fading uses LoadingPreviewOpacity and data-only drawing/native Canvas scopes. Both native scrollbars, column headers, filters, menu and footer stay outside the effect. Core and WPF product code did not change. The WPF reference already fades its separate SurfacePanel; its scrollbar/ancestor opacity and real rendered track pixels were checked during a held read.

Windows/Linux Avalonia165 PASS/6SKIP, Styles24 PASS; WPF5targeted loading-preview cases PASS. Pixel regressions require a visible nonwhite track sample and unchanged bytes before/while loading, for both orientations. Completion/error/cancel/rebind never await the test's1hour animation. Both-platform renders and fresh protected demo02 render reviewed. All8native demo scenarios PASS, Debug/Release zero warnings/errors;16client outputs match nupkg DLLs (artifacts/verification/fade-scope-protected/payloads.json). WPF demos retain their previously accepted rows.1 packages. No Git/NuGet/site publication or VM verification claimed.

Performance2026-11-19-data-preview-scope:27/27,555stable source files;18alarms vs first/17vs previous. Examples: Linux sparse1m scroll+33.12%, Windows Avalonia dense10k first raster+54.67%, unchanged WPF dense100k PrepareCore+78.80%. No A/B establishes cause. Performance gate remains open; the component report and verification diary retain full ranges and limits. Functional tests prove no awaited animation or added read, not zero rendering cost. Documentation is a sealed local4-language component draft.


## Current local filter-inset acceptance — 2026-11-19

LocalPackages.props selects protected Grid2.0.0-preview.12.inset.1; Design remains unobfuscated50.scroll.1. The filter gutter above the vertical scrollbar uses Design Surface.Secondary like the project WPF reference, outside data fading. Actual pixels checked in Light/Dark/HighContrast, including data opacity=.08. Windows/Linux Avalonia166PASS/6SKIP and Styles24PASS. All8native scenarios PASS; all8demos Debug/Release build without warnings/errors.16outputs match the protected nupkg payloads (artifacts/verification/inset-protected/payloads.json). Fresh05-buffered.png reviewed. WPF retains rows.1; author XAML preserved. No publication or VM acceptance claimed.

Performance2026-11-19-filter-inset:27/27,555stable sources,15first/9previous alarms>=10%. Examples vs previous: Linux dense10k scroll+17.22%, Windows Avalonia dense10k scroll+22.16%. Cause not established without A/B; performance gate stays open. Full ranges and limits remain in the component diary/report.

## macOS acceptance from NuGet.org — 2026-11-19

The author enabled SSH on their Mac mini and required packages to be restored only from the official feed. The agent configured a dedicated SSH key and a user-local .NET 8.0.428 SDK on macOS 27.2.1 arm64. The author approved transferring the working demo sources and two synthetic SQLite databases. NuGet packages were downloaded by the Mac from NuGet.org. The published versions are Grid Contracts, Localization, Core, Data.Http and Avalonia 2.0.0-preview.12.inset.1 plus Design.Avalonia 0.1.0-preview.50.scroll.1. An isolated cache confirmed that source for all six packages and both restored Avalonia DLLs matched the protected binaries. The clean Release build had zero warnings and errors, all five data runners and eight headless scenarios passed, and desktop startup passed for demos 01, 02 and 05 with the monitor awake. The headless checks required an explicit Release build of Native.Cases because the solution builds that referenced project in Debug. Logs are in artifacts/macos/nuget-org-inset.1-final under the Mac test checkout. No FPS measurements were taken.
