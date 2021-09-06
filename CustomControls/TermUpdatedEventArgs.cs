using ExtendedArithmetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathEquationControl
{
	public delegate void TermUpdatedEventHandler(object sender, TermUpdatedEventArgs e);

	public class TermUpdatedEventArgs : EventArgs
	{
		internal Term _term;

		/// <summary>
		/// constructor
		/// </summary>
		internal TermUpdatedEventArgs(Term newValue)
		{
			_term = newValue;
		}

		/// <summary>
		/// Get and set the exit code to be returned by this application
		/// </summary>
		public Term TermValue
		{
			get { return _term; }
			set { _term = value; }
		}
	}
}
