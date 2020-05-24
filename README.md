# ObsOverwatch
.net Topshelf service that monitors obs streaming and takes actions when configured (restart stream etc).

#What does it do
This service monitors the strains of a stream, and restarts the stream if the avegare strains are above a certain level.

#Usage
I created this because we had a computer that ran a stream 24/7 unattended. This computer was behind a PfSense that had multiple connections, and when a connection was down, the host PC already connected to the internet (through the new connection), but OBS could not "understand" that the connection should restart. This service, monitors the strain of the stream, and if it reaches 0.9 (range of 0.0 - 1.0), it closes the stream, waits some seconds, and then restarts the stream. All of the parameters, how many consecutive strains it should take the average from, how many seconds to wait between restart etc, are configurable in the "configuration.json" file.
