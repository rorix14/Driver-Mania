using System;
using UnityEngine;

namespace DrivingData
{
    // Struct currently not used 
    [Serializable]
    public struct DriveInputs
    {
        public float[] wallDistances;
        public Vector3 velocity;
        public Vector3 acceleration;
        public CornerType cornerType;
        public float angleBetweenCarAndTrack;

        public DriveInputs(float[] wallDistances, Vector3 velocity, Vector3 acceleration, CornerType cornerType,
            float angleBetweenCarAndTrack)
        {
            this.wallDistances = wallDistances;
            this.velocity = velocity;
            this.acceleration = acceleration;
            this.cornerType = cornerType;
            this.angleBetweenCarAndTrack = angleBetweenCarAndTrack;
        }
    }

    [Serializable]
    public struct DataNormalizer
    {
        public (float max, float min) RayDistance;
        public (float max, float min) Vel;
        public (float max, float min) Acc;
        public (float max, float min) CornerType;
        public (float max, float min) Angle;
        public (float max, float min) Distance;
        
        public DataNormalizer((float max, float min) rayDistance)
        {
            RayDistance = (120, 0);
            Vel = (23.5f, -23.5f);
            Acc = (10, -10);
            CornerType = (1, 4);
            Angle = (150, -150);
            Distance = (70, 2);
        }

        public float Remap(float x, (float max, float min) from, float toMin = -1.0f, float toMax = 1.0f)
        {
            var (fromMax, fromMin) = @from;
            return (x - fromMin) * (toMax - toMin) / (fromMax - fromMin) + toMin;
        }
    }

    [Serializable]
    public struct DriveFeatures
    {
        public float[] features;

        public DriveFeatures(float[] features)
        {
            this.features = features;
        }
    }

    [Serializable]
    public struct DriveLabels
    {
        public float inputSteer;
        public float inputThrottle;

        public DriveLabels(float inputSteer, float inputThrottle)
        {
            this.inputSteer = inputSteer;
            this.inputThrottle = inputThrottle;
        }
    }
}