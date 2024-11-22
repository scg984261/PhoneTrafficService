# Uninstall PhoneTrafficService
msiexec.exe /x PhoneTrafficService-Release-x86-1.0.0.2.msi /quiet /passive /norestart

# Delete the scheduled task
Unregister-ScheduledTask -TaskName "PhoneTrafficService" -Confirm:$false
