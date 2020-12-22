#define USE_STACK

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;



namespace EMX.HierarchyPlugin.Editor
{
	internal class MultiThread
	{
		//static MultyThead _get;
		/*  internal static MultyThead get {
			  get { return _get ?? (_get = GameObject.FindObjectOfType<MultyThead>()); }
		  }
		  void Awake()
		  {
			  _get = this;


		  }*/

		static int? _INSTANCE_ID;
		internal static int INSTANCE_ID
		{
			get { return _INSTANCE_ID ?? (_INSTANCE_ID = SessionState.GetInt("EMX_MultiThread_ID", 0)).Value; }
			set { SessionState.SetInt("EMX_MultiThread_ID", (_INSTANCE_ID = value).Value); }
		}

		static MultiThread()
		{
			//return;
			//#pragma warning disable
			INSTANCE_ID++;
			EditorApplication.update -= updater;
			EditorApplication.update += updater;
			if (myJob != null)
				foreach (var threadedJob in myJob)
				{
					threadedJob.Abort();
				}
			myJob.Clear();
		}

		static void updater()
		{
			if (myJob.Count == 0) return;
			myJob.RemoveAll(j => j.Update());
		}

		static List<ThreadedJob> myJob = new List<ThreadedJob>();

		static internal Job CreateJob(Action action, Action onFinished, Action invokeInLock)
		{
			var j = new Job() { action = action, onFinish = onFinished, invokeInLock = invokeInLock };
			myJob.Add(j);

			return j;
		}



		static internal void Remove(ThreadedJob j)
		{
			myJob.Remove(j);
		}
		static internal bool Contains(ThreadedJob j)
		{
			return myJob.Contains(j);
		}
	}


	internal class Job : ThreadedJob
	{
		internal Action action;
		internal Action onFinish;
		internal Action invokeInLock;

		protected override void ThreadFunction()
		{
			action();
		}
		protected override void OnFinished()
		{
			onFinish();
		}
		protected override void InvokeInLock(Action ac)
		{
			onFinish();
		}
	}



	internal class ThreadedJob
	{

		int ID;
		internal ThreadedJob()
		{
			ID = MultiThread.INSTANCE_ID;
		}

		internal bool RequestStop = false;
		private bool m_IsDone = false;
		private object m_Handle = new object();
		private System.Threading.Thread m_Thread = null;
		public bool IsDone
		{
			get
			{
				bool tmp;
				lock (m_Handle)
				{
					tmp = m_IsDone || ID != MultiThread.INSTANCE_ID;
				}
				return tmp;
			}
			set
			{
				lock (m_Handle)
				{
					m_IsDone = value;
				}
			}
		}

		protected virtual void InvokeInLock(Action ac)
		{
			lock (m_Handle)
			{
				ac();
			}
		}

		public virtual void Start()
		{
			m_Thread = new System.Threading.Thread(Run);

			m_Thread.Start();
		}
		public virtual void Abort()
		{
			lock (m_Handle)
			{
				RequestStop = true;
			}

		}
		public bool IsAlive()
		{
			return !IsDone && MultiThread.Contains(this);
		}

		protected virtual void ThreadFunction() { }

		protected virtual void OnFinished() { }

		public virtual bool Update()
		{
			if (IsDone)
			{
				MultiThread.Remove(this);
				OnFinished();
				return true;
			}
			return false;
		}
		public IEnumerator WaitFor()
		{
			while (!Update())
			{
				yield return null;
			}
		}
		private void Run()
		{
			ThreadFunction();
			IsDone = true;
		}
	}
}


