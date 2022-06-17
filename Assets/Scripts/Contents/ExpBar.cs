using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpBar : MonoBehaviour
{
    [SerializeField]
    Transform _expBar = null;

    private void Start()
    {

    }

    public void SetExpBar(float ratio)
	{
        
        ratio = Mathf.Clamp(ratio, 0, 1);
        _expBar.localScale = new Vector3(ratio, 1, 1);
	}
}
