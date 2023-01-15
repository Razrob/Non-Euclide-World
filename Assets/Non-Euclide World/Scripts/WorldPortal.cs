using System;
using UnityEngine;

public partial class WorldPortal : MonoBehaviour, ILayerChangeCallbackReceiver
{
    [SerializeField] private PortalTriggerableEntityTriggerBehaviour _portalCenterTriggerBehaviour;
    [SerializeField] private Transform _portalDirection;
    [SerializeField] private GameObject _portalMask;
    [SerializeField] private WorldPortalSwitchingMethod _switchingMethod;
    [SerializeReference] private SwitchInfoBase _worldPortalSwitchInfo;

    private MeshRenderer _maskMeshRenderer;

    private PortalTriggerableEntity _containsEntity;
    private float? _lastPlayerDotResult;

    public WorldPortalSwitchingMethod SwitchingMethod => _switchingMethod;
    public SwitchInfoBase SwitchInfo => _worldPortalSwitchInfo;
    public Plane RealPortalPlane => new Plane(_portalDirection.forward, _portalMask.transform.position);
    public Plane StartPortalPlane => new Plane(_portalDirection.forward, _portalDirection.position);
    public bool IsActive { get; private set; }

    public event Action<WorldPortal> OnActiveChange;

    private void OnValidate()
    {
        _maskMeshRenderer = _portalMask.GetComponentInChildren<MeshRenderer>(true);

        switch (_switchingMethod)
        {
            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse:

                if (_worldPortalSwitchInfo?.GetType() != typeof(SpecificSwitchInfo))
                    _worldPortalSwitchInfo = new SpecificSwitchInfo();

                break;

            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse_Two_Sided:

                if (_worldPortalSwitchInfo?.GetType() != typeof(SpecificSwitchInfo))
                    _worldPortalSwitchInfo = new SpecificSwitchInfo();

                break;

            default: _worldPortalSwitchInfo = null;
                break;
        }
    }

    private void Awake()
    {
        _portalCenterTriggerBehaviour.EnterEvent += OnEnter;
        _portalCenterTriggerBehaviour.StayEvent += OnStay;
        _portalCenterTriggerBehaviour.ExitEvent += OnExit;
    }

    private float GetTriggerablePortalDotResult(ITriggerable triggerable)
    {
        Vector3 triggerableToPortalDirection = (_portalMask.transform.position.XZ() - triggerable.Transform.position.XZ()).normalized;
        Vector3 portDirection = _portalDirection.forward.XZ().normalized;

        return Vector3.Dot(triggerableToPortalDirection, portDirection);
    }

    private WorldLayerID CalculateNextLayer(ITriggerable triggerable, float dotResult, NextLayerCalculateReason calculateReason)
    {
        return WorldLayerCalculator.CalculateNextPortalLayer(
            new LayerCalculationInfo
            {
                WorldPortal = this,
                PortalEnterSide = dotResult >= 0 ? PortalEnterSide.Forward : PortalEnterSide.Backward,
                MainLayer = WorldCore.Instance.ActiveWorldLayerID.Value,
                LayerCalculateReason = calculateReason,
            });
    }

    private void OnEnter(ITriggerable triggerable)
    {
        _portalMask.transform.position = _portalDirection.position;
        _containsEntity = triggerable.Cast<PortalTriggerableEntity>();
        RefreshLayers();
    }

    private void OnStay(ITriggerable triggerable)
    {
        if (_containsEntity != null)
            RefreshLayers();

        RefreshNextLayerView();
    }

    private void Update()
    {
        if (_containsEntity != null)
            RefreshLayers();

        RefreshNextLayerView();
    }

    private void OnExit(ITriggerable triggerable)
    {
        _containsEntity = null;
        _lastPlayerDotResult = null;
        _portalMask.transform.position = _portalDirection.position;
    }

    private void RefreshNextLayerView()
    {
        float dotResult = GetTriggerablePortalDotResult(PortalTriggerableEntity.Instance);

        WorldLayerID nextLayerID =
            CalculateNextLayer(PortalTriggerableEntity.Instance, dotResult, NextLayerCalculateReason.Refresh_Portal_Mask);

        int id;

        if (nextLayerID != WorldCore.Instance.ActiveWorldLayerID.Value)
            id = nextLayerID.LayerID;
        else
            id = 0;

        _maskMeshRenderer.material.SetInt(WorldCore.STENCIL_VALUE_SHADER_PARAMETER, id);
    }

    private void RefreshLayers()
    {
        if (_containsEntity is null)
            return;

        float dotResult = GetTriggerablePortalDotResult(_containsEntity);
        float minCameraPortalDistance = Mathf.Min(_containsEntity.Camera.nearClipPlane * 8f, 0.3f);
        float portalOffcet = minCameraPortalDistance * 2f;

        float cameraToRealPortalDistance = RealPortalPlane.GetDistanceToPoint(_containsEntity.Camera.transform.position);
        float cameraToStartPortalDistance = StartPortalPlane.GetDistanceToPoint(_containsEntity.Camera.transform.position);

        if (Mathf.Abs(cameraToRealPortalDistance) < minCameraPortalDistance)
        {
            if (Mathf.Abs(cameraToRealPortalDistance) < Mathf.Abs(cameraToStartPortalDistance))
                _portalMask.transform.position = _portalDirection.position;
            else
            {
                Vector3 direction = _portalDirection.forward * (dotResult >= 0f ? 1f : -1f);
                _portalMask.transform.position = _portalDirection.position + direction * portalOffcet;
            }
        }

        dotResult = GetTriggerablePortalDotResult(_containsEntity);

        if (_lastPlayerDotResult.HasValue)
        {
            if (dotResult.SignsDiferrent(_lastPlayerDotResult.Value))
            {
                WorldLayerID nextLayerID =
                    CalculateNextLayer(_containsEntity, _lastPlayerDotResult.Value, NextLayerCalculateReason.Change_Main_Layer);

                WorldCore.Instance.SetActiveLayer(nextLayerID.LayerID);

                return;
            }
        }

        _lastPlayerDotResult = dotResult;
    }

    public void OnLayerActivate()
    {
        SetPortalActive(true);

        if (Application.isPlaying)
        {
            if (_maskMeshRenderer is null)
                OnValidate();

            RefreshNextLayerView();
        }
    }

    public void OnLayerDeactivate()
    {
        SetPortalActive(false);
    }

    public void SetPortalActive(bool value)
    {
        _portalCenterTriggerBehaviour.gameObject.SetActive(value);
        _portalMask.gameObject.SetActive(value);
        gameObject.SetActive(value);
        IsActive = value;
        OnActiveChange?.Invoke(this);
    }
}
