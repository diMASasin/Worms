using UnityEngine;

namespace Samples.Cinemachine._2._9._5.Cinemachine_Example_Scenes.Scenes.Anywhere_Door
{
    public class TagChanger : MonoBehaviour
    {
        public void PlayerTagChanger()
        {
            this.tag = "Player";
        }

        public void UntaggedTagChanger()
        {
            this.tag = "Untagged";
        }
    }
}