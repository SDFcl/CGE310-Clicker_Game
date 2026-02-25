using System.Collections;
using UnityEngine;

public class MainDogScript : MonoBehaviour
{
    public GameObject SpriteObj;
    public float SizeScale = 0.5f;


    private void Awake()
    {
        if (SpriteObj == null) this.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (SpriteObj.transform.localScale.x == (SizeScale * 5))
        {
            return;
        }
        StartCoroutine(changeScale());
    }

    private IEnumerator changeScale()
    {
        SpriteObj.transform.localScale = new Vector3(SpriteObj.transform.localScale.x + SizeScale, SpriteObj.transform.localScale.y + SizeScale, SpriteObj.transform.localScale.z + SizeScale);
        Debug.Log("chanhe scale");
        yield return new WaitForSeconds(0.5f);
        SpriteObj.transform.localScale = new Vector3(SpriteObj.transform.localScale.x - SizeScale, SpriteObj.transform.localScale.y - SizeScale, SpriteObj.transform.localScale.z - SizeScale);
        Debug.Log("chanhe scale down");
    }
}
