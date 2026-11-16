using UnityEngine;

//Moves each map marker to match its object's real position on the terrain
//The map image is assumed to show exactly the whole terrain, north (+Z) up.

public class MinimapUI : MonoBehaviour
{
    [System.Serializable]
    public class MapMarker{
        // the real object
        public Transform target;
        //marker on the map
        public RectTransform icon;
        //tick for the player; will rotate
        public bool rotateWithTarget;
    }

    public Terrain terrain;
    public RectTransform mapArea;
    public MapMarker[] markers;

    private Vector3 worldMin;             // terrain corner (smallest x, z)
    private Vector3 worldSize;            // terrain width / length


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        worldMin = terrain.transform.position;
        worldSize = terrain.terrainData.size;

    }

    // LateUpdate = after everything has moved this frame
    void LateUpdate(){
        Vector2 mapSize = mapArea.rect.size;
        foreach (MapMarker m in markers){
            if (m.target == null || m.icon == null) continue;


            // 0..1 across the terrain
            float u = Mathf.Clamp01((m.target.position.x - worldMin.x) / worldSize.x);
            float v = Mathf.Clamp01((m.target.position.z - worldMin.z) / worldSize.z);

            // convert to map coordinates (centre of the map = 0,0)
            m.icon.anchoredPosition = new Vector2((u - 0.5f) * mapSize.x, (v - 0.5f) * mapSize.y);

            if (m.rotateWithTarget)
                m.icon.localEulerAngles = new Vector3(0f, 0f, -m.target.eulerAngles.y);
        }

    }
}
