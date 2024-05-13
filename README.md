# AFExplorer - Preview
> [!WARNING]
> IF ANY DATA LOSS IS OCCURED, I WILL NOT BE RESPONSIBLE FOR ANY KIND OF DAMAGE THIS PROGRAM DOES TO YOURS.
>
## This program is intended to run on Windows 10 22H2 and up.
> Why you might ask? Because some features will break below Windows 10 21H2, but you can try expriment that yourself and see if running this program on windows 10 or below will break or not.

> [!IMPORTANT]
> This program runs on .NET 9.0.x, install [.NET SDK 9.0 Here.](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or the program will not run.

## What does this program even used for?
> This program can help you navigate your Android OS by using ADB (or Android Debug Bridge if you want to get nerdy). Basically it can move, copy, or sometimes delete files/folder from your android storage, but remember,  be careful what you're deleting because sometimes you think it isn't important and it actually is, so check before deleting

## I have downloaded the program, what should I do?
> When you first started the program, you'll see a screen like this:
![[When settings.ini not configured yet]]([https://i.imgur.com/hWrHaJJ.jpeg])
> First thing first, navigate to the folder and create a new file called  `settings.ini` and paste this code
```ini
[Android Debug Bridge Config]
path="<replace this with your own path config>"
version="2.1.3"
```
> For the path, make sure that you copy a single line of path when the program is running or type it by hand when the program is closed because :
> 1. The program has no delay, which mean it will autmatically entered what do you put in the path
> 2. This usually can break some stuff

