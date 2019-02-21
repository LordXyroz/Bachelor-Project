using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script placed on the dropzone (SystemSetupScreen) in order to make objects editable
/// </summary>


public class DropZone : MonoBehaviour, IDropHandler
{
    public List<GameObject> editableSystemComponents = new List<GameObject>();

    public void OnDrop(PointerEventData eventData)
    {
        DraggableObject draggableObject = eventData.pointerDrag.GetComponent<DraggableObject>();
        if (draggableObject != null)
        {
            draggableObject.parentToReturnTo = this.transform;
            editableSystemComponents.Add(draggableObject.gameObject);
        }
    }
    /* private Save CreateSaveScenarioObject()
     {
         Save save = new Save();
         foreach (GameObject targetGameObject in editableSystemComponents)
         {
             GameObject target = targetGameObject.GetComponent<GameObject>();
             if (target != null)
             {
                 save.systemComponentPositions.Add(target.transform.position);
                 //save.systemComponentTypes.Add((int)target.GetComponent<SelectedObject>().type);
             }
         }

         return save;
     }

     private void SaveScenario()
     {
         Save save = CreateSaveScenarioObject();

         BinaryFormatter bf = new BinaryFormatter();
         FileStream file = File.Create(Application.persistentDataPath + "/scenariosave.save");
         bf.Serialize(file, save);
         file.Close();

         Debug.Log("Scenario saved");
     }*/
}

