314Kinect
=========

Kinect based Sign Language Interpreter (COMP314 Group Project)

Requires a Microsoft Kinect (XBOX 360 version) to be connected to run.
Requires the Microsoft Kinect SDK to be installed.

Allows instructors to train signs (or any arbitrary gesture) 
by performing the sign a few (5 or so) times to the 314Kinect program (in record mode).
The 314Kinect program then "learns" the sign.
Any student can perform the learned sign to the 314Kinect program (in evaluate mode)
and the program will give feedback (a score out of 10) regarding how closely the 
student performance of the sign matched that of the instructors.

Each sign is modelled via  HMMs (Hidden Markov Models) by using only the 
skeleton frame data provided by the Kinect.
Fingers are not tracked, so 314Kinect is only suitable for signs (and gestures)
involving no finger movement.

Authors:
-------------
Sidd Arora
William Lam
Siva Manoharan
Mark Will

