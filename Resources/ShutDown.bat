set tracker_computer_ip_address=lcave-tracker:3000

set shoutdown_delay=15


cd /D C:\Windows\System32

start http://%tracker_computer_ip_address%/power/off?token=cave_is_good^&delay=%shoutdown_delay%

shutdown.exe -s -t 3 -c " "

exit
