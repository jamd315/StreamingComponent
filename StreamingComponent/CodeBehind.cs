using ABB.Robotics.Math;
using ABB.Robotics.RobotStudio;
using ABB.Robotics.RobotStudio.Stations;
using RobotStudio.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace StreamingComponent
{
    /// <summary>
    /// Code-behind class for the StreamingComponent Smart Component.
    /// </summary>
    /// <remarks>
    /// The code-behind class should be seen as a service provider used by the 
    /// Smart Component runtime. Only one instance of the code-behind class
    /// is created, regardless of how many instances there are of the associated
    /// Smart Component.
    /// Therefore, the code-behind class should not store any state information.
    /// Instead, use the SmartComponent.StateCache collection.
    /// </remarks>
    public class CodeBehind : SmartComponentCodeBehind
    {

        public override void OnPropertyValueChanged(SmartComponent component, DynamicProperty changedProperty, object oldValue)
        {
            if (changedProperty.Name.Equals("Port"))
            {
                int newPort = (int)changedProperty.Value;
                if (newPort < 1024 || newPort > 65535)
                {
                    Logger.AddMessage(new LogMessage("Port out of range, using default port of 5000", LogMessageSeverity.Warning));
                    newPort = 5000;
                }
                component.StateCache["port"] = newPort;
                Logger.AddMessage(new LogMessage($"StreamingComponent port updated to {newPort}"));
            }
            base.OnPropertyValueChanged(component, changedProperty, oldValue);
        }

        public override void OnSimulationStart(SmartComponent component)
        {
            Logger.AddMessage(new LogMessage("StreamingComponent starting"));

            if (!component.StateCache.ContainsKey("port"))
            {
                component.StateCache.Add("port", 5000);
                Logger.AddMessage(new LogMessage("StreamingComponent using default port 5000"));
            }

            int port = (int)component.StateCache["port"];
            component.StateCache["endpoint"] = new IPEndPoint(IPAddress.Loopback, port);
            component.StateCache["client"] = new UdpClient();
            component.StateCache["frame"] = 0;

            // TODO I think this isn't persistent

            base.OnSimulationStart(component);
        }

        /// <summary>
        /// Called during simulation.
        /// </summary>
        /// <param name="component"> Simulated component. </param>
        /// <param name="simulationTime"> Time (in ms) for the current simulation step. </param>
        /// <param name="previousTime"> Time (in ms) for the previous simulation step. </param>
        public override void OnSimulationStep(SmartComponent component, double simulationTime, double previousTime)
        {
            UdpClient client = (UdpClient)component.StateCache["client"];
            IPEndPoint endpoint = (IPEndPoint)component.StateCache["endpoint"];
            int frame = (int)component.StateCache["frame"];
            component.StateCache["frame"] = frame + 1;

            // Build message
            Transform t = component.Transform;
            string message = $"fr {frame}\nts {simulationTime}\n3d 1 [1 1.000][{t.X:0.000},{t.Y:0.000},{t.Z:0.000}]\n";
            byte[] data = Encoding.ASCII.GetBytes(message);

            // Broadcast position
            client.Send(data, data.Length, endpoint);;
        }

        public override void OnSimulationStop(SmartComponent component)
        {
            Logger.AddMessage(new LogMessage("StreamingComponent stopping"));

            UdpClient client = (UdpClient)component.StateCache["client"];
            client.Close();
            client.Dispose();
            base.OnSimulationStop(component);
        }
    }
}
