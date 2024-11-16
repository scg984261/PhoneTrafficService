# Uninstall PhoneTrafficService
msiexec.exe /x PhoneTrafficService-Debug-x86-1.0.0.1.msi /quiet /passive /norestart

# Delete the scheduled task
Unregister-ScheduledTask -TaskName "PhoneTrafficService" -Confirm:$false
