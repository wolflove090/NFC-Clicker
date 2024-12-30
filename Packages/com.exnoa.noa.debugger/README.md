# NOA Debugger

English / [日本語](README-ja.md)

NOA Debugger is a tool that supports performance measurement and debugging on runtime by integrating it into client
applications.

## System Requirements

System requirements for this tool are as follows.

| OS / Environment | Supported versions      |
|------------------|-------------------------|
| iOS              | 12+                     |
| Android          | 6+                      |
| Windows / exe    | Windows 10 / Windows 11 |
| web browsers     | Chrome (latest version) |
| Unity Editor     | 2021.3+                 |

You can use this tool even if you do not meet the system requirements, but some functions may not work properly.

### Operation Confirmed Environments

We have confirmed that this tool works with the following versions of Unity:

- 2021.3.33f1
- 2022.3.51f1
- 6000.0.25f1

Moreover, in URP projects, we have confirmed that it operates with the following versions:

- 2021.3.33f1

## Tool Installation Guide

Explains the method of introducing this tool.<br>
[explanation](./Documentation~/en/Importing.md)

## Launching Tool

Explains how to launch this tool.<br>
[explanation](./Documentation~/en/Launching.md)

## Basic Operations

Explains the screen configuration and basic operations method of this tool.<br>
[explanation](./Documentation~/en/BasicOperations.md)

## Tool Settings

Explains how to set up this tool.<br>
[explanation](./Documentation~/en/Settings.md)

## Launching Various Tools in the Editor

Explains how to launch various tools in the editor.<br>
[explanation](./Documentation~/en/Tools.md)

## About the APIs

Explains about the APIs provided by this tool.<br>
[explanation](./Documentation~/en/Apis.md)

## Explanation of Each Function

### Information

Explains the feature that displays information about the operating environment.<br>
[explanation](./Documentation~/en/Information.md)

### Profiler

Explains the function that handles runtime performance measurement operations and displays measurement information.<br>
[explanation](./Documentation~/en/Profiler.md)

### Snapshot

Explains the function that enables the retention and comparison of Profiler information.<br>
[explanation](./Documentation~/en/Snapshot.md)

### ConsoleLog

Explains the feature that displays logs outputted through the Unity Debug class.<br>
[explanation](./Documentation~/en/ConsoleLog.md)

### APILog

Explains the feature that displays web API logs.<br>
[explanation](./Documentation~/en/ApiLog.md)

### Hierarchy

Explains the feature that displays hierarchy information within a scene.<br>
[explanation](./Documentation~/en/Hierarchy.md)

### DebugCommand

Explains the feature that displays the set debug commands.<br>
[explanation](./Documentation~/en/DebugCommand/DebugCommand.md)

## Adding Custom Menus to NOA Debugger

Explains how to add custom menus to the NOA Debugger.<br>
[explanation](./Documentation~/en/CustomMenu.md)

## About the Processes NOA Debugger Executes for the Application

Explains the processes that NOA Debugger executes for the application.<br>
[explanation](./Documentation~/en/InAppBehavior.md)

## Removing Tool from Compile

Explains how to remove the tool, which can be useful in cases such as when you want to compile without the tool in
environments like the release environment.<br>
[explanation](./Documentation~/en/ExcludingFromCompile.md)

## Environment- and Device-specific Setup Guide

Explains how to operate the NOA Debugger, depending on the environment and device it operates in.<br>
[explanation](./Documentation~/en/EnvironmentAndDeviceSetupGuide.md)

## License

For information about the NOA Debugger license, please refer to [this](./LICENSE.md).

## Inquiries

Please contact us from [here](https://discussions.unity.com/t/noa-debugger-for-unity-feedback-questions-and-feature-requests) for inquiries about the NOA
Debugger.

You can also access it from the [Open Unity Discussions] button in the NOA Debugger Editor that starts by
selecting `Window -> NOA Debugger` from the Unity menu.
