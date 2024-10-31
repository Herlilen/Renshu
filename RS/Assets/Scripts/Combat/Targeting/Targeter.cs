using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [SerializeField] private CinemachineTargetGroup _cinemachineTargetGroup;
    
    private Camera _mainCamera;
    
    public List<Target> targets = new List<Target>();

    public Target currentTarget {get; private set;}

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Add(target);
        target.OnDestroyed += RemoveTarget;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        
        RemoveTarget(target);
    }

    public bool SelectTarget()
    {
        if(targets.Count == 0) { return false; }
        
        Target closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (Target target in targets)
        {
            Vector2 viewPos = _mainCamera.WorldToViewportPoint(target.transform.position);
            
            if (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1) { continue; }
            
            Vector2 toCenter = viewPos - new Vector2(0.5f, 0.5f);
            
            if (toCenter.magnitude < closestDistance)
            {
                closestTarget = target;
                closestDistance = toCenter.magnitude;
            }
        }
        
        if(closestTarget == null) { return false; }
        
        currentTarget = closestTarget;
        _cinemachineTargetGroup.AddMember(currentTarget.transform, 1, 2);
        
        return true;
    }
    
    public void ClearTarget()
    {
        if(currentTarget == null) { return; }
        
        _cinemachineTargetGroup.RemoveMember(currentTarget.transform);
        
        currentTarget = null;
    }
    
    private void RemoveTarget(Target target)
    {
        if (currentTarget == target)
        {
            _cinemachineTargetGroup.RemoveMember(currentTarget.transform);
            currentTarget = null;
        }
        
        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }
}
