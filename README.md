# ServerS - Overwatch 2 Server Picker

<div align="center">
  <img src="https://img.shields.io/github/license/Aquavv/ServerS---Game"/>
  <img src="https://img.shields.io/github/v/release/Aquavv/ServerS---Game"/>
</div>

**ServerS** is a lightweight tool that helps you pick the exact server you want to play on in Overwatch 2 (and other games). If you're tired of being forced into high ping lobbies or getting routed to bad datacenters, this app lets you block them completely.

By adding simple Windows Firewall rules, the app blocks the IP addresses of servers you don't want (including the new AWS and Google Cloud servers Blizzard is using). This forces the game to connect you to your best local server.

### 🌟 [Download ServerS Here](https://Aquavv.github.io/ServerS---Game)

---

## ⚡ Features
- **Block High Ping Servers:** Easily block regions that give you lag, like the AWS/GCP nodes in São Paulo or other distant datacenters.
- **Safe to Use:** It only adds rules to your Windows Firewall. It doesn't modify game files, read memory, or inject anything, so it won't trigger anti-cheats.
- **Live Ping Monitor:** See your actual latency and packet loss to different regions directly in the app.
- **Simple UI:** Just check the servers you want to block and hit apply.

## ⬇️ How to Install
1. Go to the [Releases](https://github.com/Aquavv/ServerS---Game/releases) page.
2. Download `serverS.exe`.
3. Run the installer (you'll need to run it as Admin so it can edit the Windows Firewall).
4. Open the app from your desktop and choose your servers.

---

## ❔ FAQ

**Can I get banned for this?**
No. ServerS just uses Windows Firewall to block IP addresses. It doesn't touch the game client at all.

**Why does it need Admin rights?**
Windows requires administrator privileges to add or remove Firewall rules. The app can't block the servers without it.

**I blocked a server but I'm still connecting to it?**
Blizzard sometimes adds new server IPs. Make sure you're using the latest version of ServerS, as we constantly update the IP lists (like the latest GCP blocks). 

**My game gets stuck on "Game Found" / timeouts**
You probably blocked too many servers. Try unblocking a few nearby regions so the matchmaking system has a fallback option.

---

## 🛠️ Build it yourself

If you want to compile it from source, you'll need the .NET 10 SDK:
```bash
git clone https://github.com/Aquavv/ServerS---Game.git
cd ServerS
dotnet build ServerPickerX.slnx -c Release
```
To build the installer itself, you'll need [Inno Setup 6](https://jrsoftware.org/isinfo.php). Just run `iscc setup.iss`.

---

## 💖 Support the Project

I built this tool to help the community play without lag. If it helped you out and you want to say thanks, you can buy me a coffee!

[![Donate](https://img.shields.io/badge/Donate-PayPal-blue.svg)](https://paypal.me/TuUsuarioPayPal)

---

## 🙌 Credits

Thanks to the open-source community. This project was heavily inspired by the techniques used in various CS2 server pickers for manipulating firewalls and testing ping. 
