# monoplug-csgo
DotNet plugin engine for C#

# Dev env

- install linux 32bits

- install srcds in /home/steam/srcds (see : https://developer.valvesoftware.com/wiki/SteamCMD#Linux )
  
Script file for steamcmd :
```
@ShutdownOnFailedCommand 1
@NoPromptForPassword 1
login anonymous
force_install_dir /home/steam/csgo/
app_update 740 validate
quit
```
- game startup script for easly launching
Startup script
```bash
#!/bin/sh
/home/steam/csgo/srcds_run -game csgo -console -usercon +game_type 0 +game_mode 0 +mapgroup mg_active +map de_dust2 +sv_password dtc -debug +sv_hibernate_when_empty 0 +bot_quota_mode fill +bot_join_after_player 0 +log on
```
- install Metamod:Source (see : https://wiki.alliedmods.net/Installing_Metamod:Source )
- check that metamod is functionnal on server with `meta version` in server console
- install dev packages
```
apt-get install mono-complete build-essential git
```
- get hl2sdk
```
cd ~
git clone https://github.com/alliedmodders/hl2sdk.git
```
- switch to branch `csgo`
```
cd ~/hl2sdk
git checkout csgo
```
- get mm:s source
```
cd ~
git clone https://github.com/alliedmodders/metamod-source.git
```
- switch to tag correponding to your metamod version `mmsource-1.10.4`
```
cd ~/metamod-source
git checkout mmsource-1.10.4
```
- get project sources
```
git clone git@github.com:MaitreDede/monoplug-csgo.git
```
- compile
```
cd ~/monoplug-csgo/DotNetPlug/DotNetPlug.Native
make ENGINE=csgo clean && make ENGINE=csgo
cd ~/monoplug-csgo/DotNetPlug/DotNetPlug.Managed
xbuild DotNetPlug.Managed.csproj
```
- create symbolic link to compiled binaries
```
cd ~/csgo/csgo/addons/metamod
ln -s /home/steam/monoplug-csgo/dotnetplug.vdf
mkdir dotnetplug
cd dotnetplug
ln -s /home/steam/monoplug-csgo/DotNetPlug/DotNetPlug.Native/Release.csgo/dotnetplug_native.so
ln -s /home/steam/monoplug-csgo/DotNetPlug/DotNetPlug.Managed/bin/Debug/DotNetPlug.Managed.dll
```
- Start server
- Check that plugin is loaded : `meta list`

### Old x86_64
```
apt-get install mono-complete build-essentials git gcc g++ gcc-multilib g++-multilib clang ia32-libs lib32z1 lib32z1-dev libc6-dev-i386 libc6-i386 lib32stdc++-4.8-dev libmono-2.0-dev:i386
```
