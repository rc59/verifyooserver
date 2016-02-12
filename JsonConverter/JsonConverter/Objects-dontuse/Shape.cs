using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter.Objects
{
    [DataContract]
    class Shape
    {
        [DataMember]
        public string _id { get; set; }
        [DataMember]
        public bool Match { get; set; }
        [DataMember]
        public double MatchScore { get; set; }
        [DataMember]
        public double TotalGames { get; set; }
        [DataMember]
        public List<Stroke> Strokes;
        [DataMember]
        public bool IsSource { get; set; }
        [DataMember]
        public double ScreenHeight { get; set; }
        [DataMember]
        public double ScreenWidth { get; set; }
        [DataMember]
        public string GcmToken { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string ModelName { get; set; }
        [DataMember]
        public string DeviceId { get; set; }
        [DataMember]
        public string OS { get; set; }
        [DataMember]
        public string Instruction { get; set; }
        [DataMember]
        public string Created { get; set; }
        [DataMember]
        public double __v { get; set; }

        public string toString()
        {
            string result = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", Created, _id, Name, ModelName, DeviceId, OS, Instruction, Match.ToString(), MatchScore.ToString(), IsSource.ToString(), ScreenHeight.ToString(), ScreenWidth.ToString(), __v.ToString());
            return result;
        }
    }
}
