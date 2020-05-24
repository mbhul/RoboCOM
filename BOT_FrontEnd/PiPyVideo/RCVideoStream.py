# import the necessary packages
from threading import Thread
import cv2
import sys
import imutils
import time
from numpy import *
from imutils.convenience import resize
from imutils.video import FPS
from queue import Queue

#This class is a modification of the imutils WebcamVideoStream class
class RCVideoStream:

    def __init__(self, src=0, name="RCVideoStream", width=680, height=480, x_border=0, y_border=0, saveRawVideo=False, filename="output.mp4", targetFPS=20):
        # initialize the video camera stream and read the first frame
        # from the stream
        self.stream = cv2.VideoCapture(src)
        (self.grabbed, self.frame) = self.stream.read()

        # initialize the thread name
        self.name = name
        
        #Default Parameters
        self.frame_height = height
        self.frame_width = width
        self.left_right_border = x_border
        self.top_bottom_border = y_border
        self.lock = False
        self.saveFile = saveRawVideo
        
        #Use the default (pre-resize) dimensions for the file output video writer
        fourcc = cv2.VideoWriter_fourcc(*'mp4v')
        self.out = RCVideoWriter(filename, fourcc, targetFPS, (int(self.stream.get(3)),int(self.stream.get(4)))) 
        
        if saveRawVideo:
            self.out.start()

        # initialize the variable used to indicate if the thread should
        # be stopped
        self.stopped = False

    def start(self):
        # start the thread to read frames from the video stream
        t = Thread(target=self.update, name=self.name, args=())
        t.daemon = True
        t.start()
        
        return self

    def update(self):
        # keep looping infinitely until the thread is stopped
        while True:
            # if the thread indicator variable is set, stop the thread
            if self.stopped:
                if self.saveFile:
                    self.out.stop()
                    
                self.stream.release()
                return

            # otherwise, read the next frame from the stream
            (self.grabbed, temp_frame) = self.stream.read()
            
            if self.saveFile:
                self.out.write(temp_frame)
            
            # Perform resize as atomic operation
            while(self.lock == True):
                pass
                
            self.lock = True
            temp_frame = cv2.resize(temp_frame, (self.frame_width,self.frame_height), cv2.INTER_AREA)
            
            # If a border is specified, create it by centering the frame on a black background
            if(self.left_right_border > 0 or self.top_bottom_border > 0):
                x = self.left_right_border
                y = self.top_bottom_border
                
                #This is basically how cv2.copyMakeBorder works, but with slightly less overhead
                background = zeros((self.frame_height + int(2*y), self.frame_width + int(2*x),3), uint8)
                background[y:y+temp_frame.shape[0], x:x+temp_frame.shape[1]] = temp_frame
                self.frame = background
            else:
                self.frame = temp_frame
                
            self.lock = False

    def resize(self, width=680, height=480, x_border=0, y_border=0):
        #atomic 
        while(self.lock == True):
                pass
        
        #Update frame parameters        
        self.lock = True
        self.frame_width = width
        self.frame_height = height
        self.left_right_border = x_border
        self.top_bottom_border = y_border
        self.lock = False
    
    def read(self):
        # return the frame most recently read
        return self.frame

    def stop(self):
        # indicate that the thread should be stopped
        self.stopped = True
        
        
#Custom wrapper for OpenCV VideoWriter class
class RCVideoWriter:

    def __init__(self, filename, fcc, fps, dim):
        self.fourcc = fcc
        self.fps = fps
        self.name = filename
        self.dim = dim
        self.last_frame = 0
        #self.qsize = 128
        
        #FPS management parameters
        self.avg_fps = 0
        
        self.has_data = False
        self.stopped = False
        #self.Q = Queue(maxsize=self.qsize)
        
    def start(self):
        self.outstream = cv2.VideoWriter(self.name, self.fourcc, self.fps, self.dim)
        
        t = Thread(target=self.update, name="VideoWriter", args=())
        t.daemon = True
        t.start()
        
        self.objfps = FPS().start()
        
        return self
        
    def stop(self):
        self.stopped = True
        
    def update(self):
        
        while(True):
            if self.stopped:
                self.objfps.stop()
                self.avg_fps = self.objfps.fps()
                self.outstream.release()
                return
            
            if self.has_data:
                self.outstream.write(self.last_frame)
                self.objfps.update()
                self.has_data = False
        
    def write(self, frame):
        
        try:
            self.last_frame = frame
            self.has_data = True
            
        except:
            pass

