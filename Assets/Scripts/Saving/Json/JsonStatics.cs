// JsonStatics.cs
// 07-15-2022
// James LaFritz

using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPGEngine.Saving
{
    public static class JsonStatics
    {
        /// <summary>
        /// UnityEngine Vector3 to JsonObject.
        /// </summary>
        /// <param name="vector">The Vector3</param>
        /// <returns>JToken Object</returns>
        public static JToken ToToken(this Vector3 vector)
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["x"] = vector.x;
            stateDict["y"] = vector.y;
            stateDict["z"] = vector.z;
            return state;
        }

        /// <summary>
        /// Gets a Vector3 from a JsonObject.
        /// </summary>
        /// <param name="state">The state JToken representing the Vector3</param>
        /// <returns>The Vector3</returns>
        public static Vector3 ToVector3(this JToken state)
        {
            Vector3 vector = new Vector3();
            if (state is JObject jObject)
            {
                IDictionary<string, JToken> stateDict = jObject;

                if (stateDict.TryGetValue("x", out JToken x))
                {
                    vector.x = x.ToObject<float>();
                }

                if (stateDict.TryGetValue("y", out JToken y))
                {
                    vector.y = y.ToObject<float>();
                }

                if (stateDict.TryGetValue("z", out JToken z))
                {
                    vector.z = z.ToObject<float>();
                }
            }

            return vector;
        }

    }
}