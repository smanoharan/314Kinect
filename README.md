314Kinect
=========

Kinect based Sign Language Interpreter (COMP314 Group Project)


Use Case:
---------
1. Instructors train any sign (or any arbitrary gesture) by performing the sign a few (5 or so) times to the software (in record mode).
2. The software then "learns" the sign and saves it to disk.
3. A student performs the learned sign to the software (in evaluate mode).
4. The software gives feedback (a score out of 10) to the student, based on how well the student performance matched that of the instructor.

Each sign is modelled via HMMs (Hidden Markov Models) using the skeleton frame data provided by the Kinect.
Fingers are not tracked, so 314Kinect is only suitable for signs (and gestures) involving no finger movement.

Requirements:
-------------
* A Microsoft Kinect (XBOX 360 version) must be connected to the PC.
* Microsoft Kinect SDK must be installed.


Libraries Used:
---------------
* Accord.NET (2.2.0), LGPL, http://code.google.com/p/accord/
* AForge.NET (2.1.5), LGPL, http://www.aforgenet.com/


Authors:
-------------
* Sidd Arora (http://github.com/siddarora)
* William Lam 
* Siva Manoharan (http://github.com/smanoharan)
* Mark Will (http://github.com/maw41)

