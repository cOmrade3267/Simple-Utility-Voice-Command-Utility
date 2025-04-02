# Voice Assistant Utility Tool

## Overview
Voice Assistant Utility Tool is a Windows-based application built with C# and WinForms that allows users to control their system using voice commands. It features speech recognition and synthesis to perform various tasks such as opening applications, managing windows, and executing system commands.

## Features
- Voice command-based system control
- Open and close applications (e.g., Notepad, Calculator, Task Manager)
- Minimize all windows and show the desktop
- Lock the screen
- Open new tabs and windows in a browser
- Store and open up to 10 hyperlinks via voice commands
- Shutdown and restart the computer
- Supports global hotkey (Ctrl+Shift+Z) for toggling listening mode
- Runs in the system tray for seamless background operation

## Technologies Used
- C# (WinForms)
- System.Speech for voice recognition and synthesis
- Windows API (user32.dll) for system commands
- NotifyIcon for system tray integration

## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/VoiceAssistant.git
   ```
2. Open the project in Visual Studio.
3. Build and run the solution.

## Usage
- The application starts minimized in the system tray.
- Use **Ctrl+Shift+Z** to toggle listening mode.
- Speak predefined commands to control the system.
- Right-click the tray icon for settings or exit options.

## Commands List
| Command | Action |
|---------|--------|
| "What time is it" | Announces the current time |
| "Open Notepad" | Opens Notepad |
| "Close Notepad" | Closes all Notepad instances |
| "Minimize all windows" | Minimizes all open windows |
| "Show desktop" | Shows the desktop |
| "Lock screen" | Locks the Windows screen |
| "Open calculator" | Launches Calculator |
| "Open browser" | Opens the default browser |
| "Open task manager" | Opens Task Manager |
| "Shutdown computer" | Shuts down the system |
| "Restart computer" | Restarts the system |
| "Save" | Simulates Ctrl+S |
| "Copy" | Simulates Ctrl+C |
| "Paste" | Simulates Ctrl+V |
| "Open control panel" | Opens the Control Panel |
| "Open new window" | Opens a new window in the active application |
| "Open new tab" | Opens a new tab in the active browser |
| "Exit program" | Closes the application |
| "Open link X" | Opens stored link X (1-10) |

## Contribution
1. Fork the repository.
2. Create a new branch (`feature-branch`).
3. Make your changes and commit them.
4. Push to the branch and create a pull request.

## License
This project is licensed under the MIT License.

## Author
- **Your Name**
- GitHub: [cOmrade3267](https://github.com/yourusername)


