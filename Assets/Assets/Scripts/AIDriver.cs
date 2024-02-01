using CarUtils;
using DrivingData;
using Neural_Networks;
using UnityEngine;

public class AIDriver : MonoBehaviour
{
    [SerializeField] private string modelToLoad;

    private CarPhysics _carPhysics;
    private DataGatherer _dataGatherer;

    private LayerDense _dense1;
    private ActivationFunction _activation1;
    private LayerDense _dense2;
    private ActivationFunction _activation2;
    private LayerDense _dense3;
    private ActivationFunction _activation3;
    private LayerDense _dense4;
    private ActivationFunction _activation4;
    private LayerDense _dense5;
    private ActivationFunction _activation5;
    private LayerDense _dense6;
    private ActivationFunction _activation6;
    private LayerDense _dense7;
    private ActivationFunction _activation7;

    private float _countTime;
    private int _failed;
    
    void Start()
    {
        //LoadModel1();
        //LoadModel2();
        //LoadModel3();
        LoadModel4();
        //LoadModel5();

        _carPhysics = GetComponent<CarPhysics>();
        _dataGatherer = GetComponent<DataGatherer>();
    }

    private void FixedUpdate()
    {
        var currentFeatures = _dataGatherer.GatherData();
        //var currentFeatures = _dataGatherer.GatherDataNormalized();
        var x =  new float[1, currentFeatures.Length];
        for (int i = 0; i < currentFeatures.Length; i++)
        {
            x[0, i] = currentFeatures[i];
        }

        var modelPrediction = RunModel4(x);
        var steerValue = Mathf.Clamp(modelPrediction, -1.0f, 1.0f);
        //var steerValue = Mathf.Clamp(RunModel5(x), -1.0f, 1.0f);
        _carPhysics.MoveWithCustomPhysics(1.0f,  steerValue);
        //print(modelPrediction);
    
    // if (modelPrediction < -1 || modelPrediction > 1)
    // {
    //     print(modelPrediction);
    // }
    //
    // if (_countTime <= 20)
    //     _countTime++;
    }

    private void LoadModel1()
    {
        _dense1 = new LayerDense(21, 64);
        _activation1 = new ActivationReLu();

        _dense2 = new LayerDense(64, 64);
        _activation2 = new ActivationReLu();

        _dense3 = new LayerDense(64, 64);
        _activation3 = new ActivationReLu();

        _dense4 = new LayerDense(64, 1);
        _activation4 = new ActivationLinear();

        ModelSaver.LoadModel(modelToLoad, _dense1, _dense2, _dense3, _dense4);
    }

    private float RunModel1(float[,] x)
    {
        _dense1.Forward(x);
        ((ActivationReLu)_activation1).Forward(_dense1.Output);
        _dense2.Forward(_activation1.Output);
        ((ActivationReLu)_activation2).Forward(_dense2.Output);
        _dense3.Forward(_activation2.Output);
        ((ActivationReLu)_activation3).Forward(_dense3.Output);
        _dense4.Forward(_activation3.Output);
        ((ActivationLinear)_activation4).Forward(_dense4.Output);
        
       return _activation4.Output[0, 0];
    }
    
    private void LoadModel2()
    {
        _dense1 = new LayerDense(21, 128);
        _activation1 = new ActivationReLu();

        _dense2 = new LayerDense(128, 128);
        _activation2 = new ActivationReLu();

        _dense3 = new LayerDense(128, 1);
        _activation3 = new ActivationLinear();

        ModelSaver.LoadModel(modelToLoad, _dense1, _dense2, _dense3, _dense4);
    }
    
    private float RunModel2(float[,] x)
    {
        _dense1.Forward(x);
        ((ActivationReLu)_activation1).Forward(_dense1.Output);
        _dense2.Forward(_activation1.Output);
        ((ActivationReLu)_activation2).Forward(_dense2.Output);
        _dense3.Forward(_activation2.Output);
        ((ActivationLinear)_activation3).Forward(_dense3.Output);

        return _activation3.Output[0, 0];
    }
    
    private void LoadModel3()
    {
        _dense1 = new LayerDense(21, 64);
        _activation1 = new ActivationReLu();

        _dense2 = new LayerDense(64, 64);
        _activation2 = new ActivationReLu();

        _dense3 = new LayerDense(64, 64);
        _activation3 = new ActivationReLu();

        _dense4 = new LayerDense(64, 64);
        _activation4 = new ActivationReLu();

        _dense5 = new LayerDense(64, 1);
        _activation5 = new ActivationLinear();
        
        ModelSaver.LoadModel(modelToLoad, _dense1, _dense2, _dense3, _dense4, _dense5);
    }

    private float RunModel3(float[,] x)
    {
        _dense1.Forward(x);
        ((ActivationReLu)_activation1).Forward(_dense1.Output);
        _dense2.Forward(_activation1.Output);
        ((ActivationReLu)_activation2).Forward(_dense2.Output);
        _dense3.Forward(_activation2.Output);
        ((ActivationReLu)_activation3).Forward(_dense3.Output);
        _dense4.Forward(_activation3.Output);
        ((ActivationReLu)_activation4).Forward(_dense4.Output);
        _dense5.Forward(_activation4.Output);
        ((ActivationLinear)_activation5).Forward(_dense5.Output);
        
        return _activation5.Output[0, 0];
    }
    
    private void LoadModel4()
    {
        _dense1 = new LayerDense(21, 64);
        _activation1 = new ActivationReLu();

        _dense2 = new LayerDense(64, 64);
        _activation2 = new ActivationReLu();

        _dense3 = new LayerDense(64, 64);
        _activation3 = new ActivationReLu();

        _dense4 = new LayerDense(64, 64);
        _activation4 = new ActivationReLu();

        _dense5 = new LayerDense(64, 64);
        _activation5 = new ActivationReLu();
        
        _dense6 = new LayerDense(64, 1);
        _activation6 = new ActivationLinear();
        
        ModelSaver.LoadModel(modelToLoad, _dense1, _dense2, _dense3, _dense4, _dense5, _dense6);
    }

    private float RunModel4(float[,] x)
    {
        _dense1.Forward(x);
        ((ActivationReLu)_activation1).Forward(_dense1.Output);
        _dense2.Forward(_activation1.Output);
        ((ActivationReLu)_activation2).Forward(_dense2.Output);
        _dense3.Forward(_activation2.Output);
        ((ActivationReLu)_activation3).Forward(_dense3.Output);
        _dense4.Forward(_activation3.Output);
        ((ActivationReLu)_activation4).Forward(_dense4.Output);
        _dense5.Forward(_activation4.Output);
        ((ActivationReLu)_activation5).Forward(_dense5.Output);
        _dense6.Forward(_activation5.Output);
        ((ActivationLinear)_activation6).Forward(_dense6.Output);
        
        return _activation6.Output[0, 0];
    }
    
    private void LoadModel5()
    {
        _dense1 = new LayerDense(21, 64);
        _activation1 = new ActivationReLu();

        _dense2 = new LayerDense(64, 64);
        _activation2 = new ActivationReLu();

        _dense3 = new LayerDense(64, 64);
        _activation3 = new ActivationReLu();

        _dense4 = new LayerDense(64, 64);
        _activation4 = new ActivationReLu();

        _dense5 = new LayerDense(64, 64);
        _activation5 = new ActivationReLu();
        
        _dense6 = new LayerDense(64, 64);
        _activation6 = new ActivationReLu();
        
        _dense7 = new LayerDense(64, 1);
        _activation7 = new ActivationLinear();
        
        ModelSaver.LoadModel(modelToLoad, _dense1, _dense2, _dense3, _dense4, _dense5, _dense6, _dense7);
    }
    
    private float RunModel5(float[,] x)
    {
        _dense1.Forward(x);
        ((ActivationReLu)_activation1).Forward(_dense1.Output);
        _dense2.Forward(_activation1.Output);
        ((ActivationReLu)_activation2).Forward(_dense2.Output);
        _dense3.Forward(_activation2.Output);
        ((ActivationReLu)_activation3).Forward(_dense3.Output);
        _dense4.Forward(_activation3.Output);
        ((ActivationReLu)_activation4).Forward(_dense4.Output);
        _dense5.Forward(_activation4.Output);
        ((ActivationReLu)_activation5).Forward(_dense5.Output);
        _dense6.Forward(_activation5.Output);
        ((ActivationReLu)_activation6).Forward(_dense6.Output);
        _dense7.Forward(_activation6.Output);
        ((ActivationLinear)_activation7).Forward(_dense7.Output);
        
        return _activation7.Output[0, 0];
    }
}