using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using Google.Protobuf.WellKnownTypes;
using Server.Data;
using Server.Game;
using ServerCore;

namespace Server
{
	class Program
	{
		static Listener _listener = new Listener();

		static void GameLogicTask()
		{
			while (true)
			{
				GameLogic.Instance.Update();
				Thread.Sleep(0);
			}
		}

		static void NetworkTask()
		{
			while (true)
			{
				List<ClientSession> sessions = SessionManager.Instance.GetSessions();
				foreach (ClientSession session in sessions)
				{
					session.FlushSend();
				}

				Thread.Sleep(0);
			}
		}

		public static string Name { get; } = "데포르쥬";
		public static int Port { get; } = 7777;
		public static string IpAddress { get; set; }

		static void Main(string[] args)
		{
			GameLogic.Instance.Push(() => { GameLogic.Instance.Add(1); });

			// DNS (Domain Name System)
			string host = Dns.GetHostName();
			IPHostEntry ipHost = Dns.GetHostEntry(host);
			IPAddress ipAddr = ipHost.AddressList[0];
			IPEndPoint endPoint = new IPEndPoint(ipAddr, Port);

			IpAddress = ipAddr.ToString();

			_listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
			Console.WriteLine("Listening...");

			// NetworkTask
			{
				Thread t = new Thread(NetworkTask);
				t.Name = "Network Send";
				t.Start();
			}

			// GameLogic
			Thread.CurrentThread.Name = "GameLogic";
			GameLogicTask();
		}
	}
}
