using System;
using System.Collections.Generic;
using System.Text;

namespace MerlinEncrypt
{
	internal class Crypt
	{
		#region Initialize

		/// <summary>
		/// Encryption / Decryption Functions, with event notification
		/// </summary>
		public Crypt()
		{
			// register the event
			clsAES.AES_EncryptDecryptEvent += new clsAES.AES_EncryptDecryptEventHandler(clsAES_AES_EncryptDecryptEvent);
		}

		/// <summary>
		/// Encryption / Decryption Functions, with event notification
		/// </summary>
        /// <param name="keySize">default key size</param>
        public Crypt(AES_KeySize keySize)
        {
			m_KeySize = keySize;

			// register the event
			clsAES.AES_EncryptDecryptEvent += new clsAES.AES_EncryptDecryptEventHandler(clsAES_AES_EncryptDecryptEvent);
		}

		/// <summary>
		/// Encryption / Decryption Functions, with event notification
		/// </summary>
		/// <param name="keySize">default key size</param>
		/// <param name="defaultPassword">default password</param>
        public Crypt(AES_KeySize keySize, string defaultPassword)
        {
			m_KeySize = keySize;
			DefaultPassword = defaultPassword;

			// register the event
			clsAES.AES_EncryptDecryptEvent += new clsAES.AES_EncryptDecryptEventHandler(clsAES_AES_EncryptDecryptEvent);
		}

		// Initialize
		#endregion

		#region Event Declarations

		public delegate void MerlinEncryptUpdateEventHandler(int PercentageEncrypted, bool Encrypting);
		public event MerlinEncryptUpdateEventHandler EncryptProgressUpdate;

		/// <summary>
		/// Raise the encrypt/decrypt progress event
		/// </summary>
		/// <param name="PercentageEncrypted">percentage encrtyped/decrypted</param>
		/// <param name="Encrypting">true if encrypting, false if decrypting</param>
		void clsAES_AES_EncryptDecryptEvent(int PercentageEncrypted, bool Encrypting)
		{
			if (EncryptProgressUpdate != null)
				EncryptProgressUpdate(PercentageEncrypted, Encrypting);
		}

		// Event Declarations
		#endregion

		#region Variables and Properties

		private int PasswordLength = 128;

		private AES_KeySize m_KeySize = AES_KeySize.Bits128;
		/// <summary>
		/// Sets the size of the key
		/// </summary>
		public AES_KeySize KeySize
		{
			get { return m_KeySize; }
			set 
			{ 
				m_KeySize = value;

				// chnage PasswordLength as needed
				switch (m_KeySize)
				{
					case AES_KeySize.Bits128:
						PasswordLength = 128;
						break;
					case AES_KeySize.Bits192:
						PasswordLength = 192;
						break;
					case AES_KeySize.Bits256:
						PasswordLength = 256;
						break;
				}
			}
		}

        // 12screwyou34guysima56goinghome78
        private byte[] m_DPB = new byte[32] { 49, 50, 115, 99, 114, 101, 119, 121, 111, 117, 51, 52, 103, 117, 121, 115, 105, 109, 97, 53, 54, 103, 111, 105, 110, 103, 104, 111, 109, 101, 55, 56 };
        private string m_DefaultPassword = string.Empty;
		/// <summary>
		/// The default password to use if none is given
		/// </summary>
		public string DefaultPassword
		{
			get 
            {
                if (string.IsNullOrEmpty(m_DefaultPassword))
                    m_DefaultPassword = ASCIIEncoding.ASCII.GetString(m_DPB);

                return m_DefaultPassword; 
            }
			set { m_DefaultPassword = value; }
		}

		// Variables and Properties
		#endregion

		#region Byte Encryption

		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encryptedByteArray">the byte array to decrypt</param>
		public byte[] DecryptByteArray(byte[] encryptedByteArray)
		{
			return DecryptByteArray(encryptedByteArray, DefaultPassword);
		}

		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encryptedByteArray">the byte array to decrypt</param>
		public byte[] DecryptByteArray(byte[] encryptedByteArray, string password)
		{
			// set the default password
			if (string.IsNullOrEmpty(password))
				password = DefaultPassword;

			// convert the password to a byte array
			byte[] pswd = GetFullPassword(password);

			// decrypt
			return AES_Decrypt(m_KeySize, pswd, encryptedByteArray);
		}

		/// <summary>
		/// encrypts  a string
		/// </summary>
		/// <param name="plainByteArray">the byte array to encrypt. must be at least length of 9</param>
		public byte[] EncryptByteArray(byte[] plainByteArray)
		{
			return EncryptByteArray(plainByteArray, DefaultPassword);
		}

		/// <summary>
		/// encrypts  a string
		/// </summary>
		/// <param name="plainByteArray">the byte array to encrypt. must be at least length of 9</param>
		public byte[] EncryptByteArray(byte[] plainByteArray, string password)
		{
			// set the default password
			if (string.IsNullOrEmpty(password))
				password = DefaultPassword;

			// convert the password to a byte array
			byte[] pswd = GetFullPassword(password);

			// encrypt
			return AES_Encrypt(m_KeySize, pswd, plainByteArray);
		}

		#endregion // Byte Encryption

		#region String Encryption

		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encryptedString"></param>
		public string DecryptString(string encryptedString)
		{
			return DecryptString(encryptedString, DefaultPassword);
		}

		/// <summary>
		/// Decrypts a string
		/// </summary>
		/// <param name="encryptedString"></param>
		public string DecryptString(string encryptedString, string password)
		{
			// set the default password
			if (string.IsNullOrEmpty(password))
				password = DefaultPassword;

			// convert the password to a byte array
			byte[] pswd = GetFullPassword(password);

			// decrypt
			string tempStr = AES_Decrypt(m_KeySize, ConvertByteArrayToString(pswd), encryptedString);

			// remove extra characters if they were added
			tempStr = tempStr.Replace("|", string.Empty);

			return tempStr;
		}

		/// <summary>
		/// encrypts  a string
		/// </summary>
		/// <param name="plainText">the string to encrypt</param>
		public string EncryptString(string plainText)
		{
			return EncryptString(plainText, DefaultPassword);
		}

		/// <summary>
		/// encrypts  a string
		/// </summary>
		/// <param name="plainText">the string to encrypt</param>
		public string EncryptString(string plainText, string password)
		{
			// set the default password
			if (string.IsNullOrEmpty(password))
				password = DefaultPassword;

			// convert the password to a byte array
			byte[] pswd = GetFullPassword(password);

			// if plainText.length <9, we need to add special letters to it
			while (plainText.Length < 9)
				plainText = string.Format("{0}{1}", plainText, "|");

			// encrypt
			return AES_Encrypt(m_KeySize, ConvertByteArrayToString(pswd), plainText);
		}

		#endregion // String Encryption

		#region AES - Advanced Encryption Standard

		public enum AES_KeySize 
		{ 
			Bits128 = 0, 
			Bits192 = 1, 
			Bits256 = 2
		};  // key size, in bits, for construtor

		#region Encrypt

		/// <summary>
		/// Encrypts a string using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the string to encrypt</param>
		private string AES_Encrypt(AES_KeySize keySize, string Password, string input)
		{
			string output = string.Empty;

			//try
			{
				// get the proper key size
				clsAES.KeySize kSize;
				switch (keySize)
				{
					case AES_KeySize.Bits128:
						kSize = clsAES.KeySize.Bits128;
						break;
					case AES_KeySize.Bits192:
						kSize = clsAES.KeySize.Bits192;
						break;
					case AES_KeySize.Bits256:
						kSize = clsAES.KeySize.Bits256;
						break;
					default:
						kSize = clsAES.KeySize.Bits128;
						break;
				}

				// do the encryption
				clsAES aes = new clsAES(kSize, Password);
				output = aes.Encrypt(input.Trim());
			}

//			catch (Exception excep)
//			{
//			}

			// return the string
			return output;
		}

		/// <summary>
		/// Encrypts a byte array using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the byte array to encrypt</param>
		private byte[] AES_Encrypt(AES_KeySize keySize, string Password, byte[] input)
		{
			// convert the password to a byte array
			byte[] pwd = ConvertToByteArray(Password);

			// do the encryption
			return AES_Encrypt(keySize, pwd, input);
		}

		/// <summary>
		/// Encrypts a byte array using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the byte array to encrypt</param>
		private byte[] AES_Encrypt(AES_KeySize keySize, byte[] Password, byte[] input)
		{
			byte[] output = new byte[0];

//			try
			{
				// get the proper key size
				clsAES.KeySize kSize;
				switch (keySize)
				{
					case AES_KeySize.Bits128:
						kSize = clsAES.KeySize.Bits128;
						break;
					case AES_KeySize.Bits192:
						kSize = clsAES.KeySize.Bits192;
						break;
					case AES_KeySize.Bits256:
						kSize = clsAES.KeySize.Bits256;
						break;
					default:
						kSize = clsAES.KeySize.Bits128;
						break;
				}

				// do the encryption
				clsAES aes = new clsAES(kSize, Password);
				output = aes.Encrypt(input);
			}

//			catch (Exception excep)
//			{
//			}

			// return the string
			return output;
		}

		#endregion // Encrypt

		#region Decrypt

		/// <summary>
		/// Decrypts a string using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the string to decrypt</param>
		private string AES_Decrypt(AES_KeySize keySize, string Password, string input)
		{
			string output = string.Empty;

			//try
			{
				// get the proper key size
				clsAES.KeySize kSize;
				switch (keySize)
				{
					case AES_KeySize.Bits128:
						kSize = clsAES.KeySize.Bits128;
						break;
					case AES_KeySize.Bits192:
						kSize = clsAES.KeySize.Bits192;
						break;
					case AES_KeySize.Bits256:
						kSize = clsAES.KeySize.Bits256;
						break;
					default:
						kSize = clsAES.KeySize.Bits128;
						break;
				}

				// do the decryption
				clsAES aes = new clsAES(kSize, Password);
				output = aes.Decrypt(input.Trim());
			}

//			catch (Exception excep)
//			{
//			}

			// return the string
			return output;
		}

		/// <summary>
		/// Decrypts a byte array using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the byte array to decrypt</param>
		private byte[] AES_Decrypt(AES_KeySize keySize, string Password, byte[] input)
		{
			// convert the password to a byte array
			byte[] pwd = ConvertToByteArray(Password);

			// do the decryption
			return AES_Decrypt(keySize, pwd, input);
		}

		/// <summary>
		/// Decrypts a byte array using AES encryption
		/// </summary>
		/// <param name="keySize">the key size</param>
		/// <param name="Password">the password</param>
		/// <param name="input">the byte array to decrypt</param>
		private byte[] AES_Decrypt(AES_KeySize keySize, byte[] Password, byte[] input)
		{
			byte[] output = new byte[0];

			//try
			{
				// get the proper key size
				clsAES.KeySize kSize;
				switch (keySize)
				{
					case AES_KeySize.Bits128:
						kSize = clsAES.KeySize.Bits128;
						break;
					case AES_KeySize.Bits192:
						kSize = clsAES.KeySize.Bits192;
						break;
					case AES_KeySize.Bits256:
						kSize = clsAES.KeySize.Bits256;
						break;
					default:
						kSize = clsAES.KeySize.Bits128;
						break;
				}

				// do the decryption
				clsAES aes = new clsAES(kSize, Password);
				output = aes.Decrypt(input);
			}

			//catch (Exception excep)
			{
			}

			// return the string
			return output;
		}

		#endregion // Decrypt

		#endregion // AES - Advanced Encryption Standard}

		#region Helper Functions

		/// <summary>
		/// build the password string
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		private byte[] GetFullPassword(string password)
		{
			if (string.IsNullOrEmpty(password))
				password = DefaultPassword;

			// make sure it's long enough
			while (password.Length < 4)
				password += password;

			// add the password to the stringbuilder
			StringBuilder sb = new StringBuilder();
			int onethird = 0;
			sb.Append(password);

			// loop through until we get the desired length for password
			while (sb.Length < PasswordLength)
			{
				// take the middle chunk and append it
				onethird = sb.Length / 3;
				sb.Append(sb.ToString().Substring(onethird, onethird));
			}

			// convert the password to a byte array
			return ConvertToByteArray(sb.ToString());
		}

		/// <summary>
		/// convert the bytearray to a string
		/// </summary>
		/// <param name="textB"></param>
		/// <returns></returns>
		internal string ConvertByteArrayToString(byte[] textB)
		{
			StringBuilder sb = new StringBuilder();
			int textBLen = textB.Length;
			for (int i = 0; i < textBLen; i++)
				sb.Append(System.Convert.ToChar(textB[i]));

			return sb.ToString();
		}

		/// <summary>
		/// convert a string to a byte array
		/// </summary>
		/// <param name="textStr"></param>
		/// <returns></returns>
		internal byte[] ConvertToByteArray(string textStr)
		{
			byte[] textB = new byte[textStr.Length];
			int textBLen = textB.Length;
			for (int i = 0; i < textBLen; i++)
				textB[i] = System.Convert.ToByte(textStr[i]);

			return textB;
		}

		/// <summary>
		/// convert a string that is a byte string to a byte array
		/// </summary>
		/// <param name="textStr"></param>
		/// <returns></returns>
		public byte[] ConvertByteStringToByteArray(string textStr)
		{
			byte[] textB = new byte[textStr.Length / 3];
			int j = 0, textStrLen = textStr.Length;
			for (int i = 0; i < textStrLen; i = i + 3)
				textB[j++] = System.Convert.ToByte(textStr.Substring(i, 3));

			return textB;
		}

		/// <summary>
		/// converts a byte array to a byte string
		/// </summary>
		/// <param name="textB"></param>
		/// <returns></returns>
		public string ConvertByteArrayToByteString(byte[] textB)
		{
			StringBuilder sb = new StringBuilder();
			int j = 0;
			for (int i = 0; i < textB.Length; i++)
			{
				j = textB[i].ToString().Length;
				if (j == 3)
					sb.Append(string.Format("{0}", textB[i].ToString().Trim()));
				else if (j == 2)
					sb.Append(string.Format("0{0}", textB[i].ToString().Trim()));
				else if (j == 1)
					sb.Append(string.Format("00{0}", textB[i].ToString().Trim()));
			}
			return sb.ToString();
		}

		// Helper Functions
		#endregion
	}
}
