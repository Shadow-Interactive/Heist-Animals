using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

namespace ObserverPattern
{
	public class EventConsole
	{
		PlayerLogic p1;
		PlayerLogic p2;

		Text t1, t2, t3, t4, t5; //get in the text slots

		ObserverPattern.Watcher mes;


		//Will send notifications that something has happened to whoever is interested
		Subject subject = new Subject();

		//p1 is the friendly runner, p2 is the enemy runner
		public EventConsole(PlayerLogic p1, PlayerLogic p2)
		{
			this.p1 = p1;
			this.p2 = p2;
			mes = new Watcher(p1, p2, new Events());
		}

		// Use this for initialization
		public void init()
		{

		}
		//call on update
		public string repetitiveshit()
		{
			return mes.OnNotify();
		}

	}

	//try to load in the message from GUI manager
	public class Events
	{
		//const string dllname = "GUIManager";

		//[DllImport(dllname)]
		//private static extern void LoadButtonsFromFile();

		//[DllImport(dllname)]
		//private static extern void AddButton(Color btnColor, Vector3 btnPosition, string newTxt, int ID);

		//[DllImport(dllname)]
		//private static extern void RemoveButton(int index);

		//[DllImport(dllname)]
		//private static extern void ChangeEntityColor(Color newColor, int index);

		//[DllImport(dllname)]
		//private static extern void ChangeEntityPosition(Vector3 newPosition, int index);

		//[DllImport(dllname)]
		//private static extern void ChangeEntityText(string newTxt, int index);

		//[DllImport(dllname)]
		//private static extern float getEntityColorR(int index);
		//[DllImport(dllname)]
		//private static extern float getEntityColorG(int index);
		//[DllImport(dllname)]
		//private static extern float getEntityColorB(int index);
		//[DllImport(dllname)]
		//private static extern float getEntityColorA(int index);

		//[DllImport(dllname)]
		//private static extern float getEntityVecX(int index);
		//[DllImport(dllname)]
		//private static extern float getEntityVecY(int index);
		//[DllImport(dllname)]
		//private static extern float getEntityVecZ(int index);

		//[DllImport(dllname)]
		//private static extern System.IntPtr GetEntityText(int index);

		//[DllImport(dllname)]
		//private static extern void Destroy();

		//[DllImport(dllname)]
		//private static extern void EventDeleteFront();

		//[DllImport(dllname)]
		//private static extern bool EventIsEmpty();

		//[DllImport(dllname)]
		//private static extern System.IntPtr EventGetFront();

		//[DllImport(dllname)]
		//private static extern int GetFrontQIndex();

		//[DllImport(dllname)]
		//private static extern int GetEntitySize();



		//friendly announcements
		public string fTeleported()
		{
			return "player got teleported";
		}
		public string fZapped()
		{
			return "player got zapped";
		}
		public string fObjective()
		{
			return "player got an objective";
		}
		public string fTrapped()
		{
			return "player got trapped";
		}
		public string fDisarmed()
		{
			return "player disarmed the trap";
		}

		//enemy announcements
		public string eTeleported()
		{
			return "player got teleported";
		}
		public string eZapped()
		{
			return "player got zapped";
		}
		public string eObjective()
		{
			return "player got an objective";
		}
		public string eTrapped()
		{
			return "player got trapped";
		}
		public string eDisarmed()
		{
			return "player disarmed the trap";
		}
	}


	//this is your observer class
	public class Watcher
	{
		PlayerLogic p1;//p1 is friendly runner
		PlayerLogic p2;//p2 is the enemy runner

		Events gameEvent;

		public Watcher(PlayerLogic p1, PlayerLogic p2, Events gameEvent)
		{
			this.p1 = p1;
			this.p2 = p2;
			this.gameEvent = gameEvent;
		}

		//What the box will do if the event fits it (will always fit but you will probably change that on your own)
		public string OnNotify()
		{
			return checkEvents();
		}

		string checkEvents()
		{
			//friendly
			if (p1.currstate == 1)
				return (gameEvent.fTrapped() + " event console works.");
			else if (p1.currstate == 2)
				return gameEvent.fZapped();
			else if (p1.currstate == 3)
				return gameEvent.fTeleported();
			else if (p1.currstate == 4)
				return gameEvent.fObjective();
			else if (p1.currstate == 5)
				return gameEvent.fDisarmed();
			else if (p1.currstate == 0)
				return ("Nothing Happens hopefully event console works");

			//enemy
			else if (p2.currstate == 1)
				return gameEvent.eTrapped();
			else if (p2.currstate == 2)
				return gameEvent.eZapped();
			else if (p2.currstate == 3)
				return gameEvent.eTeleported();
			else if (p2.currstate == 4)
				return gameEvent.eObjective();
			else if (p2.currstate == 5)
				return gameEvent.eDisarmed();
			else
				return null;

			//return "nothing";
		}
	}


	//this is your subject class
	//sn:can prolly jam this in somewhere
	//this would be your text
	//i dont really need this
	public class Subject
	{
		//A list with observers that are waiting for something to happen
		List<Watcher> message = new List<Watcher>();

		//Send notifications if something has happened
		public void Notify()
		{
			for (int i = 0; i < message.Count; i++)
			{
				//Notify all observers even though some may not be interested in what has happened
				//Each observer should check if it is interested in this event
				message[i].OnNotify();
			}
		}

		//Add observer to the list
		public void AddObserver(Watcher observer)
		{
			message.Add(observer);
		}

		//Remove observer from the list
		public void RemoveObserver(Watcher observer)
		{
			message.Remove(observer);
		}
	}

}
