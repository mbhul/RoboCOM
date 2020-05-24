import sys
import cv2
import time
import imutils
from threading import Thread
from numpy import *
#from PIL import Image 
from screeninfo import get_monitors
from imutils.video import FPS
from RCVideoStream import RCVideoStream
from RCVideoStream import RCVideoWriter

class VideoShow:
    """
    Class that continuously shows a frame using a dedicated thread.
    """
    def __init__(self, frame=None):
        self.frame = frame
        self.stopped = False

    def start(self):
        Thread(target=self.show, args=()).start()
        self.fps = FPS().start()
        return self

    def show(self):
        while not self.stopped:
            cv2.imshow("Video", self.frame)
            self.fps.update()
            if cv2.waitKey(1) == ord("q"):
                self.stopped = True

        self.fps.stop()
        print("Display FPS: ", self.fps.fps())
        
    def stop(self):
        self.stopped = True

###########################
# main
###########################
print("Start Capture")

m_list = get_monitors()
if (len(m_list) < 1):
    screen_height = 480
    screen_width = 1200
else:
    print("Screen Width: ", str(m_list[0].width))
    print("Screen Height: ", str(m_list[0].height))
    screen_height = m_list[0].height - 200
    screen_width = m_list[0].width

if len(sys.argv) > 1:
    save_vid_stream = bool(sys.argv[1])
else:
    save_vid_stream = False

print("Read Frames")
vs = RCVideoStream(src=0,saveRawVideo=save_vid_stream,targetFPS=60).start()
time.sleep(1)

frame_counter = 0

#Calculate the desired frame size once
frame = vs.frame
video_shower = VideoShow(frame).start()

#ratio of screen height to frame height
scale_percent_h = int((float(screen_height) / float(frame.shape[0])) * 100)

#new width scaled by percentage required to reach full screen height
width = int(frame.shape[1] * scale_percent_h / 100)

x = int((screen_width - width) / 2)
vs.resize(width,screen_height,x)
time.sleep(0.1)

#Note: dimensions of the VideoWriter have to be EXACTLY the same size as the frame
#if save_vid_stream == True:
#    print("Open file to save the video stream")
#    fourcc = cv2.VideoWriter_fourcc(*'MJPG')
#    out = RCVideoWriter('output.mp4', 0x6c, 20, (frame.shape[1], frame.shape[0])) #((width + (2*x)),screen_height)) #(int(vs.stream.get(3)),int(vs.stream.get(4))))
#    out.start()

while(True):
    
    if video_shower.stopped:
        video_shower.stop()
        break
    
    frame = vs.frame
    video_shower.frame = frame
    time.sleep(0.017)
    
#END WHILE

time.sleep(2)
vs.stop()

if save_vid_stream == True:
    print("Stopping Video Writer")
    time.sleep(2)
    print("Effective Output FPS: ", vs.out.avg_fps)
   
cv2.destroyAllWindows()