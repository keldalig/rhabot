using System;
using System.Runtime.InteropServices; // DllImport

namespace XihSolutions.Serial
{
	/// <summary>
	/// Summary description for DotNETOneWaySerialWrapper.
	/// </summary>
	internal class DNOWSerialWrapper
	{
		public DNOWSerialWrapper()
		{
		}

		[DllImport("OneWaySerial.dll", CharSet=CharSet.Ansi)]
		public static extern String GetSerial(
			String serialPassword,
			int regOptions,
			int serialExpireDays,
			int regExpire,
			int regExpireDay,
			int regExpireMonth,
			int regExpireYear);
	}
}
