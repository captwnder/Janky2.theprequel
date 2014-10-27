using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.ComponentModel;

namespace Otter {
    /// <summary>
    /// Component for connecting to and interacting with an IRC server.
    /// </summary>
    public class IRC : Component {

        public string
            Server,
            Nick,
            Pass,
            Name = "Otter Bot";

        public int Port;

        public bool Debug = true;

        List<string> channels = new List<string>();

        TcpClient connection;
        NetworkStream networkStream;
        StreamReader streamReader;
        StreamWriter streamWriter;

        public bool Connected = false;

        public bool Running = true;

        public BackgroundWorker BackgroundWorker = new BackgroundWorker();
        
        public IRC(string server, int port = 6667, string nick = "otter_bot", string pass = null) {
            Server = server;
            Nick = nick;
            Port = port;
            Pass = pass;
        }

        public void Join(string channel, string password = null) {
            if (!Connected) return;

            if (!channels.Contains(channel)) {
                channels.Add(channel);
                SendData("JOIN", channel);
            }
        }

        public void Part(string channel) {
            channels.Remove(channel);
        }

        public void Close() {
            if (Connected) {
                streamReader.Close();
                streamWriter.Close();
                networkStream.Close();
                connection.Close();
                Connected = false;
                Running = false;
            }
        }

        public void Connect() {
            try {
                connection = new TcpClient(Server, Port);
            }
            catch {
                if (Debug) Console.WriteLine("Connection Error");
            }

            try {
                networkStream = connection.GetStream();
                streamReader = new StreamReader(networkStream);
                streamWriter = new StreamWriter(networkStream);
            }
            catch {
                if (Debug) Console.WriteLine("Communication Error");
            }
            finally {
                Connected = true;

                if (Pass != null) {
                    SendData("PASS", Pass);
                }
                SendData("USER", Nick + " something something " + Name);
                SendData("NICK", Nick);
                
                BackgroundWorker.DoWork += Work;
            }
        }

        public void SendData(string command, string param = null) {
            if (!Connected) return;

            if (param == null) {
                streamWriter.WriteLine(command);
            }
            else {
                streamWriter.WriteLine(command + " " + param);
            }
            streamWriter.Flush();
            if (Debug) Console.WriteLine("Sent: " + command + " " + param);

        }

        public void Work(object sender, DoWorkEventArgs e) {

            if (Connected) {
                string data;
                if (Running) {
                    
                    data = streamReader.ReadLine();
                    if (Debug) Console.WriteLine("IRC> " + data);
                    if (data != null) {
                        if (data.Substring(0, 4) == "PING") {
                            SendData("PONG");
                        }
                    }
                    else {
                        //Assume null data is disconnect?
                        Close();
                    }
                }
            }
        }

        public override void Added() {
            base.Added();
        }

        public override void Update() {
            base.Update();
            if (!BackgroundWorker.IsBusy) {
                BackgroundWorker.RunWorkerAsync();
            }

        }

        public override void Removed() {
            base.Removed();
        }
    }
}
