msbuild PhoneTrafficService\PhoneTrafficService.csproj /t:Rebuild -property:configuration=Release;Platform=x86
msbuild PhoneTrafficServiceTest\PhoneTrafficServiceTest.csproj /t:Rebuild -property:configuration=Release;Platform=x86
msbuild PhoneTrafficServiceInstaller\PhoneTrafficServiceInstaller.wixproj /t:Rebuild -property:configuration=Release;Platform=x86
