# Quickpaste
Quickpaste lets you host your own pastebin to quickly send or request files/text from others just by sending a link.  
You can host Quickpaste on Windows, Linux and macOS.


Anyone hosting a Quickpaste web app on their machine/server has full control over the pastes that are saved in it.  
You can choose to make your pastes public or private, and even temporarily share them with others.


Quickpaste uses the [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) web framework and runs on [the .NET Core 2 Runtime](https://www.microsoft.com/net/download).
However, you can run Quickpaste as an executable without installing the .NET Runtime if you have Windows 7/8.1/10 or Ubuntu 14.04/16.04.


### **You can download the latest version of Quickpaste from the [Releases](https://github.com/Beyhum/quickpaste/releases) page.**  
### **Check out the [Installation](https://github.com/Beyhum/quickpaste/wiki/Installation) page on the wiki to get started.**
### **Check out the [Building from Source](https://github.com/Beyhum/quickpaste/wiki/Building-from-Source) page if you'd like to contribute.**

## Features
- Create public/private paste with text/files
- Request text/files from others by sending them a link
- Temporarily share private files with others through a link that expires
- Copy links/text in one click
- Customize links or have them automatically generated
- Save files in local database or using cloud storage

## How to Use
- You have 1 User Account per Quickpaste server 
- A User can create Pastes or Upload Links (used to create a Paste without being authenticated)
- To request a paste from someone, create an Upload Link and send them its URL. The URL will allow anyone to create 1 Paste
- You can also enter the Upload Link's QuickLink field as a code in the Welcome page to create a Paste 

## Technologies Used
- ASP.NET Core 2
- Angular 4
- SQLite
- Webpack
- Bootstrap 3.3.7

## Examples

<img width="300" align="left" alt="Create Paste" title="Create Paste" src="https://gist.github.com/Beyhum/5c7a0a32be8feda2c86239593944c51e/raw/e86a2498241d907094c4e945c545193c0e515ec4/create-paste.gif">

<img width="300" align="right" alt="Share Paste" title="Share Paste"  src="https://gist.githubusercontent.com/Beyhum/5c7a0a32be8feda2c86239593944c51e/raw/e86a2498241d907094c4e945c545193c0e515ec4/share-paste.gif">
