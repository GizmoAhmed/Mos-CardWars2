using Modal;
using PlayerStuff;
using UnityEngine;

namespace Buttons
{
    public class XButton : MonoBehaviour
    {
        private GameObject _parent;
    
        void Start()
        {
            _parent = transform.parent.gameObject;
        }
    
        public void ExitOnClick()
        {
            IModal modal = _parent.GetComponent<IModal>();
            
            modal.CloseModal();
        }
    }
}
