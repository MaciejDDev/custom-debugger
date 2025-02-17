# CustomDebugger

 Custom Debugger is a Unity tool that extends the console logs, allowing you to create custom log categories with assigned colors for better organization and visualization.

 *Features*
 - Customizable log categories
 - Unique colors for each category
 - Automatic generation of static constants
 - Easy integration with any Unity project

1. Installation

- Install as a Unity Package
Open Unity, go to Window > Package Manager.
Click on + > Add package from git URL...
git URL: https://github.com/MaciejDDev/custom-debugger.git

Usage
1️. Open the Settings Window
Go to Window > Custom Debugger and configure your log categories.
![image](https://github.com/user-attachments/assets/4cd7c8ac-8ff1-4e33-a901-3a22190aaf8f)

This will automatically generate two files:
- Assets/Resources/CustomDebugger/CustomDebuggerSettings.asset
- Assets/Resources/CustomDebugger/LogCategories.cs

![image](https://github.com/user-attachments/assets/27b760ea-f7a6-4dee-8516-6eb7147ee225)

NOTE: You should not modify these classes manually. Instead use the settings window.

2️. Define Custom Categories
From the settings window, you can add new log categories, set colors, enable/disable categories.

3. Code examples
   
  CustomDebugger.Log(LogCategories.Default,"Hello World");
  CustomDebugger.Log(LogCategories.Player,"Player spawned");
  CustomDebugger.LogWarning(LogCategories.Player,"Player warning");
  CustomDebugger.LogError(LogCategories.Player,"Player null reference");
  CustomDebugger.Log(LogCategories.Enemies,"Enemy spawned.");

![image](https://github.com/user-attachments/assets/29457f46-642d-4220-8583-c65d3292c13b)
