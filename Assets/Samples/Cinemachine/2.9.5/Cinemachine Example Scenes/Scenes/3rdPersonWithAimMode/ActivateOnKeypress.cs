using UnityEngine;

namespace Samples.Cinemachine._2._9._5.Cinemachine_Example_Scenes.Scenes._3rdPersonWithAimMode
{
    public class ActivateOnKeypress : MonoBehaviour
    {
        public KeyCode ActivationKey = KeyCode.LeftControl;
        public int PriorityBoostAmount = 10;
        public GameObject Reticle;

        global::Cinemachine.CinemachineVirtualCameraBase vcam;
        bool boosted = false;

        void Start()
        {
            vcam = GetComponent<global::Cinemachine.CinemachineVirtualCameraBase>();
        }

        void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (vcam != null)
            {
                if (Input.GetKey(ActivationKey))
                {
                    if (!boosted)
                    {
                        vcam.Priority += PriorityBoostAmount;
                        boosted = true;
                    }
                }
                else if (boosted)
                {
                    vcam.Priority -= PriorityBoostAmount;
                    boosted = false;
                }
            }

            if (Reticle != null)
                Reticle.SetActive(boosted);
#else
        InputSystemHelper.EnableBackendsWarningMessage();
#endif
        }
    }
}