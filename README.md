# Remote-Control-In-unity
Remote control a rover in unity and real life

Python files goes on raspberry pi and sets up the server. C# script can go on camera game asset in unity.
Make sure in unity to assign the variables to corrosponding game objects(buttons, vheicle, etc)

arduino controller controls motors. Pi to arduino via usb

Using YDlidar x4 can now map environment virtually.


----Setup----
Once environment is created, run pi_getlidarData_sending.py. After pi side server is started, run unity code.
