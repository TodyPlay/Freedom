using System.IO;
using System.Text;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Saved
{
    //玩家存档
    [BurstCompile]
    public static class PlayerSaved
    {
        public static void Saving(in Content content)
        {
            const string directoryPath = "Saving/Player";
            Directory.CreateDirectory(directoryPath);

            using var fs = new FileStream($"{directoryPath}/{content.id}.json", FileMode.OpenOrCreate,
                FileAccess.Write);

            var json = JsonUtility.ToJson(content);

            Debug.Log(json);

            fs.Write(Encoding.UTF8.GetBytes(json));
        }

        public static bool Loading(int id, out Content content)
        {
            var path = $"Saving/Player/{id}.json";

            var exists = File.Exists(path);

            if (!exists)
            {
                content = default;
                return false;
            }

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            //读取fs中的json内容，转化为content
            var buffer = new byte[fs.Length];
            var len = fs.Read(buffer, 0, buffer.Length);

            var json = Encoding.UTF8.GetString(buffer, 0, len);

            Debug.Log(json);

            content = JsonUtility.FromJson<Content>(json);

            return true;
        }


        //存档内容
        public struct Content
        {
            public int id;

            public string name;

            public float3 position;
        }
    }
}