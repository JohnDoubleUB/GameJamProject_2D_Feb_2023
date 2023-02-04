using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;
using UnityEngine;

public class SaveSystem<TSerializableObject>
{
    public string SaveExtension;
    public string SaveName;
    public string SaveLocation;

    public bool VersioningMustMatch;
    public bool PrefixAllXml;
    public bool UseEncryption;

    public Guid EncryptionGuid = Guid.Parse("ed1708258c5c43b688afc1aa926d5189");

    private string GetPathString() 
    {
        return Path.Combine(SaveLocation, $"{SaveName}.{SaveExtension}");
    }

    public void SaveDataToXMLFile(TSerializableObject serializableObject)
    {
        string path = GetPathString();
        if (PrefixAllXml) path += ".xml";

        XmlSerializer serializer = new XmlSerializer(typeof(SerializedXmlWithMetaData<TSerializableObject>));

        using (Stream stream = new FileStream(path, File.Exists(path) ? FileMode.Create : FileMode.CreateNew))
        {
            if (UseEncryption)
            {
                using (Stream encryptedStream = GetEncryptedXMLStream(stream, CryptoStreamMode.Write))
                {
                    serializer.Serialize(encryptedStream, new SerializedXmlWithMetaData<TSerializableObject>(serializableObject));
                }
            }
            else
            {
                serializer.Serialize(stream, new SerializedXmlWithMetaData<TSerializableObject>(serializableObject));
            }
        }

        Debug.Log(serializableObject.GetType().ToString() + " File saved: " + path);
    }

    public bool TryLoadDataFromXMLFile(out TSerializableObject serializableObject)
    {
        string path = GetPathString();
        if (PrefixAllXml) path += ".xml";

        if (File.Exists(path) && ReadValidObjectWithMetaData(path, out SerializedXmlWithMetaData<TSerializableObject> deserializedObjectWithMetaData))
        {
            serializableObject = deserializedObjectWithMetaData.SerializedObject;
            return true;
        }
        else
        {
            Debug.LogWarning("Valid Save File not found in " + path + ". VersioningMustMatch is set to: " + VersioningMustMatch.ToString());
            serializableObject = default(TSerializableObject);
            return false;
        }
    }

    //Returns true if the metadata meets the metadata requirements, i.e. matching versions
    private bool ReadValidObjectWithMetaData(string path, out SerializedXmlWithMetaData<TSerializableObject> deserializedObject)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(SerializedXmlWithMetaData<TSerializableObject>));

        bool success = false;

        using (Stream stream = new FileStream(path, FileMode.Open))
        {
            try
            {
                if (UseEncryption)
                {
                    try
                    {
                        using (Stream encryptedStream = GetEncryptedXMLStream(stream, CryptoStreamMode.Read))
                        {
                            deserializedObject = (SerializedXmlWithMetaData<TSerializableObject>)serializer.Deserialize(encryptedStream);
                        }

                        success = VersioningMustMatch ? deserializedObject.Version == Application.version : true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning("Failed to decrypt file: " + path + ". Could be that this file wasn't encrypted? New File will be created. Error: " + e);
                        deserializedObject = default(SerializedXmlWithMetaData<TSerializableObject>);
                    }
                }
                else
                {
                    deserializedObject = (SerializedXmlWithMetaData<TSerializableObject>)serializer.Deserialize(stream);
                    success = VersioningMustMatch ? deserializedObject.Version == Application.version : true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to Deserialize file: " + path + " but exception was handled, Invalid XML, could be an old save file?, Error: " + e);
                deserializedObject = default(SerializedXmlWithMetaData<TSerializableObject>);
            }
        }

        return success;
    }


    private Stream GetEncryptedXMLStream(Stream unEncryptedStream, CryptoStreamMode cryptoStreamMode)
    {
        byte[] encryptionGuidAsBytes = EncryptionGuid.ToByteArray();

        AesCryptoServiceProvider aes = new AesCryptoServiceProvider()
        {
            Key = encryptionGuidAsBytes,
            IV = encryptionGuidAsBytes
        };


        ICryptoTransform cryptoTransform = cryptoStreamMode == CryptoStreamMode.Write ? aes.CreateEncryptor() : aes.CreateDecryptor();

        CryptoStream cryptoStream = new CryptoStream(
            unEncryptedStream,
            cryptoTransform,
            cryptoStreamMode
            );

        return cryptoStream; //return to use
    }

}

public struct SerializedXmlWithMetaData<TSerializableObject>
{
    public string Version;
    public TSerializableObject SerializedObject;

    public SerializedXmlWithMetaData(TSerializableObject SerializedObject)
    {
        this.SerializedObject = SerializedObject;
        Version = Application.version;
    }
}