import sys
import cv2
import time
import imutils
from numpy import *
#from PIL import Image 
from screeninfo import get_monitors
from imutils.video import FPS
from RCVideoStream import RCVideoStream
from RCVideoStream import RCVideoWriter

print("Start Capture")

m_list = get_monitors()
print("Screen Width: ", str(m_list[0].width))

screen_height = m_list[0].height - 200
screen_width = m_list[0].width

if len(sys.argv) > 1:
    save_vid_stream = bool(sys.argv[1])
else:
    save_vid_stream = False

print("Read Frames")
vs = RCVideoStream(src=0).start()
fps = FPS().start()

frame_counter = 0

#Calculate the desired frame size once
frame = vs.read()

#ratio of screen height to frame height
scale_percent_h = int((float(screen_height) / float(frame.shape[0])) * 100)

#new width scaled by percentage required to reach full screen height
width = int(frame.shape[1] * scale_percent_h / 100)

x = int((screen_width - width) / 2)
vs.resize(width,screen_height,x)
time.sleep(0.1)

#Note: dimensions of the VideoWriter have to be EXACTLY the same size as the frame
if save_vid_stream == True:
    print("Open file to save the video stream")
    fourcc = cv2.VideoWriter_fourcc(*'mp4v')
    out = RCVideoWriter('output.mp4', fourcc, 60, ((width + (2*x)),screen_height))
    out.start()

while(True):
    frame = vs.read()

    if not frame is None:
        #Write frame to video 
        if save_vid_stream == True:
             out.write(frame)
             
        cv2.imshow('RCVideo',frame)
        cv2.moveWindow('RCVideo', 0, 0)

    #time.sleep(0.005)
    
    if (cv2.waitKey(1) & 0xff == ord('q')):
        break
    
    fps.update()
    
#END WHILE
vs.stop()
fps.stop()

if save_vid_stream == True:
    out.stop()
    print("Stopping Video Writer")
    time.sleep(1)
    print("Effective Output FPS: ", out.avg_fps)

#print("[INFO] elasped time: {:.2f}".format(fps.elapsed()))
print("[INFO] approx. FPS: {:.2f}".format(fps.fps()))
 
cv2.destroyAllWindows()