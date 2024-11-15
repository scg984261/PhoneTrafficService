# Run the MSI Installer script.
msiexec.exe /i PhoneTrafficService-Debug-x86-2.0.0.8.msi SP_RUNMODE="ALBANY_HOUSE" SP_INCOMING_PATH="C:\ELEPHANT\AHINCOMING.CSV" SP_SPREADSHEET_PATH=`"C:\PhoneNumbersAllocated\AH Phone Numbers Allocated.xls`" /quiet /passive /norestart

# Create the Scheduled Task to run automatically.
$Action = New-ScheduledTaskAction -Execute "C:\PhoneTrafficService\PhoneTrafficService.exe" -
$Trigger = New-ScheduledTaskTrigger -Daily -At '2:00 AM'
$Settings = New-ScheduledTaskSettingsSet
$Task = New-ScheduledTask -Action $Action -Trigger $Trigger -Settings $Settings
Register-ScheduledTask -TaskName "PhoneTrafficService" -InputObject $Task
