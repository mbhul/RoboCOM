The scripts in this folder are optomized for video capture and writing to file on
a Raspberry Pi 3.

In order to maximize throughput, when the option to save the video to file is selected,
the RCVideoStream will write directly to the RCVideoRecorder the unprocessed frame.

In addition, the on screen display of the captured video is done in a separate thread in order to minimize latency between frames. 
