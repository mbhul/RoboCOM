import cv2
import time
import imutils
from numpy import *
from PIL import Image 
from screeninfo import get_monitors
from imutils.video import WebcamVideoStream
from imutils.video import FPS

print("Get Camera")
cam = cv2.VideoCapture(0)

m_list = get_monitors()
print(str(m_list[0].width))

screen_height = m_list[0].height - 100
screen_width = m_list[0].width
background = zeros((screen_height,screen_width,3), uint8)

print("Read Frames")
vs = WebcamVideoStream(src=0).start()
fps = FPS().start()

while(True):
   #ret,frame = cam.read()
   frame = vs.read()
  
   scale_percent_h = int((float(screen_height) / float(frame.shape[0])) * 100)
   scale_percent_w = int((float(screen_width) / float(frame.shape[1])) * 100)
   
   #scale_percent = 200 # percent of original size
   width = int(frame.shape[1] * scale_percent_h / 100)
   height = int(frame.shape[0] * scale_percent_h / 100)
   dim = (width, height)
   
   x = int((screen_width - width) / 2)
   y = m_list[0].y
   
   # resize image
   #resized = cv2.resize(frame, dim, interpolation = cv2.INTER_AREA) 
   resized = imutils.resize(frame, width)
   
   # frame on larger background
   #background[y:y+frame.shape[0], x:x+frame.shape[1]] = frame
   background[y:y+resized.shape[0], x:x+resized.shape[1]] = resized

   if not frame is None:
      #cv2.imshow('RCVideo',frame)
      cv2.imshow('RCVideo',background)
      cv2.moveWindow('RCVideo', 0, 0) #moveWindow('frame', x, y)
      
   if (cv2.waitKey(1) & 0xff == ord('q')):
      break
   
   fps.update()

fps.stop()
print("[INFO] elasped time: {:.2f}".format(fps.elapsed()))
print("[INFO] approx. FPS: {:.2f}".format(fps.fps()))
    
cam.release()
cv2.destroyAllWindows()