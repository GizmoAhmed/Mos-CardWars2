using UnityEngine;

namespace PlayerStuff
{
    public class DiscardModal : MonoBehaviour, IModal
    {
        //IModal
        public void CloseModal()
        {
            gameObject.SetActive(false);
        }
    }
}
