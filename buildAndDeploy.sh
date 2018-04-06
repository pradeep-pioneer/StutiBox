echo "Publishing!"
rm -rf StutiBox/bin/Release/netcoreapp2.0/linux-arm/
dotnet publish /p:MvcRazorCompileOnPublish=false -c release -r linux-arm

echo "Killing the service at 192.168.1.104"
ssh pi@192.168.1.104 "sudo killall StutiBox"

echo "Killing the service at 192.168.1.107"
ssh pi@192.168.1.107 "sudo killall StutiBox"

echo "Purging the existing published file at 192.168.1.104"
ssh pi@192.168.1.104 "rm ~/StutiBox/publish/ -rf"

echo "Purging the existing published file at 192.168.1.107"
ssh pi@192.168.1.107 "rm ~/StutiBox/publish/ -rf"

echo "Copying the published files to 192.168.1.104"
scp -r StutiBox/bin/Release/netcoreapp2.0/linux-arm/publish/ pi@192.168.1.104:~/StutiBox/

echo "Copying the published files to 192.168.1.107"
scp -r StutiBox/bin/Release/netcoreapp2.0/linux-arm/publish/ pi@192.168.1.107:~/StutiBox/

echo "starting the service at 192.168.1.104"
ssh pi@192.168.1.104 "~/StutiBox/publish/StutiBox" &
echo "starting the service at 192.168.1.107"
ssh pi@192.168.1.107 "~/StutiBox/publish/StutiBox" &
exit
