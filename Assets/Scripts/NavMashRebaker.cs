using System.Collections;
using NavMeshPlus.Components;
using UnityEngine;

namespace DefaultNamespace
{
    public class NavMashRebaker : MonoBehaviour
    {
        private NavMeshSurface _navMeshSurface;

        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();
            
            GlobalEventManager.OnObstacleFieldsUpdate.AddListener(() => StartCoroutine(UpdateWithDelay()));
        }

        private IEnumerator UpdateWithDelay()
        {
            yield return null;
            _navMeshSurface.BuildNavMesh();
        }
        
        private void Start()
        {
            GlobalEventManager.UpdateObstacleFields();
        }
    }
}