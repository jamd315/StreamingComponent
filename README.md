# StreamingComponent

## Basic Usage

0. Download the latest release from the [releases page](https://github.com/jamd315/StreamingComponent/releases/latest)
1. Import the .rslib file into your station.  You'll have to allow code to run.
2. Drag the StreamingComponent onto a part you want to track, and select that you do want to update the position.
3. Update the port if needed by right clicking the StreamingComponent and selecting Properties.
4. Start the simulation, and the position of the part will be streamed to the port you selected on localhost.

To monitor for testing, use `nc` or `ncat` like `nc -l -u 127.0.0.1 5000`

##### Future plans
This works alright for what I need it to do so I probably won't update it too much.  If you need additional features, feel free to modify the source yourself or submit an issue on github and I'll probably implement it if it's simple.
Building isn't too hard, just install RobotStudio SDK and update the paths in the .csproj and add some .dll references.