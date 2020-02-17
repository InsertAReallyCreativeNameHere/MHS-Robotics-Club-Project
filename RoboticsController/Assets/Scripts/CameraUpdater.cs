using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CameraUpdater : MonoBehaviour
{
    public GameObject trackedObject;
    [SerializeField] new Camera camera;

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        camera.transform.parent.position = trackedObject.transform.position;
        Thread thread = new Thread(async () => await LateCameraUpdate(trackedObject.transform));
        thread.Start(trackedObject.transform);
    }

    public async Task LateCameraUpdate(Transform transformOfTrackedObj)
    {
        await Task.Delay(2000);
        camera.transform.parent.localRotation = Quaternion.Euler(0, transformOfTrackedObj.localEulerAngles.y, 0);
    }
}
