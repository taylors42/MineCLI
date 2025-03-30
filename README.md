# 🎮 MineCLI

<div align="center">

![MineCLI Logo](https://img.shields.io/badge/MineCLI-A%20Minecraft%20Server%20Management%20Tool-brightgreen?style=for-the-badge&logo=minecraft)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

</div>

## 📖 Overview

MineCLI is a powerful command-line interface tool designed to help Minecraft server administrators manage their servers efficiently. With MineCLI, you can monitor online players, manage server configurations, and perform administrative tasks all from a simple, intuitive console interface.

## ✨ Features

- **🔐 Secure Server Connection**: Connect to your Minecraft server using RCON with password authentication
- **👥 Player Monitoring**: Real-time monitoring of online players with auto-refresh capability
- **⚙️ Configuration Management**: Save and manage server configurations for quick access
- **🎨 User-Friendly Interface**: Colorful, intuitive console interface with menu navigation
- **🔄 Persistent Sessions**: Automatically reconnect to your saved server configuration

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or higher
- A Minecraft server with RCON enabled

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/taylors42/MineCLI
   cd MineCLI
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## 🔧 Usage

### First-time Setup

When you first run MineCLI, you'll be prompted to enter your Minecraft server details:

1. Enter your server IP or hostname
2. Enter the RCON port (default is 25575)
3. Enter your RCON password

After successful authentication, your server configuration will be saved for future sessions.

### Main Menu

The main menu provides the following options:

- **Show online Players**: Displays a list of currently connected players with auto-refresh
- **Delete my server config file**: Removes saved server configuration
- **Exit**: Closes the application

Use the arrow keys to navigate and press Enter to select an option.

## 📊 Player Monitoring

The player monitoring feature provides:

- Current player count / maximum player count
- List of all connected players
- Automatic refresh every 10 seconds
- Press any key to return to the main menu

## 🛠️ Technical Details

MineCLI is built with:

- C# and .NET 9.0
- RCON protocol for server communication
- JSON for configuration storage
- Console-based UI with color formatting

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🤝 Contributing

Contributions are welcome! Feel free to submit a Pull Request.

## 🙏 Acknowledgements

- [CoreRCON](https://github.com/Challengermode/CoreRcon) - RCON client library
- [Newtonsoft.Json](https://www.newtonsoft.com/json) - JSON framework

---

<div align="center">

**Made with ❤️ for Minecraft server administrators**

</div>