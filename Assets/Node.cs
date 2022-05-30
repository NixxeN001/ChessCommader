using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace SimpleNodes
{

    /// <summary>
    /// Nodes store data, are comparable, and have have independent In and Out edges.
    /// </summary>
    /// <typeparam name="T">Node Data Type</typeparam>
    [Serializable]
    public class Node<T>
    {
        public ulong UID;
        public T Data;

        public List<Node<T>> Out = new List<Node<T>>();
        public List<Node<T>> In = new List<Node<T>>();
        public float weight;

        public int X, Y;

        public Node()
        {

        }

        public Node(T data)
        {
            Data = data;
        }

        public Node(T data, int x, int y)
        {
            Data = data;
            X = x;
            Y = y;
        }

        public Node(Node<T> source)
        {
            if (source == null)
            {
                return;
            }
            string data = JsonConvert.SerializeObject(source.Data);
            Data = JsonConvert.DeserializeObject<T>(data);
        }

        /// <summary>
        /// This is VERY expensive operation. Only use when absolutely necessary
        /// </summary>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public NodeCompareResult Compare(Node<T> compareTo)
        {
            string js1 = JsonConvert.SerializeObject(this, Formatting.Indented);
            string js2 = JsonConvert.SerializeObject(compareTo, Formatting.Indented);

            return js1 == js2 ? NodeCompareResult.Equal : NodeCompareResult.Different;
        }

        /// <summary>
        /// Get object's resulting Json String
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Few use cases. Mostly use Serialize and Deserialize into an already known type
        /// </summary>
        /// <param name="json">json source</param>
        /// <returns></returns>
        public static Node<T> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Node<T>>(json);
        }

        public async Task<WriteRequestReturn> AppendCurrentNodeStateToDiskFileAsync()
        {
            string path = Application.dataPath + $"/Nodes/{NodeUtility.GetUniqueNodeID()}_Node.json";
            StreamWriter writer = new StreamWriter(path, false);
            try
            {
                await writer.WriteLineAsync(Serialize());

                await writer.FlushAsync();
                writer.Close();
                return new WriteRequestReturn()
                {
                    Error = null,
                    IsSuccessful = true
                };
            }
            catch (Exception e)
            {
                await writer.FlushAsync();
                writer.Close();
                return new WriteRequestReturn()
                {
                    Error = e,
                    IsSuccessful = false
                };
            }
        }
    }

    public enum NodeCompareResult
    {
        Equal,
        Different
    }

    public struct WriteRequestReturn
    {
        public bool IsSuccessful;
        public Exception Error;
    }


}