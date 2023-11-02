import socket
import time
import arduino_controller

rover = arduino_controller.controller()
speed = 4  

def start_server():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.bind(('0.0.0.0', 33102)) 
    server_socket.listen(1)
    print("Server started! Waiting for connections...")

    client_socket, address = server_socket.accept()
    print(f"Client connected from {address}")
    
    if rover.start==False :
        rover.startR()

    try:
        #is currently swap-ping between listining and sending
        while True:
            received_data = client_socket.recv(1024).decode()
            if received_data:
                if received_data == "Forward":
                    print(f"Received message from Unity: {received_data}")
                    rover.setspeed(speed)
                    rover.setaction(speed,1)
                    time.sleep(0.25)
                    rover.setaction(speed,5)
            
                    msg = "F"
                    client_socket.sendall(msg.encode())
                    
                elif received_data == "Reverse":
                    print(f"Received message from Unity: {received_data}")
                    rover.setspeed(speed)
                    rover.setaction(speed,2)
                    time.sleep(0.25)
                    rover.setaction(speed,5)
            
                    msg = "Rev"
                    client_socket.sendall(msg.encode())
                    
                elif received_data == "Left":
                    print(f"Received message from Unity: {received_data}")
                    rover.setspeed(speed)
                    rover.setaction(speed,3)
                    time.sleep(0.25)
                    rover.setaction(speed,5)
            
                    msg = "L"
                    client_socket.sendall(msg.encode())
                    
                elif received_data == "Right":
                    print(f"Received message from Unity: {received_data}")
                    rover.setspeed(speed)
                    rover.setaction(speed,4)
                    time.sleep(0.25)
                    rover.setaction(speed,5)
            
                    msg = "Right"
                    client_socket.sendall(msg.encode())

            #time.sleep(1)
                
    except KeyboardInterrupt:
        rover.stopR()
        print("Closing connection")
        client_socket.close()
        server_socket.close()

#start_server()