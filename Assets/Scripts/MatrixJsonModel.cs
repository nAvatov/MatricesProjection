using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DefaultNamespace
{
    [Serializable]
    public class MatrixJsonModel
    {
        public float m00 { get; set; }
        public float m01 { get; set; }
        public float m02 { get; set; }
        public float m03 { get; set; }
        public float m10 { get; set; }
        public float m11 { get; set; }
        public float m12 { get; set; }
        public float m13 { get; set; }
        public float m20 { get; set; }
        public float m21 { get; set; }
        public float m22 { get; set; }
        public float m23 { get; set; }
        public float m30 { get; set; }
        public float m31 { get; set; }
        public float m32 { get; set; }
        public float m33 { get; set; }
    }

    [Serializable]
    public class MatricesDataJsonModel 
    {
        [JsonProperty("Matrices")]
        public List<MatrixJsonModel> Matrices { get; set; }    
    }
}