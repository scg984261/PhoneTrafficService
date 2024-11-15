# Uninstall PhoneTrafficService
msiexec.exe /x PhoneTrafficService-Debug-x86-2.0.0.8.msi /quiet /passive /norestart

# Delete the scheduled task
Unregister-ScheduledTask -TaskName "PhoneTrafficService" -Confirm:$false
