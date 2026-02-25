using UnityEngine;
using UnityEngine.Pool;

public class FloatingTextPool : MonoBehaviour
{
    public FloatingText prefab;
    public int defaultCapacity = 50;
    public int maxSize = 200;

    IObjectPool<FloatingText> pool;

    void Awake()
    {
        pool = new ObjectPool<FloatingText>(
            CreateObject,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            true,
            defaultCapacity,
            maxSize
        );
    }

    FloatingText CreateObject()
    {
        FloatingText obj = Instantiate(prefab, transform);
        obj.SetPool(pool);
        obj.gameObject.SetActive(false);
        return obj;
    }

    void OnTakeFromPool(FloatingText obj)
    {
        obj.gameObject.SetActive(true);
    }

    void OnReturnedToPool(FloatingText obj)
    {
        obj.gameObject.SetActive(false);
    }

    void OnDestroyPoolObject(FloatingText obj)
    {
        Destroy(obj.gameObject);
    }

    public FloatingText Get()
    {
        return pool.Get();
    }
}