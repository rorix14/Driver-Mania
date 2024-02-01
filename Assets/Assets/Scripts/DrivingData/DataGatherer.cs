using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CarUtils;
using Unity.Collections;
using UnityEngine;

namespace DrivingData
{
    public class DataGatherer : MonoBehaviour
    {
        [SerializeField] private RaceCheckPoint currentCheckPoint;
        [SerializeField] private int numOfRaysPerSide;
        [SerializeField] private float rayDistance;
        [SerializeField] private LayerMask wallMask;
        [SerializeField] private string savedFileName;
        [SerializeField] private bool saveData;

        // only used for testing and visualisation;
        [NonSerialized] public Vector3[] ConvertedDirections;

        private Vector3[] _directions;
        private CarPhysics _carPhysics;

        private float[] _currentFeatures;
        private List<DriveFeatures> _gatheredData;
        private List<DriveLabels> _inputsData;
        private DataNormalizer _dataNormalizer;

        private RaceCheckPoint _startCheckPoint;
        
        public event Action<RaceCheckPoint> OnCheckpointChangedEvent;
        
        private NativeArray<RaycastCommand> _wallHitChecks;
        private NativeArray<RaycastHit> _wallHitResults;

        public RaceCheckPoint CurrentCheckPoint => currentCheckPoint;
        public RaceCheckPoint StartCheckPoint => _startCheckPoint;
        public LayerMask WallMask => wallMask;
        public float RayDistance => rayDistance;

        private void Awake()
        {
            _startCheckPoint = currentCheckPoint;
            _carPhysics = GetComponent<CarPhysics>();
            _dataNormalizer = new DataNormalizer((0, 0));

            _gatheredData = new List<DriveFeatures>();
            _inputsData = new List<DriveLabels>();

            _directions = new Vector3[numOfRaysPerSide * 2 + 1];
            ConvertedDirections = new Vector3[_directions.Length];

            var forward = Vector3.forward;
            _directions[0] = forward;

            for (int i = 1; i <= numOfRaysPerSide; i++)
            {
                var angle = (float)i / numOfRaysPerSide * 90;
                _directions[i * 2 - 1] = Quaternion.Euler(0, -angle, 0) * forward;
                _directions[i * 2] = Quaternion.Euler(0, angle, 0) * forward;
            }

            _wallHitChecks = new NativeArray<RaycastCommand>(_directions.Length, Allocator.Persistent);
            _wallHitResults = new NativeArray<RaycastHit>(_directions.Length, Allocator.Persistent);
        }

        public void ResetCar()
        {
            currentCheckPoint = _startCheckPoint;
            _gatheredData = new List<DriveFeatures>();
            _inputsData = new List<DriveLabels>();
        }

        public float[] GatherData()
        {
            var carTransform = transform;
            var capturedFeatures = new float[_directions.Length + 8];
            for (int i = 0; i < _directions.Length; i++)
            {
                ConvertedDirections[i] = carTransform.localRotation * _directions[i];
                Physics.Raycast(carTransform.position, ConvertedDirections[i], out RaycastHit hit, rayDistance,
                    wallMask);
                capturedFeatures[i] = hit.distance;

                // _wallHitChecks[i] = new RaycastCommand(transform.position, ConvertedDirections[i], rayDistance, wallMask);
            }
            
            // var job = RaycastCommand.ScheduleBatch(_wallHitChecks, _wallHitResults, 1);
            // job.Complete();
            // for (int i = 0; i < _wallHitResults.Length; i++)
            // {
            //     capturedFeatures[i] = _wallHitResults[i].distance;
            // }

            for (int i = 0; i < 3; i++)
            {
                capturedFeatures[_directions.Length + i] = _carPhysics.Velocity[i];
                capturedFeatures[_directions.Length + i + 3] = _carPhysics.Acceleration[i];
            }
            
            capturedFeatures[_directions.Length + 6] = (float)currentCheckPoint.GetCornerType;
            capturedFeatures[_directions.Length + 7] =
                Vector3.SignedAngle(carTransform.forward, currentCheckPoint.GetCornerDir, Vector3.up);

            // capturedFeatures[_directions.Length] = _carPhysics.Velocity.x;
            //
            // capturedFeatures[_directions.Length + 1] = _carPhysics.Velocity.z;
            //
            // capturedFeatures[_directions.Length + 2] = _carPhysics.Acceleration.x;
            //
            // capturedFeatures[_directions.Length + 3] = _carPhysics.Acceleration.z;
            //
            // capturedFeatures[_directions.Length + 4] = (float)currentCheckPoint.GetCornerType;
            //
            // capturedFeatures[_directions.Length + 5] =
            //     Vector3.SignedAngle(carTransform.forward, currentCheckPoint.GetCornerDir, Vector3.up);
            //
            // capturedFeatures[_directions.Length + 6] = (float)currentCheckPoint.NextCheckPoint.GetCornerType;
            //
            // capturedFeatures[_directions.Length + 7] = Vector3.Distance(transform.position,
            //     currentCheckPoint.NextCheckPoint.transform.position);

            if (saveData)
            {
                _gatheredData.Add(new DriveFeatures(capturedFeatures));
            }

            return capturedFeatures;
        }

        public void GatherInputs(DriveLabels inputs)
        {
            if (saveData)
            {
                _inputsData.Add(inputs);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var newCheckPoint = other.GetComponent<RaceCheckPoint>();
            OnCheckpointChangedEvent?.Invoke(newCheckPoint);
            currentCheckPoint = newCheckPoint ? newCheckPoint : currentCheckPoint;
        }
        

        private void OnDestroy()
        {
            _wallHitChecks.Dispose();
            _wallHitResults.Dispose();
            
            if (!saveData)
                return;

            FileHandler.SaveToJSON(_gatheredData, "Assets/SavedData/Datasets/" + savedFileName + ".json");
            FileHandler.SaveToJSON(_inputsData, "Assets/SavedData/Datasets/" + savedFileName + "Labels.json");

            FeaturesToCSV();
            LablesToCSV();
            print("Saved Data!!" + _gatheredData.Count);
        }

        private void FeaturesToCSV()
        {
            var fileStream =
                new FileStream(Application.dataPath + "/Assets/SavedData/Datasets/" + savedFileName + ".csv",
                    FileMode.Create);
            using var writer = new StreamWriter(fileStream);

            var fileFeaturesNames = "";
            for (int i = 0; i < _directions.Length; i++)
            {
                fileFeaturesNames += "Dist" + (i + 1) + ",";
            }

            writer.WriteLine(fileFeaturesNames +
                             "velX,velZ,AccX,AccZ,CornerType,AngleBetween,NextCorner,DistToNext");

            foreach (var data in _gatheredData)
            {
                var fileLine = "";
                for (int i = 0; i < data.features.Length; i++)
                {
                    fileLine += data.features[i].ToString(CultureInfo.InvariantCulture);
                    fileLine += i < data.features.Length - 1 ? "," : "";
                }

                writer.WriteLine(fileLine);
            }
        }

        private void LablesToCSV()
        {
            var fileStream =
                new FileStream(Application.dataPath + "/Assets/SavedData/Datasets/" + savedFileName + "Labels.csv",
                    FileMode.Create);
            using var writer = new StreamWriter(fileStream);

            writer.WriteLine("Steer, Accelerate");

            foreach (var input in _inputsData)
            {
                writer.WriteLine(input.inputSteer.ToString(CultureInfo.InvariantCulture) + "," +
                                 input.inputThrottle.ToString(CultureInfo.InvariantCulture));
            }
        }

        public float[] GatherDataNormalized()
        {
            var carTransform = transform;
            var capturedFeatures = new float[_directions.Length + 8];
            for (int i = 0; i < _directions.Length; i++)
            {
                ConvertedDirections[i] = carTransform.localRotation * _directions[i];

                Physics.Raycast(carTransform.position, ConvertedDirections[i], out RaycastHit hit, rayDistance,
                    wallMask);

                capturedFeatures[i] = _dataNormalizer.Remap(hit.distance, _dataNormalizer.RayDistance);
            }

            // for (int i = 0; i < 3; i++)
            // {
            //     capturedFeatures[_directions.Length + i] = _carPhysics.Velocity[i];
            //     capturedFeatures[_directions.Length + i + 3] = _carPhysics.Acceleration[i];
            // }
            //
            // capturedFeatures[_directions.Length + 6] = (float)currentCheckPoint.GetCornerType;
            // capturedFeatures[_directions.Length + 7] =
            //     Vector3.SignedAngle(carTransform.forward, currentCheckPoint.GetCornerDir, Vector3.up);

            capturedFeatures[_directions.Length] = _dataNormalizer.Remap(_carPhysics.Velocity.x, _dataNormalizer.Vel);

            capturedFeatures[_directions.Length + 1] =
                _dataNormalizer.Remap(_carPhysics.Velocity.z, _dataNormalizer.Vel);

            capturedFeatures[_directions.Length + 2] =
                _dataNormalizer.Remap(_carPhysics.Acceleration.x, _dataNormalizer.Acc);

            capturedFeatures[_directions.Length + 3] =
                _dataNormalizer.Remap(_carPhysics.Acceleration.z, _dataNormalizer.Acc);

            capturedFeatures[_directions.Length + 4] =
                _dataNormalizer.Remap((float)currentCheckPoint.GetCornerType, _dataNormalizer.CornerType);

            capturedFeatures[_directions.Length + 5] = _dataNormalizer.Remap(
                Vector3.SignedAngle(carTransform.forward, currentCheckPoint.GetCornerDir, Vector3.up),
                _dataNormalizer.Angle);

            capturedFeatures[_directions.Length + 6] =
                _dataNormalizer.Remap((float)currentCheckPoint.NextCheckPoint.GetCornerType,
                    _dataNormalizer.CornerType);

            capturedFeatures[_directions.Length + 7] = _dataNormalizer.Remap(Vector3.Distance(transform.position,
                currentCheckPoint.NextCheckPoint.transform.position), _dataNormalizer.Distance);

            if (saveData)
            {
                _gatheredData.Add(new DriveFeatures(capturedFeatures));
            }

            return capturedFeatures;
        }
    }
}

// testing ranges
// private float maxRayDist = 0;
// private float minRayDist = Mathf.Infinity;
// private float maxVelX = 0;
// private float minVelX = Mathf.Infinity;
// private float maxVelZ = 0;
// private float minVelZ = Mathf.Infinity;
// private float maxAccX = 0;
// private float minAccX = Mathf.Infinity;
// private float maxAccZ = 0;
// private float minAccZ = Mathf.Infinity;
// private float maxAngle = 0;
// private float minAngle = Mathf.Infinity;
// private float maxDist = 0;
// private float minDist = Mathf.Infinity;

// if (hit.distance > maxRayDist)
//     maxRayDist = hit.distance;
// if (hit.distance < minRayDist)
//     minRayDist = hit.distance;


// if (_carPhysics.Velocity.x > maxVelX)
//     maxVelX = _carPhysics.Velocity.x;
// if (_carPhysics.Velocity.x < minAccX)
//     minVelX = _carPhysics.Velocity.x;
//
// if (_carPhysics.Velocity.z > maxVelZ)
//     maxVelZ = _carPhysics.Velocity.z;
// if (_carPhysics.Velocity.z < minVelZ)
//     minVelZ = _carPhysics.Velocity.z;
//
// if (_carPhysics.Acceleration.x > maxAccX)
//     maxAccX = _carPhysics.Acceleration.x;
// if (_carPhysics.Acceleration.x < minAccX)
//     minAccX = _carPhysics.Acceleration.x;
//
// if (_carPhysics.Acceleration.z > maxAccZ)
//     maxAccZ = _carPhysics.Acceleration.z;
// if (_carPhysics.Acceleration.z < minAccZ)
//     minAccZ = _carPhysics.Acceleration.z;
//
// if (capturedFeatures[_directions.Length + 5] > maxAngle)
//     maxAngle = capturedFeatures[_directions.Length + 5];
// if (capturedFeatures[_directions.Length + 5] < minAngle)
//     minAngle = capturedFeatures[_directions.Length + 5];
//
// if (capturedFeatures[_directions.Length + 7] > maxDist)
//     maxDist = capturedFeatures[_directions.Length + 7];
// if (capturedFeatures[_directions.Length + 7] < minDist)
//     minDist = capturedFeatures[_directions.Length + 7]

// print(
//     $"maxRay {maxRayDist}, minRay {minRayDist}, maxValX {maxVelX}, minValX {minVelX}, " +
//     $"maxVelZ {maxVelZ}, minVelZ {minVelZ}, maxAccX {maxAccX}, minAccX {minAccX}, maxAccZ {maxAccZ}, minAccZ {minAccZ}," +
//     $"maxAngle {maxAngle}, minAngle {minAngle}, maxDist {maxDist}, minDist {minDist}");


// foreach (var wallDistance in data.wallDistances)
// {
//     fileLine += wallDistance.ToString(CultureInfo.InvariantCulture) + ",";
// }
//
// for (int i = 0; i < 3; i++)
// {
//     fileLine += data.velocity[i].ToString(CultureInfo.InvariantCulture) + ",";
// }
//
// for (int i = 0; i < 3; i++)
// {
//     fileLine += data.acceleration[i].ToString(CultureInfo.InvariantCulture) + ",";
// }
//
// writer.WriteLine(fileLine + (int)data.cornerType + "," +
//                  data.angleBetweenCarAndTrack.ToString(CultureInfo.InvariantCulture));