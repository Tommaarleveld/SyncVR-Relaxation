using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SyncVR.DLC
{
    public class DLCBundle : IEquatable<DLCBundle>
    {
        public string name;
        public string category;
        public string path;
        public bool has_dll;
        public long byteSize;

        public DLCBundle ()
        {
            name = "";
            category = "";
            path = "";
            has_dll = false;
            byteSize = 0;
        }

        public bool Equals (DLCBundle other)
        {
            return (NameNoExtension() == other.NameNoExtension() && category == other.category);
        }

        public override string ToString ()
        {
            return (category + " - " + name).Replace(".unity3d","");
        }

        public string NameNoExtension ()
        {
            return name.Replace(name.Substring(name.LastIndexOf('.')), "");
        }

        public string DLLPath ()
        {
            return path.Replace(".unity3d", ".dll");
        }

        public string ZipPath ()
        {
            return path.Replace(".unity3d", ".zip");
        }

        public bool IsZip ()
        {
            return name.Contains(".zip");
        }
    }
}