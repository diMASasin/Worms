using System;

public class WaterMediator
{
    private readonly Water _water;

    private bool _shouldIncreaseLevel;

    public WaterMediator(Water water)
    {
        _water = water;
    }

    public void IncreaseLevelIfAllowed()
    {
        if (_shouldIncreaseLevel)
            _water.IncreaseLevel();
    }

    public void AllowIncreaseWaterLevel()
    {
        _shouldIncreaseLevel = true;
    }
}