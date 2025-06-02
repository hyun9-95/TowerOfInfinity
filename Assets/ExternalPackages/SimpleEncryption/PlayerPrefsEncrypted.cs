using System;
using System.Globalization;
using UnityEngine;

namespace Assets.SimpleEncryption
{
	/// <summary>
	/// A wrapper over PlayerPrefs that provides protected data storage.
	/// </summary>
    public static class PlayerPrefsEncrypted
	{
		private const string Key = "Key";

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(Md5.ComputeHash(key));
        }

        public static string GetString(string key)
        {
            key = Md5.ComputeHash(key);

            return PlayerPrefs.HasKey(key) ? B64X.Decrypt(PlayerPrefs.GetString(key), Key) : null;
        }

        public static void SetString(string key, string value)
        {
	        if (value == null)
	        {
		        PlayerPrefs.DeleteKey(Md5.ComputeHash(key));
			}
	        else
	        {
				PlayerPrefs.SetString(Md5.ComputeHash(key), B64X.Encrypt(value, Key));
			}

            PlayerPrefs.Save();
        }

        public static int GetInt(string key)
        {
            return int.Parse(GetString(key) ?? "0");
        }

        public static void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }

        public static bool GetBool(string key)
        {
            return GetString(key) != null;
        }

        public static void SetBool(string key, bool value)
        {
            if (value)
            {
                SetString(key, UnityEngine.Random.Range(0, 999999).ToString());
            }
            else
            {
                PlayerPrefs.DeleteKey(Md5.ComputeHash(key));
                PlayerPrefs.Save();
            }
        }

        public static DateTime GetDateTime(string key)
        {
            var value = GetString(key);

            return value == null ? DateTime.MinValue : DateTime.Parse(value);
        }

        public static void SetDateTime(string key, DateTime value)
        {
            SetString(key, value.ToString(CultureInfo.InvariantCulture));
        }

        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(Md5.ComputeHash(key));
            PlayerPrefs.Save();
        }
    }
}