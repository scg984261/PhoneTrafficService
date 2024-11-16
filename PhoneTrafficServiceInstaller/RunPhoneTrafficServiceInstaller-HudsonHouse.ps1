# Run the Installer.
msiexec.exe /i PhoneTrafficService-Debug-x86-1.0.0.0.msi SP_RUNMODE="HUDSON_HOUSE" SP_INCOMING_PATH="C:\ELEPHANT\HHINCOMING.CSV" SP_SPREADSHEET_PATH=`"C:\PhoneNumbersAllocated\HH Phone Numbers Allocated.xls`" /quiet /passive /norestart

# Create the Scheduled Task to run automatically
$Action = New-ScheduledTaskAction -Execute "C:\PhoneTrafficService\PhoneTrafficService.exe" -
$Trigger = New-ScheduledTaskTrigger -Daily -At '20:45 PM'
$Settings = New-ScheduledTaskSettingsSet
$Task = New-ScheduledTask -Action $Action -Trigger $Trigger -Settings $Settings
Register-ScheduledTask -TaskName "PhoneTrafficService" -InputObject $Task