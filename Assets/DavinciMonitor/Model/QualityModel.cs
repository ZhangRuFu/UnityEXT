using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityModel : IModel
{
    public int Level { get; private set; }

    public override void Init()
    {
        Level = QualitySettings.GetQualityLevel();
    }

    public override void Update()
    {
        
    }
}
