[![Build status](https://ci.appveyor.com/api/projects/status/ornlq9rscs71pw8t?svg=true)](https://ci.appveyor.com/project/Flysenberg/radiobot)

# RadioBot

Discord music bot written in C# using the Discord.Net Api Wrapper (1.0.2) made by @RogueException.

## Audio playback libraries & programs

Audio playback requires two libraries, `libsodium.dll` and `opus.dll`, found in the official documentation [here](https://discord.foxbot.me/docs/guides/voice/sending-voice.html#installing).

Or [here](https://dsharpplus.emzi0767.com/natives/vnext_natives_win32_x64.zip) if the download link is dead on the official website.

Audio downloading & encoding are handled by [youtube-dl](https://rg3.github.io/youtube-dl/) and [ffmpeg](https://www.ffmpeg.org/).

>The libraries and the programs are not included in this repository, you will have to add them manually to the project.
