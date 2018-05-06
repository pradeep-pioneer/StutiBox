echo "Publishing!"

rm -rf StutiBox/bin/Release/netcoreapp2.0/linux-arm/
dotnet publish /p:MvcRazorCompileOnPublish=false -c release -r linux-arm

echo "attempting to stop the service on 200"
ssh pi@192.168.1.200 "sudo systemctl stop Stutibox.service"

echo "attempting to stop the service on 201"
ssh pi@192.168.1.201 "sudo systemctl stop Stutibox.service"

echo "attempting to disable the service on 200"
ssh pi@192.168.1.200 "sudo systemctl disable Stutibox.service"

echo "attempting to disable the service on 201"
ssh pi@192.168.1.201 "sudo systemctl disable Stutibox.service"

echo "attempting to remove the service unit file on 200"
ssh pi@192.168.1.200 "sudo rm /lib/systemd/system/Stutibox.service"

echo "attempting to remove the service unit file on 201"
ssh pi@192.168.1.201 "sudo rm /lib/systemd/system/Stutibox.service"

echo "Purging the existing published file at 192.168.1.200"
ssh pi@192.168.1.200 "rm ~/StutiBox/publish/ -rf"

echo "Purging the existing published file at 192.168.1.201"
ssh pi@192.168.1.201 "rm ~/StutiBox/publish/ -rf"

echo "Copying the published files to 192.168.1.200"
scp -r StutiBox/bin/Release/netcoreapp2.0/linux-arm/publish/ pi@192.168.1.200:~/StutiBox/

echo "Copying the published files to 192.168.1.201"
scp -r StutiBox/bin/Release/netcoreapp2.0/linux-arm/publish/ pi@192.168.1.201:~/StutiBox/

echo "Copying the service files to designated location on 200"
ssh pi@192.168.1.200 sudo cp "/home/pi/StutiBox/publish/Stutibox.service" "/lib/systemd/system/Stutibox.service"
echo "Enabling the service on 200"
ssh pi@192.168.1.200 sudo systemctl daemon-reload
ssh pi@192.168.1.200 sudo systemctl enable Stutibox.service
echo "Starting the service on 200"
ssh pi@192.168.1.200 sudo systemctl start Stutibox.service


echo "Copying the service files to designated location on 201"
ssh pi@192.168.1.201 sudo cp "/home/pi/StutiBox/publish/Stutibox.service" "/lib/systemd/system/Stutibox.service"
echo "Enabling the service on 201"
ssh pi@192.168.1.200 sudo systemctl daemon-reload
ssh pi@192.168.1.201 sudo systemctl enable Stutibox.service
echo "Starting the service on 201"
ssh pi@192.168.1.201 sudo systemctl start Stutibox.service


