# Download link
## [You can download it directly here](https://github.com/Hobospider132/MahiruMate/releases/download/beta-2/MahiruMate-beta-2.2.exe) Note: This is a WINDOWS only program. Also, please read the README fully. There's no technical jargon and it will answer a lot of potential questions, sorry there is so much though, I am an overthinker and a yapper so please bear with me 🙏
<br><br>
However, if you wish to build please use the following build command after navigating to the route directory:
<br>```dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:DebugType=None /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true```
<br>Next navigate to ```MahiruMate-main\MahiruMate-main\bin\Release\net8.0-windows\win-x64\publish``` where you should find the built version
<br>You will need .NET 8.0 Windows to build it as this is a WPF

# Where's Mahiru?
I'm still waiting on the animations so in the meantime, I've included an image of Indi's balls instead.
This is a beta currently so please let me know if you encounter bugs. My email is Hobospider132@gmail.com
</br><a href="https://x.com/Hobospider132">Twitter</a>
</br><a href="https://www.discord.com/users/649892152398315540">Discord</a>
</br>I'm also mainly active in this <a href="https://discord.gg/otonari">discord server</a> if you wish to reach me here.

# Ok, I meant where's Mahiru as in she disappeared off my screen
If she (or rather Indi's balls) has disappeared off your screen it is 1 of 3 possibilities. 
- Bug
- Crashed
- Hunger, thirst or happiness got too low.

99% of the time she's disappeared off your screen due to the third reason. Check her stats from time to time and do the corresponding action to raise her moods. If they reach 0, she will leave. You can toggle this on and off in the pet menu and then you can drag her all you want without worrying about her disappearing or worrying about hunger and thirst

If she's gone and it was not due to a level dropping too low, please either open an issue or message me on a provided contact. Please provide details such as:

- Your specific operating system (Windows 7, Windows 10, Windows 11, etc)
- What you were doing when she disappeared
- What your system specifications are

# I'm a little worried, a single standalone exe? 
I did this to make the program easier to use for those who are not as technically capable. The exe installs nor downloads ***ANYTHING*** to the users computer. It is fully portable amd lightweight

# What does it do currently?
- Can play pong
- Walks randomly
- Has random dialogue
- Can be dragged around the screen
- Fixed intial response to food but random post responses
- Hunger, thirst and happiness systems
- Hunger, thirst and happiness can be toggled on and off

# What are the plans for the future?
The future plans include:
- Further dialogue
- ~~Proper animations for idling, walking and dragging~~
- May use 3D models instead for the animations rather than pixel art ones
- Menu optimisation

Open to suggestions as well so please message me ideas. I will consider them all.

# I love Indi's balls. What will happen to his balls after animations are made?
Don't worry, I will be releasing an alternative version of each release where Indi's balls will be the pet.
However, there will be ***NO*** animations for that version, it will ***PURELY*** be the same image seen
in these beta releases

# I hate feature creep, will this succumb to feature creep? Oh please tell me there won't be any feature creep

Relax, if I stay locked in (unlikely) and if I stick to the set plan (maybe likely) the project will never succumb to feature creep. I have already ensured that technical debt will not accumulate and previously refactored the code for readability and ease of modificaton. Feature creep will (most likely) not be an issue


# Demo:

Main features showcase: <video src="https://github.com/user-attachments/assets/a8f586fb-70a9-4562-9656-c658dd4d5ed3"></video>

Movement showcase: <video src="https://github.com/user-attachments/assets/7abdf991-2c8d-437d-9a51-1a54bfcf6564"></video>

More features are inside, not everything has been included in the demo. Demo is also based on the first beta release.
I have not been bothered to record further videos. This is just to give a general idea of what to expect
