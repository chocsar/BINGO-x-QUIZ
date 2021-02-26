using System.Collections.Generic;
using UnityEngine;
using System;

namespace Utility
{
    public static class UtilityPass
    {
        private const string PASSWORD_CHARS =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string GeneratePassword(int length = 15)
        {
            var sb = new System.Text.StringBuilder(length);
            var r = new System.Random();

            for (int i = 0; i < length; i++)
            {
                int pos = r.Next(PASSWORD_CHARS.Length);
                char c = PASSWORD_CHARS[pos];
                sb.Append(c);
            }

            return sb.ToString();
        }
    }

    public static class UtilityFirebase
    {
        public static string StringParse(string _data)
        {
            return _data.Trim();
        }
    }

    public static class UtilityRestJson
    {
        public static Dictionary<string, string> JsonPhaseLoad(string _data)
        {
            Debug.Log(_data);

            Dictionary<string, string> phaseDic = new Dictionary<string, string>();
            var data = _data;
            data = data.TrimStart('{').TrimEnd('}');
            var datas = data.Split(',');
            Debug.Log(datas);
            foreach (var userdata in datas)
            {
                var userdatas = userdata.Split(':');
                Debug.Log(userdatas);
                phaseDic.Add(userdatas[0].Trim('"'), userdatas[1].Trim('"'));
            }

            return phaseDic;
        }

        public static List<ClientStatus> JsonStatusLoad(string _data)
        {
            List<ClientStatus> clientStatuses = new List<ClientStatus>();

            string[] comma = { "}," };
            string[] colon = { ":{" };
            Dictionary<string, string> statusDic = new Dictionary<string, string>();
            var data = _data;
            data = data.TrimStart('{').TrimEnd('}');
            var datas = data.Split(comma, StringSplitOptions.None);
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] += "}";
                //Debug.Log(datas[i]);
            }
            foreach (var item in datas)
            {
                var individualItem = item.Split(colon, StringSplitOptions.None);
                individualItem[1] = "{" + individualItem[1];
                //Debug.Log(individualItem[1]);
                ClientStatus status = JsonUtility.FromJson<ClientStatus>(individualItem[1]);
                clientStatuses.Add(status);
            }

            return clientStatuses;
            
        }
    }
}
