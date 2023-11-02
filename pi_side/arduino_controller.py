
#https://www.raspberrypi-spy.co.uk/2020/12/install-arduino-ide-on-raspberry-pi/


#!/usr/bin/env python3       
   

import pyfirmata                
import time
#Basic GUI with increments
from threading import Thread

class controller:
    def __init__(self):
        self.start = False
        self.action=""
        self.speed=0
        
        self.board =   pyfirmata.Arduino('/dev/ttyACM0')
        print("Communication Successfully started")
        
      
        
        ##setup pwm signal for motor 1
        self.motor1 = self.board.digital[3]
        self.motor1.mode = pyfirmata.PWM
        
        ##setup digital signal for motor direction  
        self.m1_clockwise = self.board.digital[4]
        self.m1_counterclockwise = self.board.digital[2]

        ##setup pwm signal for motor 2
        self.motor2 = self.board.digital[5]
        self.motor2.mode = pyfirmata.PWM
        
        ##setup digital signal for motor direction  
        self.m2_clockwise = self.board.digital[8]
        self.m2_counterclockwise = self.board.digital[7]
        
        ##setup pwm signal for motor 3
        self.motor3 = self.board.digital[6]
        self.motor3.mode = pyfirmata.PWM
        
        ##setup pwm signal for motor 4
        self.motor4 = self.board.digital[9]
        self.motor4.mode = pyfirmata.PWM
        
        ##setup pwm signal for motor 5
        self.motor5 = self.board.digital[10]
        self.motor5.mode = pyfirmata.PWM
        
        ##setup pwm signal for motor 6
        self.motor6 = self.board.digital[11]
        self.motor6.mode = pyfirmata.PWM
        
        

    # long-running background task
    def background_task(self):
        #global data
        # record the last seen value
        #last_seen = data
        # run forever
        while (self.start):
            # check for change
            if self.action == "Forward":
                # report the change
                print(self.action, self.speed)
                
                self.motor1.write(self.speed*0.2)
                self.m1_clockwise.write(1)
                self.m1_counterclockwise.write(0)
                
                self.motor2.write(self.speed*0.2)
                self.m2_clockwise.write(1)
                self.m2_counterclockwise.write(0)
                
                self.motor3.write(self.speed*0.2)
                self.motor4.write(self.speed*0.2)
                self.motor5.write(self.speed*0.2)
                self.motor6.write(self.speed*0.2)
                
                
                # update last seen
            if self.action == "Backward":    
                print(self.action, self.speed)
                self.motor1.write(self.speed*0.2)
                self.m1_clockwise.write(0)
                self.m1_counterclockwise.write(1)
                
                self.motor2.write(self.speed*0.2)
                self.m2_clockwise.write(0)
                self.m2_counterclockwise.write(1)
                
                self.motor3.write(self.speed*0.2)
                self.motor4.write(self.speed*0.2)
                self.motor5.write(self.speed*0.2)
                self.motor6.write(self.speed*0.2)
                
            if self.action == "Turnleft":
                print(self.action, self.speed)
                
                self.m1_clockwise.write(1)
                self.m1_counterclockwise.write(0)
                
                self.m2_clockwise.write(0)
                self.m2_counterclockwise.write(1)
                
                self.motor1.write(self.speed*0.2)
                self.motor2.write(self.speed*0.2)
                self.motor3.write(self.speed*0.2)
                self.motor4.write(self.speed*0.2)
                self.motor5.write(self.speed*0.2)
                self.motor6.write(self.speed*0.2)
                
                
                #while loop for turn action
                #set to foward
            if self.action == "Turnright":    
                print(self.action, self.speed)
                
                self.m1_clockwise.write(0)
                self.m1_counterclockwise.write(1)
                
                self.m2_clockwise.write(1)
                self.m2_counterclockwise.write(0)
                
                
                self.motor1.write(self.speed*0.2)
                self.motor2.write(self.speed*0.2)
                self.motor3.write(self.speed*0.2)
                self.motor4.write(self.speed*0.2)
                self.motor5.write(self.speed*0.2)
                self.motor6.write(self.speed*0.2)
                
            if self.action == "Stop":    
                print(self.action, self.speed)
                
                self.motor1.write(0)
                self.m1_counterclockwise.write(0)        
                self.m1_clockwise.write(0)
        
                self.motor2.write(0)
                self.m2_counterclockwise.write(0)        
                self.m2_clockwise.write(0)
        
                self.motor3.write(0)
                self.motor4.write(0)
                self.motor5.write(0)
                self.motor6.write(0)
                
                
                
               
            # block for a while
            #time.sleep(0.5)

        
    def setaction(self, speed, direct):
        print ("in controller ", direct)
        if (direct==1) :
            self.action="Forward"
        if (direct==2) :
            self.action="Backward"            
        if (direct==3) :
            self.action="Turnleft"
        if (direct==4) :
            self.action="Turnright"
        if (direct==5) :
            self.action="Stop"
            
 #       match direct:
 #           case 1:
 #               self.action="Forward"
 #           case 2:
 #               self.action="Backward"
 #           case 3:
 #               self.action="Turnleft"
 #           case 4:
 #               self.action="Turnright"           
 #           case default:
 #               self.action=""
            
        
 #       self.speed=speed
        #while (not self.stop):
            #self.motor.write(speed)
            #self.direction.write(direction) 
         #   print(speed,direction)
            #time.sleep(1)


    def setspeed(self, speed):
        self.speed=speed
        
    #def backward(self, speed, direction):
        #while (not self.stop):
        #    self.motor.write(speed)
        #    self.direction.write(direction) 
  
    #def turnleft(self):
        #while (not self.stop):
        #    self.motor.write(speed)
        #    self.direction.write(direction) 

    #def turnright(self, speed, direction):
        #while (not self.stop):
        #    self.motor.write(speed)
        #    self.direction.write(direction)
            
    def startR(self):
        self.start= True
        print('Starting Rover background task...')
        daemon = Thread(target=self.background_task, daemon=True, name='Rover')
        daemon.start()          

            
    def stopR(self):
        self.start = False
        self.action=""
        self.motor1.write(0)
        self.m1_counterclockwise.write(0)        
        self.m1_clockwise.write(0)
        
        self.motor2.write(0)
        self.m2_counterclockwise.write(0)        
        self.m2_clockwise.write(0)
        
        self.motor3.write(0)
        self.motor4.write(0)
        self.motor5.write(0)
        self.motor6.write(0)
        
