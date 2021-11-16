#include <windows.h>
#include <cpl.h>


char Path[MAX_PATH];


LONG CALLBACK CPlApplet (HWND hwndCPL, UINT uMsg, LPARAM lParam1, LPARAM lParam2)
{
	switch (uMsg)
	{
		case CPL_INIT:
		{
			HANDLE hinst = GetModuleHandle("Types.cpl");
			GetModuleFileName(hinst, Path, MAX_PATH);
			strcpy(Path + strlen(Path) - 3, "exe");
			
			return TRUE;
		}
		
		case CPL_INQUIRE: {
			
			CPLINFO* i = (CPLINFO*) lParam2;
			
			i->idIcon = 1;
			i->idName = 1;
			i->idInfo = 2;
			
		} break;
		
		case CPL_DBLCLK: WinExec(Path, SW_NORMAL); break;
		case CPL_GETCOUNT: return 1;
	}
	
	return 0;
}
