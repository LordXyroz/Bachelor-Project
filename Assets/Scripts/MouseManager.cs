using UnityEngine;

public class MouseManager : MonoBehaviour
{

    /// <summary>
    /// 
    /// Will only detect objects that has a collider
    /// 
    /// </summary>
    public GameObject selectedObject;

    private void Start()
    {

    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;
        //RaycastHit[] hits = Physics.RaycastAll(ray);

        if (Physics.Raycast(ray, out hitInfo))
        {
            /// The object hit may not be root, transform.root gives the "root-est" object hit. 
            /// If this does not work, might need to use transform.parent
            GameObject hitObject = hitInfo.transform.root.gameObject;

            //Debug.Log("Mouse is over: " + hitObject.name);
            SelectObject(hitObject);
        }
        else
        {
            ClearSelection();
        }

        void SelectObject(GameObject obj)
        {
            if (selectedObject != null)
            {
                if (obj == selectedObject)
                {
                    return;
                }
            }
            selectedObject = obj;
        }

        void ClearSelection()
        {
            selectedObject = null;
        }
    }
}
