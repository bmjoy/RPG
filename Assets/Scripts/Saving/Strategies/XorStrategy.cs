// XorStrategy.cs
// 07-16-2022
// James LaFritz

using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RPGEngine.Saving
{
    /// <summary>
    /// A <see cref="SavingStrategy"/> that Saves and Loads in the form of
    /// <a href="https://en.wikipedia.org/wiki/XOR_cipher">XOR Cypher</a>.
    /// </summary>
    [CreateAssetMenu(menuName = "SavingStrategies/Xor", fileName = "XorStrategy")]
    public class XorStrategy : SavingStrategy
    {
        [Header("Use Context Menu to generate a random key.")] [SerializeField]
        string key = "TheQuickBrownFoxJumpedOverTheLazyDog";

        #region Overrides of SavingStrategy

        /// <inheritdoc />
        public override string Extension => ".sav";

        /// <inheritdoc />
        public override void SaveToFile(string saveFile, JObject state)
        {
            string path = GetPath(saveFile);
            Debug.Log($"Saving to {path} ");
            using StreamWriter textWriter = File.CreateText(path!);
            string json = state.ToString();
            string encoded = EncryptDecrypt(json, key);
            textWriter.Write(encoded);
        }

        /// <inheritdoc />
        public override JObject LoadFromFile(string saveFile)
        {
            string path = GetPath(saveFile);
            if (!File.Exists(path))
            {
                return new JObject();
            }

            using StreamReader textReader = File.OpenText(path!);
            string encoded = textReader.ReadToEnd();
            string json = EncryptDecrypt(encoded, key);
            return (JObject)JToken.Parse(json!);
        }

        #endregion

        /// <summary>
        /// Encrypts or Decrypts a string using XOR.
        /// </summary>
        /// <param name="input">The string to encrypt/decrypt.</param>
        /// <param name="encryptionKey">The string to XOR with.</param>
        /// <returns></returns>
        private string EncryptDecrypt(string input, string encryptionKey)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            StringBuilder output = new StringBuilder();
            int encryptionKeyLength = encryptionKey.Length;
            int i = 0;
            foreach (char c in input)
            {
                output.Append((char)(c ^ encryptionKey[i++ % encryptionKeyLength]));
            }

            return output.ToString();
        }

        #if UNITY_EDITOR

        [ContextMenu("Generate Random Key")]
        private void GenerateKey()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("key");
            property.stringValue = System.Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        #endif
    }
}