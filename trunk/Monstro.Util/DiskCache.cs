using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Monstro.Util
{
    public class DiskCache
    {
        private readonly String _path;

        public DiskCache(String path)
        {
            Directory.CreateDirectory(path);   
            _path = path;
        }

        private String GetFilePath(String key)
        {
            if (key.Length > 150)
                key = key.Substring(0, 100) + "..." + Crypto.MD5Hex(key);
            return Path.Combine(_path, Util.CleanFileName(key));
        }

        public void Add(string key, byte[] content)
        {
            File.WriteAllBytes(GetFilePath(key), content);
        }

        public void Remove(string key)
        {
            var path = GetFilePath(key);
            if (File.Exists(path))
                File.Delete(path);
        }

        public byte[] Get(String key)
        {
            String path = GetFilePath(key);
            if (!File.Exists(path))
                return null;
            using (var stream = File.OpenRead(path))
            {
                return Util.StreamToBytes(stream);
            }
        }
    }
}
