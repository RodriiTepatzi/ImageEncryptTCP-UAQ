using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP
{
	public class EncryptionManager
	{
		public static List<int> EncryptImage(string path)
		{
			byte[] key = Encoding.ASCII.GetBytes("2k");
			byte[] image = File.ReadAllBytes(path);

			List<int> EncrypMessage = RC4(image, key);

			return EncrypMessage;

			//File.WriteAllBytes(path, EncrypMessage.ConvertAll(b => (byte)b).ToArray());
		}

		static List<int> KSA(byte[] key)
		{

			//Llenamos K con la key
			//Si no cabe la repetimos
			List<int> K = new List<int>();

			int KeyLength = key.Length;
			int i = 0;
			int j = 0;

			while (i < 256)
			{

				if (j == KeyLength)
				{
					j = 0;
				}

				K.Add(key[j]);
				i++;
				j++;

			}

			List<int> S = new List<int>(256);
			for (i = 0; i < 256; i++)
			{

				S.Add(i);

			}

			j = 0;
			for (i = 0; i < 256; i++)
			{

				j = (j + S[i] + K[i]) % 256;

				int aux = S[i];
				S[i] = S[j];
				S[j] = aux;

			}

			return S;

		}

		static List<int> KeyStream(List<int> S, int DataLength)
		{

			List<int> KeyStream = new List<int>();

			int i = 0;
			int j = 0;

			for (int k = 0; k < DataLength; k++)
			{

				i = (i + 1) % 256;
				j = (j + S[i]) % 256;

				int aux = S[i];
				S[i] = S[j];
				S[j] = aux;

				int t = (S[i] + S[j]) % 256;
				KeyStream.Add(S[t]);

			}

			return KeyStream;
		}

		static List<int> RC4(byte[] data, byte[] key)
		{
			List<int> S = KSA(key);
			List<int> K = KeyStream(S, data.Length);
			List<int> EncrypMessage = new List<int>();

			for (int i = 0; i < data.Length; i++)
			{
				EncrypMessage.Add(data[i] ^ K[i]);
			}

			return EncrypMessage;
		}
	}
}
