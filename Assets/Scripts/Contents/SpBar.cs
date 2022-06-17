using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpBar : MonoBehaviour
{
    [SerializeField]
    Transform _spBar = null;

    private void Start()
    {

    }

    public void SetSpBar(float ratio)
	{
        
        ratio = Mathf.Clamp(ratio, 0, 1);
        _spBar.localScale = new Vector3(ratio, 1, 1);
	}
}
