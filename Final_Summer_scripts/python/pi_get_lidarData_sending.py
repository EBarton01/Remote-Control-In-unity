import socket
import json
import threading
import time
import PyLidar3
from queue import Queue

lidar_data_queue = Queue()  # Create a queue to share Lidar data between threads

Unity_HOST = "136.183.246.38"  # The server's hostname or IP address
PORT = 12346  # The port used by the server

def lidar_thread():
    # Serial port to which lidar connected, Get it from device manager windows
    # In Linux type in terminal -- ls /dev/tty* 
    port = "/dev/ttyUSB0"  # Linux
    Obj = PyLidar3.YdLidarX4(port)
    if Obj.Connect():
        print(Obj.GetDeviceInfo())
        gen = Obj.StartScanning()
        t = time.time()  # Start time
        lidar_data_list = []  # List to accumulate lidar data
        while (time.time() - t) < 30:  # Scan for 30 seconds
            lidar_data = next(gen)
            lidar_data_list.append(lidar_data)
            if len(lidar_data_list) == 360:
                # Send data to Unity when we have 360 pairs
                send_data_to_unity(lidar_data_list)
                lidar_data_list.clear()
            time.sleep(0.5)
        Obj.StopScanning()
        Obj.Disconnect()
    else:
        print("Error connecting to device")

def send_data_to_unity(data_list):
    # Assuming you have Lidar data in the format of {angle(0): distance, angle(2): distance, ..., angle(359): distance}
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        try:
            print("Before connecting...")
            s.connect((Unity_HOST, PORT))
            print("Connected...")

            lidar_data = {}
            for item in data_list:
                lidar_data.update(item)

            print("Sending data...")
            s.sendall(json.dumps(lidar_data).encode())
            print("Sent data")

        except Exception as e:
            print("Error sending data:", e)

# Start the threads
lidar_thread = threading.Thread(target=lidar_thread)

lidar_thread.start()

# Wait for the thread to finish (optional)
lidar_thread.join()

# If you want the thread to run indefinitely, you can remove the join() call and add a loop to keep the main thread running.
# while True:
#     time.sleep(1)
