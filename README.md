# ServerS - The Ultimate Connection Optimizer 🌐

### 🌟 [Visita el Sitio Web Oficial para Descargar ServerS](https://FN-FAL113.github.io/ServerS)

<!-- [INSERT_YOUR_LOGO_HERE] -->
*(Reemplaza esta línea con el enlace a tu logo. Ej: `<img src="ruta/a/tu/logo.png" width="200"/>`)*

[![License](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](LICENSE)
[![Release](https://img.shields.io/badge/Release-v1.0.0-success.svg)](#)

**ServerS** is an advanced, premium-grade network routing utility built for competitive gamers. Take absolute control over your matchmaking experience by forcing your game to connect only to the lowest-latency datacenters available worldwide.

</div>

---

## 🚀 Overview

In modern competitive games, matchmaking systems often route you to suboptimal servers to reduce queue times, resulting in frustrating, high-latency matches. **ServerS** solves this by intelligently managing your system's inbound and outbound network rules. 

By analyzing the global infrastructure of major gaming networks (including expansive AWS and Google Cloud Platform subnets), ServerS safely restricts your connection to specific regions. If you only want to play in your local region with 20ms ping, ServerS makes it happen.

<!-- [INSERT_APP_SCREENSHOT_HERE] -->
> 🖼️ *(Reemplaza esta línea con una captura de pantalla de la interfaz de la aplicación)*
> *Ej: `![App Interface](docs/screenshot.png)`*

---

## ✨ Key Features

- **🎯 Precision Routing:** Instantly block high-ping regions and AWS/GCP relay IP clusters with a single click.
- **🛡️ Safe & Non-Intrusive:** Operates strictly on a network level (Windows Firewall). It **does not** touch game files, inject code, or read memory, making it completely safe and undetectable by anti-cheats.
- **💎 Premium Commercial Interface:** Designed with a sleek, modern, and user-friendly aesthetic. No clunky terminals—just point, click, and play.
- **⚡ Real-Time Latency Tracking:** Continuously monitors ICMP ping responses to ensure your selected datacenters are performing optimally.
- **📦 Standalone Executable:** No bloated installations. A single portable `.exe` file that just works.

---

## 🛠️ How It Works

<!-- [INSERT_BEFORE_AFTER_IMAGE_HERE] -->
> 🖼️ *(Reemplaza esta línea con una imagen comparando el Ping en el juego Antes y Después)*

When you select a server in **ServerS**, the application leverages elevated privileges to seamlessly inject strict routing policies into your Windows Firewall. 

For example, if the matchmaking system tries to relay your connection through a distant datacenter (like `SAE1` or `GRU1`), the firewall instantly drops those packets, forcing the game's netcode to select the next best available server—which will be the local, low-ping region you left unblocked.

---

## 📥 Installation

1. Go to the [Releases](#) tab on this repository.
2. Download the latest `serverS.exe` standalone installer.
3. Run the installer (it will require Administrator privileges to apply network rules).
4. Launch **ServerS** from your desktop shortcut!

### Compiling from Source

If you prefer to compile the application yourself, you will need the .NET 10 SDK:
```bash
git clone https://github.com/YourUsername/ServerS.git
cd ServerS
dotnet build ServerPickerX.slnx -c Release
```

---

## ❔ FAQ

**Will this get me banned?**
Absolutely not. ServerS does not interact with the game client in any way. It acts as an automated network administrator tool.

**Why do I need to run it as Administrator?**
The Windows operating system requires Administrator privileges to modify system-level firewall rules. Without these permissions, the app cannot block the required IPs.

**I'm getting matchmaking timeouts, what's wrong?**
You may have blocked too many regions. The matchmaking system needs at least one healthy region to connect to. Ensure you leave your closest datacenter completely unblocked.

---

## 💖 Support the Project

ServerS is developed for free with passion. If this tool saved your matches and eliminated your lag, consider buying me a coffee! 

[![Donate with PayPal](https://raw.githubusercontent.com/stefan-niedermann/paypal-donate-button/master/paypal-donate-button.png)](https://paypal.me/TuUsuarioPayPal)

---

## 🙌 Acknowledgements & Credits

This project was built from the ground up to provide a premium experience for modern games, but we stand on the shoulders of giants. 

Special thanks to the open-source community and various legacy network-routing tools (such as community-made CS2 server pickers) for inspiring the firewall-manipulation concepts and ICMP ping methodologies used in this project. 

---
<div align="center">
  <i>Developed with ❤️ for the competitive gaming community.</i>
</div>
