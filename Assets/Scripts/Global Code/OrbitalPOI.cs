using UnityEngine;
using System.Collections;

public class OrbitalPOI {

    public GameObject poiIcon;
    private Vector2 offsetVector;

    private Vector2 localOrbitalPosition;
    private SpriteRenderer icon;
    private SpriteRenderer placard;
    private Vector2 scale;
    

    public OrbitalPOI(GameObject poiIcon, Vector2 localOrbitalPosition)
    {
        this.poiIcon = poiIcon;
        this.localOrbitalPosition = localOrbitalPosition;
        
        icon = poiIcon.GetComponent<SpriteRenderer>();
        placard = poiIcon.GetComponentInChildren<SpriteRenderer>();
        
        scale = new Vector3(GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST/2, GlobalElements.zoomLevel / GlobalElements.UI_SCALE_CONST/2, 0);

        float width = placard.sprite.bounds.size.x / 2;
        float height = placard.sprite.bounds.size.y / 2;
        
        this.poiIcon.transform.localScale = scale;
        offsetVector = this.localOrbitalPosition.normalized * Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));
        placard.transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(offsetVector.y, offsetVector.x) * Mathf.Rad2Deg - 45, Vector3.forward);
    }

    public GameObject getPoiIcon()
    {
        return poiIcon;
    }
    
    public Vector2 getLocalPosition()
    {
        return localOrbitalPosition;
    }

    public Vector2 getOffsetVector()
    {
        return offsetVector;
    }

    //sets scale and updates other values accordingly
    public void setScale(Vector2 scale)
    {
        this.scale = scale;
        poiIcon.transform.localScale = scale;
        float width = (placard.sprite.bounds.max.x - placard.sprite.bounds.min.x) * scale.x; 
        float height = (placard.sprite.bounds.max.y - placard.sprite.bounds.min.y) * scale.y;
        offsetVector = this.localOrbitalPosition.normalized * Mathf.Sqrt(Mathf.Pow(width / 2, 2) + Mathf.Pow(height / 2, 2));
    }

    public Vector2 getScale()
    {
        return scale;
    }
}
