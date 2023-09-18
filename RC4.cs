using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEncryptTCP
{
	public class RC4
	{
		private byte[] key;
		private int[] S;
		private int i, j;

		public RC4(byte[] key)
		{
			this.key = key;
			S = new int[256];
			for (i = 0; i < 256; i++)
			{
				S[i] = i;
			}

			j = 0;
			for (i = 0; i < 256; i++)
			{
				j = (j + S[i] + key[i % key.Length]) % 256;
				Swap(S, i, j);
			}

			i = 0;
			j = 0;
		}

		private void Swap(int[] s, int i, int j)
		{
			int temp = s[i];
			s[i] = s[j];
			s[j] = temp;
		}

		public byte[] Encrypt(byte[] data)
		{
			byte[] encryptedData = new byte[data.Length];
			for (int k = 0; k < data.Length; k++)
			{
				i = (i + 1) % 256;
				j = (j + S[i]) % 256;
				Swap(S, i, j);
				int K = S[(S[i] + S[j]) % 256];
				encryptedData[k] = (byte)(data[k] ^ K);
			}
			return encryptedData;
		}

		public byte[] Decrypt(byte[] encryptedData)
		{
			// Decryption is the same as encryption for RC4
			return Encrypt(encryptedData);
		}
	}
}
