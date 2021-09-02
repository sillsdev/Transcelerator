// --------------------------------------------------------------------------------------------
#region // Copyright  2021, SIL International.   
// <copyright from='2012' to='2021' company='SIL International'>
//		Copyright  2021, SIL International.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: TxlSplashScreen.cs
// --------------------------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace SIL.Transcelerator
{
	#region TxlSplashScreen implementation
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Transcelerator Splash Screen
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class TxlSplashScreen : IProgressMessage, IDisposable
	{
		#region Data members
		private delegate void SetStringPropDelegate(string value);

		private RealSplashScreen m_splashScreen;
		private EventWaitHandle m_waitHandle;
		Screen m_displayToUse;
		#endregion

		#region Disposable stuff
		#if DEBUG
		/// <summary/>
		~TxlSplashScreen()
		{
			Dispose(false);
		}
		#endif
		
		/// <summary/>
		private bool IsDisposed { get; set; }
		
		/// <summary/>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		/// <summary/>
		private void Dispose(bool fDisposing)
		{
			Debug.WriteLineIf(!fDisposing, "****** Missing Dispose() call for " + GetType() + ". ****** ");
			if (fDisposing && !IsDisposed)
			{
				// dispose managed and unmanaged objects
				Close();
				m_waitHandle?.Close();
				m_splashScreen?.Dispose();
			}
			m_waitHandle = null;
			m_splashScreen = null;
			IsDisposed = true;
		}
		#endregion

		#region Public Methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Shows the splash screen
		/// </summary>
		/// ------------------------------------------------------------------------------------
		public void Show(Screen display)
		{
			if (m_splashScreen != null)
				return;

			m_displayToUse = display;
			m_waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

			StartSplashScreen(); // Create Modeless dialog (this had better be called on Main GUI thread)
			
			Debug.Assert(m_splashScreen != null);
			Message = string.Empty;
		}

        /// ----------------------------------------------------------------------------------------
        /// <summary>
        /// Activates the (real) splash screen
        /// </summary>
        /// ----------------------------------------------------------------------------------------
        public void Activate()
        {
			if (m_splashScreen != null)
			{
				try
				{
					m_splashScreen.Invoke(new MethodInvoker(m_splashScreen.Activate));
				}
				catch
				{
					// Something bad happened, but don't die
				}
			}
		}

		/// ----------------------------------------------------------------------------------------
		/// <summary>
		/// Closes the splash screen
		/// </summary>
		/// ----------------------------------------------------------------------------------------
		public void Close()
		{
			if (m_splashScreen == null)
				return;

			m_splashScreen.RealClose(); // Invokes if needed. Guaranteed not to throw.
			
			m_splashScreen = null;
		}
		#endregion

		#region Public properties
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// The message to display to indicate startup activity on the splash screen
		/// </summary>
		/// <value></value>
		/// ------------------------------------------------------------------------------------
		public string Message
		{
			set => m_splashScreen.SetMessage(value); // Invokes if needed. Guaranteed not to throw.
		}
		#endregion

		#region private methods
		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Starts the splash screen.
		/// </summary>
		/// ------------------------------------------------------------------------------------
		private void StartSplashScreen()
		{
			m_splashScreen = new RealSplashScreen(m_displayToUse);
			m_splashScreen.WaitHandle = m_waitHandle;
			m_splashScreen.Show();
		}
		#endregion
	}
	#endregion
}
