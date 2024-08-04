using Cinemachine;
using UnityEngine;

namespace Samples.Cinemachine._2._9._5.Cinemachine_Example_Scenes.Shared.Scripts
{

[AddComponentMenu("")] // Don't display in add component menu
public class ActivateCamOnPlay : MonoBehaviour
{
    public CinemachineVirtualCameraBase vcam;

	// Use this for initialization
	void Start () 
    {
	    if (vcam)
	    {
	        vcam.MoveToTopOfPrioritySubqueue();
	    }
	}
}

}