using System;
using System.Linq;
using UnityEngine;

public partial class WorldPortal : MonoBehaviour, ILayerChangeCallbackReceiver
{
    [SerializeField] private PortalTriggerableEntityTriggerBehaviour _portalCenterTriggerBehaviour;
    [SerializeField] private MainCameraContainsTriggerBehaviour _visualMaskTriggerBehaviour;
    [SerializeField] private Transform _portalDirection;
    [SerializeField] private MeshRenderer _portalRenderer;
    [SerializeField] private WorldPortalSwitchingMethod _switchingMethod;
    [SerializeReference] private WorldPortalSwitchInfoBase _worldPortalSwitchInfo;

    private float? _lastEnterDotResult;

    public bool CameraInMaskZone => _visualMaskTriggerBehaviour.ContainsComponents.Count > 0;
    public bool IsActive { get; private set; }

    public event Action<WorldPortal> OnActiveChange;
    public event Action<WorldPortal> OnCameraTriggerChange;

    private void OnValidate()
    {
        switch (_switchingMethod)
        {
            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer:

                if (_worldPortalSwitchInfo?.GetType() != typeof(WorldPortalSpecificSwitchInfo))
                    _worldPortalSwitchInfo = new WorldPortalSpecificSwitchInfo();

                break;

            default: _worldPortalSwitchInfo = null;
                break;
        }
    }

    private void Awake()
    {
        _portalCenterTriggerBehaviour.EnterEvent += OnEnter;
        _portalCenterTriggerBehaviour.ExitEvent += OnExit;

        _visualMaskTriggerBehaviour.EnterEvent += triggerable => OnCameraTriggerChange?.Invoke(this);
        _visualMaskTriggerBehaviour.ExitEvent += triggerable => OnCameraTriggerChange?.Invoke(this);
    }

    private float GetTriggerablePortalDotResult(ITriggerable triggerable)
    {
        Vector3 triggerableToPortalDirection = (_portalDirection.position.XZ() - triggerable.Transform.position.XZ()).normalized;
        Vector3 portDirection = _portalDirection.forward.XZ().normalized;

        return Vector3.Dot(triggerableToPortalDirection, portDirection);
    }

    private void OnEnter(ITriggerable triggerable)
    {
        float dotResult = GetTriggerablePortalDotResult(triggerable);
        _lastEnterDotResult = dotResult;
        WorldLayer nextLayer = null;

        switch (_switchingMethod)
        {
            case WorldPortalSwitchingMethod.Switch_To_Next_Layer:

                if (dotResult >= 0)
                    nextLayer = WorldLayerExtensions.CalculateNextLayer(WorldCore.ActiveWorldLayerID.Value);
                else
                    nextLayer = WorldLayerExtensions.CalculatePreviousLayer(WorldCore.ActiveWorldLayerID.Value);

                break;

            case WorldPortalSwitchingMethod.Switch_To_Previous_Layer:

                if (dotResult >= 0)
                    nextLayer = WorldLayerExtensions.CalculatePreviousLayer(WorldCore.ActiveWorldLayerID.Value);
                else
                    nextLayer = WorldLayerExtensions.CalculateNextLayer(WorldCore.ActiveWorldLayerID.Value);

                break;

            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer:

                WorldPortalSpecificSwitchInfo info = _worldPortalSwitchInfo.Cast<WorldPortalSpecificSwitchInfo>();

                if (dotResult >= 0)
                {
                    nextLayer = WorldLayersRepository.GetWithID(info.NextWorldLayerID);
                    info.SetPreviousWorldLayerID(WorldCore.ActiveWorldLayerID.Value);
                }
                else
                    nextLayer = WorldLayersRepository.GetWithID(info.PreviousWorldLayerID);

                break;
        }

        WorldCore.SetActiveLayers(nextLayer.LayerID);
        //Debug.Log("_last: " + _lastEnterDotResult.Value);
        //Debug.Log(transform.parent.name + " " + nextLayer.LayerID);

        _portalRenderer.enabled = false;
    }

    private void OnExit(ITriggerable triggerable)
    {
        if (!_lastEnterDotResult.HasValue)
            return;

        //Debug.Log("exit");
        float dotResult = GetTriggerablePortalDotResult(triggerable);

        if (dotResult.SignsDiferrent(_lastEnterDotResult.Value))
        {
            _portalCenterTriggerBehaviour.Collider.enabled = false;
            //_visualMaskTriggerBehaviour.gameObject.SetActive(false);
            IsActive = false;
            OnActiveChange?.Invoke(this);
        }
        else 
        {
            //Debug.Log("last: " + _lastEnterDotResult.Value + " current: " + dotResult);

            switch (_switchingMethod)
            {
                case WorldPortalSwitchingMethod.Switch_To_Next_Layer:
                     
                    WorldCore.SetActiveLayers(WorldCore.LastActiveWorldLayerID.Value);

                    break;

                case WorldPortalSwitchingMethod.Switch_To_Previous_Layer:

                    WorldCore.SetActiveLayers(WorldCore.LastActiveWorldLayerID.Value);

                    break;

                case WorldPortalSwitchingMethod.Switch_To_Specific_Layer:

                    WorldPortalSpecificSwitchInfo info = _worldPortalSwitchInfo.Cast<WorldPortalSpecificSwitchInfo>();
                    WorldCore.SetActiveLayers(info.PreviousWorldLayerID);

                    break;
            }
        }

        _lastEnterDotResult = null;

        if (!CameraInMaskZone)
            _visualMaskTriggerBehaviour.gameObject.SetActive(false);
        else
        {
            OnCameraTriggerChange += CameraTriggerChange;

            void CameraTriggerChange(WorldPortal worldPortal)
            {
                if (worldPortal.IsActive || worldPortal.CameraInMaskZone)
                    return;
                
                OnCameraTriggerChange -= CameraTriggerChange;
                _visualMaskTriggerBehaviour.gameObject.SetActive(false);
            }
        }
    }

    public void OnLayerActivate()
    {
        float? dotResult = null;

        if (PortalTriggerableEntity.Instance != null)
            dotResult = GetTriggerablePortalDotResult(PortalTriggerableEntity.Instance);

        //foreach (WorldPortal portal in WorldLayersRepository.GetWithID(WorldCore.ActiveWorldLayerID.Value).Portals)
        //{
        //    if (portal == this)
        //        continue;

        //    if (!portal._lastEnterDotResult.HasValue)
        //        continue;

        //    if (!dotResult.HasValue && portal._lastEnterDotResult.Value < dot)
        //}

        if (dotResult.HasValue && dotResult.Value < 0f)
            _portalRenderer.enabled = true;

        if (Application.isPlaying)
        {
            WorldLayer lastWorldLayer = null; 
            
            if (WorldCore.LastActiveWorldLayerID.HasValue)
                lastWorldLayer = WorldLayersRepository.GetWithID(WorldCore.LastActiveWorldLayerID.Value);

            if (lastWorldLayer is null || 
                lastWorldLayer.Portals.Count is 0 || lastWorldLayer.Portals.All(p => !p.IsActive && !p.CameraInMaskZone))
            {
                _portalCenterTriggerBehaviour.Collider.enabled = true;
                _visualMaskTriggerBehaviour.gameObject.SetActive(true);
                _portalRenderer.enabled = true;
                IsActive = true;
                OnActiveChange?.Invoke(this);
            }
            else
            {
                foreach (WorldPortal worldPortal in lastWorldLayer.Portals)
                {
                    if (!worldPortal.IsActive && !worldPortal.CameraInMaskZone)
                        continue;

                    worldPortal.OnActiveChange += OnOtherPortalActiveChange;
                    worldPortal.OnCameraTriggerChange += OnOtherPortalCameraTriggerChange;
                    break;
                }

                void OnOtherPortalActiveChange(WorldPortal otherPortal)
                {
                    if (otherPortal.IsActive || otherPortal.CameraInMaskZone)
                        return;

                    otherPortal.OnActiveChange -= OnOtherPortalActiveChange;
                    otherPortal.OnCameraTriggerChange -= OnOtherPortalCameraTriggerChange;

                    _portalCenterTriggerBehaviour.Collider.enabled = true;
                    _visualMaskTriggerBehaviour.gameObject.SetActive(true);
                    _portalRenderer.enabled = true;
                    IsActive = true;
                    OnActiveChange?.Invoke(this);
                }

                void OnOtherPortalCameraTriggerChange(WorldPortal otherPortal)
                {
                    if (otherPortal.IsActive || otherPortal.CameraInMaskZone)
                        return;

                    otherPortal.OnActiveChange -= OnOtherPortalActiveChange;
                    otherPortal.OnCameraTriggerChange -= OnOtherPortalCameraTriggerChange;

                    _portalCenterTriggerBehaviour.Collider.enabled = true;
                    _visualMaskTriggerBehaviour.gameObject.SetActive(true);
                    _portalRenderer.enabled = true;
                    IsActive = true;
                    OnActiveChange?.Invoke(this);
                }
            }
        }
        else
        {
            _portalRenderer.enabled = true;
            _portalCenterTriggerBehaviour.Collider.enabled = true;
            _visualMaskTriggerBehaviour.gameObject.SetActive(true);
            IsActive = true;
            OnActiveChange?.Invoke(this);
        }
    }

    public void OnLayerDeactivate()
    {
        if (Application.isPlaying)
        {

        }
        else
        {
            _portalRenderer.enabled = false;
            _portalCenterTriggerBehaviour.Collider.enabled = false;
            _visualMaskTriggerBehaviour.gameObject.SetActive(false);
            IsActive = false;
            OnActiveChange?.Invoke(this);
        }
    }
}
