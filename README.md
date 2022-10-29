### Synapse X Remake Overlay UI
- An Overlay UI for Synapse X Remake

### It doesn't open?
- Either the roblox is not running or you didn't attached the WeAreDevsAPI

### How does it work?
- Instead of Injecting the UI into roblox, it retrieves the dimensions of the bounding rectangle of roblox and overlays it.

- ``GetWindowRect([in] HWND, [ref]/[out] rect)`` Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen
     - [GetWindowRect Documentation](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowrect)

- ``IsIconic([in] HWND)`` Determines whether the specified window is minimized (iconic). if it returns true we force our program to minimize windowstate, if it returns false we force our program to normal windowstate
     - [IsIconic Documentation](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-isiconic)

### Preview

- [GIF Preview](https://gyazo.com/0d22a8bbe31209cfb38ba07f498b4fcc)

![image](https://user-images.githubusercontent.com/104715127/198831647-53754919-d145-441c-9c49-fdb2a5d782ed.png)
