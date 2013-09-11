// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2009, SIL International. All Rights Reserved.
// <copyright from='2009' to='2009' company='SIL International'>
//		Copyright (c) 2009, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of the MIT License (http://sil.mit-license.org/)
// </copyright> 
#endregion
// 
// File: ContinuableErrorException.cs
// ---------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace SIL.Utils
{
	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// Use this exception when an error occurs from which a user may continue. This will encourage
	/// error reporting (stack and log provided for user to report error to development team).
	/// </summary>
	/// ----------------------------------------------------------------------------------------
	public class ContinuableErrorException : ApplicationException
	{
		/// -----------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ContinuableErrorException"/> class.
		/// </summary>
		/// -----------------------------------------------------------------------------------
		public ContinuableErrorException(string message)
			: base(message)
		{
		}

		/// ------------------------------------------------------------------------------------
		/// <summary>
		/// Initializes a new instance of the <see cref="ContinuableErrorException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// ------------------------------------------------------------------------------------
		public ContinuableErrorException(string message, Exception innerException) :
			base(message, innerException)
		{
		}
	}
}
