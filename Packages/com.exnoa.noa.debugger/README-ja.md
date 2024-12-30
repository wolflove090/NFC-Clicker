# NOA Debugger

[English](README.md) / 日本語

NOA Debuggerは、クライアントアプリケーションへ組み込むことでランタイム上のパフォーマンス計測やデバッグをサポートするツールです。

## ツールの動作保証環境

本ツールの動作保証環境は以下のとおりです。

| OS / 動作環境     | 動作保証バージョン               |
|---------------|-------------------------|
| iOS           | 12+                     |
| Android       | 6+                      |
| Windows / exe | Windows 10 / Windows 11 |
| ブラウザ          | Chrome (最新版)            |
| Unityエディタ     | 2021.3+                 |

動作保証環境を満たしていない場合においても本ツールをご利用いただけますが、一部機能が正常に動作しない場合があります。

### 動作確認環境

本ツールは以下のバージョンのUnityにおいて動作することを確認しています。

- 2021.3.33f1
- 2022.3.51f1
- 6000.0.25f1

また、URPプロジェクトにおいては以下のバージョンで動作することを確認しています。

- 2021.3.33f1

## ツールの導入方法

本ツールの導入方法について解説します。<br>
[解説](./Documentation~/ja/Importing.md)

## ツールの起動方法

本ツールの起動方法について解説します。<br>
[解説](./Documentation~/ja/Launching.md)

## ツールの操作方法

本ツールの画面構成と操作方法について解説します。<br>
[解説](./Documentation~/ja/BasicOperations.md)

## ツールの設定方法

本ツールの設定方法について解説します。<br>
[解説](./Documentation~/ja/Settings.md)

## エディタ上における各種ツールの起動方法

エディタ上における各種ツールの起動方法について解説します。<br>
[解説](./Documentation~/ja/Tools.md)

## ツールが提供するAPIについて

本ツールの提供するAPIについて解説します。<br>
[解説](./Documentation~/ja/Apis.md)

## 各機能についての説明

### Information

動作環境についての情報を表示する機能について解説します。<br>
[解説](./Documentation~/ja/Information.md)

### Profiler

Runtimeのパフォーマンス計測の操作と計測情報を表示する機能について解説します。<br>
[解説](./Documentation~/ja/Profiler.md)

### Snapshot

Profiler情報を保持し比較できる機能について解説します。<br>
[解説](./Documentation~/ja/Snapshot.md)

### ConsoleLog

UnityのDebugクラスを通して出力されたログを表示する機能について解説します。<br>
[解説](./Documentation~/ja/ConsoleLog.md)

### APILog

Web APIのログを表示する機能について解説します。<br>
[解説](./Documentation~/ja/ApiLog.md)

### Hierarchy

シーン内のヒエラルキー情報を表示する機能について解説します。<br>
[解説](./Documentation~/ja/Hierarchy.md)

### DebugCommand

設定したデバッグコマンドを表示する機能について解説します。<br>
[解説](./Documentation~/ja/DebugCommand/DebugCommand.md)

## NOA Debuggerに独自のメニューを追加する方法

NOA Debuggerに独自のメニューを追加する方法について解説します。<br>
[解説](./Documentation~/ja/CustomMenu.md)

## NOA Debuggerがアプリケーションに対して実行する各種処理について

NOA Debuggerがアプリケーションに対して実行する各種処理について解説します。<br>
[解説](./Documentation~/ja/InAppBehavior.md)

## ツールを取り除く方法

リリース環境ではツールを取り除いてコンパイルしたいケースがあります。<br>
ここでは、特定の環境でツールを取り除く方法を解説します。<br>
[解説](./Documentation~/ja/ExcludingFromCompile.md)

## 環境別・デバイス別セットアップガイド

NOA Debuggerを動作させる環境やデバイスによる利用方法について解説します。<br>
[解説](./Documentation~/ja/EnvironmentAndDeviceSetupGuide.md)

## ライセンス

NOA Debuggerのライセンスに関する情報は [こちら](./LICENSE.md) を参照してください。

## お問い合わせ

NOA Debuggerに関するお問い合わせは [こちら](https://discussions.unity.com/t/noa-debugger-for-unity-feedback-questions-and-feature-requests) からご連絡ください。<br>
Unityメニューの `Window -> NOA Debugger` を選択することで起動するNOA Debugger Editorの [Open Unity Discussions] ボタンからもアクセスできます。
