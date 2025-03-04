using System;
using System.Text.Json.Serialization;

namespace wheresmydevice.Models
{
    public class SensorData
    {
        [JsonPropertyName("sensor")]
        public Sensor Sensor { get; set; }
    }

    public class Sensor
    {
        [JsonPropertyName("id")]
        public int DeviceId { get; set; }
        [JsonPropertyName("accel")]
        public Acceleration Accel { get; set; }

        [JsonPropertyName("weather")]
        public Weather Weather { get; set; }

        [JsonPropertyName("gnss")]
        public Gnss Gnss { get; set; }
    }

    public class Acceleration
    {
        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("z")]
        public float Z { get; set; }
    }

    public class Weather
    {
        [JsonPropertyName("co2")]
        public float Co2 { get; set; }

        [JsonPropertyName("hum")]
        public float Humidity { get; set; }

        [JsonPropertyName("iaq")]
        public int Iaq { get; set; }

        [JsonPropertyName("pre")]
        public int Pressure { get; set; }

        [JsonPropertyName("tem")]
        public float Temperature { get; set; }

        [JsonPropertyName("voc")]
        public float Voc { get; set; }
    }

    public class Gnss
    {
        [JsonPropertyName("timeBootMs")]
        public long TimeBootMs { get; set; }

        [JsonPropertyName("lat")]
        public int Latitude { get; set; }

        [JsonPropertyName("lon")]
        public int Longitude { get; set; }

        [JsonPropertyName("alt")]
        public int Altitude { get; set; }

        [JsonPropertyName("relativeAlt")]
        public int RelativeAltitude { get; set; }

        [JsonPropertyName("vx")]
        public int Vx { get; set; }

        [JsonPropertyName("vy")]
        public int Vy { get; set; }

        [JsonPropertyName("vz")]
        public int Vz { get; set; }

        [JsonPropertyName("hdg")]
        public int Heading { get; set; }
    }
}
