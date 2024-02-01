using Neural_Networks;

public class GaDNA 
{
    public readonly LayerDense[] DenseLayers;
    
    private readonly LayerDense _dense1;
    private readonly ActivationFunction _activation1;
    private readonly LayerDense _dense2;
    private readonly ActivationFunction _activation2;
    private readonly LayerDense _dense3;
    private readonly ActivationFunction _activation3;
    private readonly LayerDense _dense4;
    private readonly ActivationFunction _activation4;
    private readonly LayerDense _dense5;
    private readonly ActivationFunction _activation5;
    private readonly LayerDense _dense6;
    private readonly ActivationFunction _activation6;
    
    public GaDNA()
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

        DenseLayers = new[] { _dense1, _dense2, _dense3, _dense4, _dense5, _dense6};
    }

    public float DoAction(float[,] x)
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
}
