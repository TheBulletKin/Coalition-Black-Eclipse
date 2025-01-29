using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiEntity : MonoBehaviour
{
    AiCommandListener commandListener;

	private void Start()
	{
		commandListener = GetComponent<AiCommandListener>();
	}
}
