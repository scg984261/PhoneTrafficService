# Run the MSI Installer script.
msiexec.exe /i PhoneTrafficService-Release-x86-1.0.0.2.msi SP_RUNMODE="ALBANY_HOUSE" SP_INCOMING_PATH="C:\ELEPHANT\INCOMING.csv" SP_SPREADSHEET_PATH=`"C:\Share\General\Telephone System\AH Phone Numbers Allocated.xls`" /quiet /passive /norestart

# Create the Scheduled Task to run automatically.
$Action = New-ScheduledTaskAction -Execute "C:\PhoneTrafficService\PhoneTrafficService.exe" -
$Trigger = New-ScheduledTaskTrigger -Daily -At '02:00 AM'
$Settings = New-ScheduledTaskSettingsSet
$Task = New-ScheduledTask -Action $Action -Trigger $Trigger -Settings $Settings
Register-ScheduledTask -TaskName "PhoneTrafficService" -InputObject $Task
