Call Send

Sub Send()
Set objShell = WScript.CreateObject("WScript.Shell")
objShell.Run "C:\Users\Git\SMS\SMS.exe ReadSQLTable"
Set objShell = Nothing
end sub