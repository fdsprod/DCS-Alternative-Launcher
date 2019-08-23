# DCS Alternative Launcher  
  
![DCS Alternative Launcher](https://cdn.discordapp.com/attachments/415664512981794818/611242983978827802/dal-icon-256.png)  

**Want to help support the project?**  
Never required but always appreciated.  Donations and Patreon subscriptions help motivate continued ehancements, feature request and support.   If you choose.  Please only subscribe or dontate if you feel that I deserve it or have earned it, and are financially able to do so (make sure you're taken care of first!).

[![](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=9RRKTB5QZBKDS&source=url)

[![](https://c5.patreon.com/external/logo/become_a_patron_button.png)](https://www.patreon.com/bePatron?u=8451589)

**About The Project**

The DCS Alternative Launcher was initially created to get around the setup process of viewports in DCS in conjunction with application called Helios.   The primary focus was to have the launcher make edits to stock DCS files to allow the name change of the standard viewport names (LEFT_MFCD, RIGHT_MFCD, etc) that are shared across modules so that viewports were uniquely named per module (F_18_LEFT_MFCD, A_10C_LEFT_MFCD) and no longer shared.  The issue here is that the edits go away each time DCS World updates, requiring external solutions like OVGME to re-apply the edits.   The process of using OVGME inherently has issues as well, for instance, if the files that were replaced are edited by Eagle Dynamics, you would wipe out those edits by using old versions of the files stored in OVGME.
DCS Alternative Launcher gets around this by verifying the proper edits (single, or multiple lines of code) exist or are made to each of the files when luanching DCS World from this application.  
  
The application has quickly grown into an idea haven for features either sought after or "nice-to-have" that are missing from DCS at present day.   Most games these days have a launcher, the launcher typically has news updates or video content about new features/patches/etc.   Most launchers have the ability to edit the game settings prior to launching the game, and some even have the ability to sync these settings to the cloud in order to be restored at a later time in necessary.  DCS Alternative Laucher aims to do as much of this as possible (with some extra ideas/feedback from you guys)  

**Roadmap**
The current planned and WIP feature list is as follows:  
  
**Handles Multiple Install** (Complete)  
*-auto detect and manually specify*  
**Override Viewport Names (excluding FC3)*** (WIP)  
*-as an example LEFT_MFCD is shared between modules, the launcher will modify the game files to allow A_10C_LEFT_MFCD and FA_18C_LEFT_MFCD to coexist so you dont need to swap monitor configurations just to play a different module*  
*-Ensure all file modifications are made on launch of DCS from the Launcher (this avoids the need for OVGME)*  
**Custom Kneeboard Position** (Planned)  
*-Allow you to define a custom kneeboard position without breaking IC*  
**Cockpit Camera Position** (Planned)  
*-EZ editing of per module default cockpit camera position*  
**Advanced Tweaks (discovering what works and what doesnt takes time)** (WIP)  
*-Disable/Enable Stats Collection*  
*-Enable Debug Console*  
*-Screenshot Quality/Format*  
*-Disable Track Writing*  
*-Silent Crash Reports*  
*-Various Graphics Overrides (currently over 50)*  
*-Various Terrain Camera Overrides (currently over 100)*  
*-Various Terrain Mirror Overrides (currently over 100)*  
*-Various Terrain Reflections (currently over 100)*  
*-Various Terrain MFD Options*  
**Sound** (WIP)  
*-Per module Cockpit World Gain/Lowpass*  
*-Ambient Samples, Radius, Distnace, Time, Height*  
*-Doppler Warp*  
**Counter Measure Profiles** (Planned)  
*-A-10C, Viggen, F-18... etc.*  
**Keybinding setup** (Planned)  
*-Edit all keybindings from inside the launcher*  
**Cloud Syncronization** (Planned)  
*-Save your bindings, settings, cm profiles, etc. to the cloud to allow backup and restore*  

If you are interested in helping test, provide feedback, and /or offer suggestions, please join https://discord.gg/fsd2Bxa

**If you do consider to download and** help out... or to even just look at the app and evaluate it,  **please take into consideration** that the application is **pre-alpha**, so there are **lots of bugs** and it does have the **potential to mess up your settings** or **overwrite files** you have made edits to... so **please** make a **backup of your files** first.  
  
Here are some videos and screenshots of the tool in its current state  
  
**Setup Wizard** *(this walks you through setting up your screen config so it can automatically generate the MonitorConfig lua for you)*  
(https://streamable.com/1hvdn)    
  
**Viewport Editor** *( still a huge wip )*  
(https://streamable.com/rpilg)  

And here are some screenshots  
   
**Main View**  
![Main View](https://media.discordapp.net/attachments/415664512981794818/611234642242306049/unknown.png)  
  
**Viewport Settings**  
![Viewport Settings](https://media.discordapp.net/attachments/415664512981794818/611232715316002836/unknown.png)  
  
**Graphics Settings**  
![Graphics Settings](https://media.discordapp.net/attachments/603005883542142987/609138693189533706/unknown.png)  
  
**Advanced Options** *(This is still a huge WIP, some of these settings are deprecated from ED, but I literally have to check them 1 by 1 to know... so this list may change)*
![Advanced Options](https://media.discordapp.net/attachments/603005883542142987/609139978307502112/unknown.png)  
