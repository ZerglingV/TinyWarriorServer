using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TinyWarriorServer
{
        public class ByteBuffer
        {
                MemoryStream stream = null;
                BinaryWriter writer = null;
                BinaryReader reader = null;

                #region -- Construct Method --

                public ByteBuffer()
                {
                        stream = new MemoryStream();
                        writer = new BinaryWriter(stream);
                }

                public ByteBuffer(string data)
                {
                        stream = new MemoryStream();
                        writer = new BinaryWriter(stream);
                        writer.Write(data);
                }

                public ByteBuffer(byte[] data)
                {
                        if (data != null)
                        {
                                stream = new MemoryStream(data);
                                reader = new BinaryReader(stream);
                        }
                        else
                        {
                                stream = new MemoryStream();
                                writer = new BinaryWriter(stream);
                        }
                }

                #endregion

                #region -- Reader & Writer --

                public string GetString()
                {
                        return reader.ReadString();
                }

                public object GetObject(byte[] data)
                {
                        writer.Flush();
                        try
                        {
                                BinaryFormatter formatter = new BinaryFormatter();
                                MemoryStream rems = new MemoryStream(data);
                                data = null;
                                return formatter.Deserialize(rems);
                        }
                        catch
                        {
                                return null;
                        }
                }

                public byte[] ToBytes()
                {
                        writer.Flush();
                        return stream.ToArray();
                }

                // use serialization to transfer object
                public byte[] ToBytes(object data)
                {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, data); // contains writer operation
                        return ToBytes();
                }

                public void Close()
                {
                        if (writer != null) writer.Close();
                        if (reader != null) reader.Close();

                        stream.Close();
                        writer = null;
                        reader = null;
                        stream = null;
                }

                #endregion
        }
}
