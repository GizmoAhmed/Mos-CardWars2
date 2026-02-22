using UnityEngine;

namespace Modal
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
