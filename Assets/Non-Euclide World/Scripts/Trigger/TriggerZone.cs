using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TriggerZone : MonoBehaviour
{
    private List<ITriggerable> _containsComponents = new List<ITriggerable>(5);
    public IReadOnlyList<ITriggerable> ContainsComponents => _containsComponents;

    protected virtual Func<ITriggerable, bool> EnteredComponentIsSuitable { get; } = (ITriggerable component) => true;

    protected virtual void OnEnter(ITriggerable component) { }
    protected virtual void OnExit(ITriggerable component) { }

    private Collider _collider;

    public Collider Collider
    {
        get
        {
            if (_collider is null || !_collider)
                _collider = GetComponent<Collider>();

            return _collider;
        }
    }
    public bool ZoneActive { get; private set; } = true;
    protected virtual bool _refreshEnterdComponentsAfterExit => true;
    protected virtual int _maxContainsCount => int.MaxValue;

    public event Action<ITriggerable> EnterEvent;
    public event Action<ITriggerable> ExitEvent;

    public bool Contains(ITriggerable component) => _containsComponents.Contains(component);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITriggerable component))
        {
            if (_containsComponents.Contains(component))
                return;

            if (!EnteredComponentIsSuitable(component))
                return;

            if (_containsComponents.Count >= _maxContainsCount)
                return;

            _containsComponents.Add(component);

            if (!ZoneActive)
                return;

            OnEnter(component);
            EnterEvent?.Invoke(component);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ITriggerable component))
        {
            if (!_containsComponents.Remove(component))
                return;

            if (!ZoneActive)
                return;

            OnExit(component);
            ExitEvent?.Invoke(component);

            if (_refreshEnterdComponentsAfterExit && _containsComponents.Count > 0)
            {
                OnEnter(_containsComponents.First());
                EnterEvent?.Invoke(_containsComponents.First());
            }
        }
    }

    private void OnDisable()
    {
        for (int i = _containsComponents.Count - 1; i >= 0; i--)
        {
            ITriggerable component = _containsComponents[i];
            _containsComponents.RemoveAt(i);
            OnExit(component);
            ExitEvent?.Invoke(component);
        }
    }

    public void SetZoneActive(bool value)
    {
        if (ZoneActive == value)
            return;

        ZoneActive = value;

        if(ZoneActive)
        {
            foreach (ITriggerable component in _containsComponents)
            {
                OnEnter(component);
                EnterEvent?.Invoke(component);
            }
        }
        else
        {
            foreach (ITriggerable component in _containsComponents)
            {
                OnExit(component);
                ExitEvent?.Invoke(component);
            }
        }
    }
}