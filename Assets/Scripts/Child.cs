using System.Collections;
using UnityEngine;

public class Child : MonoBehaviour
{
    private IParent _parent;
    [SerializeField] float delay;

    private void Start()
    {
        _parent = GetComponentInParent<IParent>();
    }

    public void ParentMethod()
    {
        _parent.ParentAction();
    }

    public void ParentDeath()
    {
        StartCoroutine(delayCorutine());
    }

    private IEnumerator delayCorutine()
    {
        yield return new WaitForSeconds(delay);
        _parent.ParentDeath();
    }
}
