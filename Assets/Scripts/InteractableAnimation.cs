using UnityEngine;

public class InteractableAnimation : MonoBehaviour, IInteract
{
    [SerializeField] Animator _animator;
    [SerializeField] string _parameterName;
    private bool _parameterValue;

    private void Start()
    {
        _parameterValue = false;
    }

    public void Interact()
    {
        _parameterValue = !_parameterValue;
        _animator.SetBool(_parameterName, _parameterValue);
    }
}
