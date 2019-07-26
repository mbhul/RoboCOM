import cv2
import time
#from VideoCapture import Device
from numpy import *
from PIL import Image 
from screeninfo import get_monitors

print("Get Camera")
cam = cv2.VideoCapture(0) #, cv2.CAP_DSHOW)
#cam = Device(devnum=0, showVideoWindow=0) 
#image=cam.getImage() #first one supposedly always black

m_list = get_monitors()
print(str(m_list[0].width))

screen_height = m_list[0].height - 100
screen_width = m_list[0].width

print("Read Frames")
while(True):
   ret,frame = cam.read()
   #image=cam.getImage() 
   #frame = asarray(image) #convert the image into a matrix
  
   scale_percent_h = int((float(screen_height) / float(frame.shape[0])) * 100)
   scale_percent_w = int((float(screen_width) / float(frame.shape[1])) * 100)
   
   #scale_percent = 200 # percent of original size
   width = int(frame.shape[1] * scale_percent_h / 100)
   height = int(frame.shape[0] * scale_percent_h / 100)
   dim = (width, height)
   
   x = int((screen_width - width) / 2)
   y = m_list[0].y
   
   # resize image
   resized = cv2.resize(frame, dim, interpolation = cv2.INTER_AREA) 
   
   # frame on larger background
   background = zeros((screen_height,screen_width,3), uint8)
   background[y:y+resized.shape[0], x:x+resized.shape[1]] = resized

   if not frame is None:
      cv2.imshow('frame',background)
      cv2.moveWindow('frame', 0, 0) #moveWindow('frame', x, y)
      
   if (cv2.waitKey(1) & 0xff == ord('q')):
      break

cam.release()
cv2.destroyAllWindows()