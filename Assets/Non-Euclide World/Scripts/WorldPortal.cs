using System;
using System.Linq;
using UnityEngine;

public partial class WorldPortal : MonoBehaviour, ILayerChangeCallbackReceiver
{
    [SerializeField] private PortalTriggerableEntityTriggerBehaviour _portalCenterTriggerBehaviour;
    [SerializeField] private Transform _portalDirection;
    [SerializeField] private GameObject _portalMask;
    [SerializeField] private WorldPortalSwitchingMethod _switchingMethod;
    [SerializeReference] private WorldPortalSwitchInfoBase _worldPortalSwitchInfo;

    private PortalTriggerableEntity _containsEntity;
    private float? _lastPlayerDotResult;

    public Plane RealPortalPlane => new Plane(_portalDirection.forward, _portalMask.transform.position);
    public Plane StartPortalPlane => new Plane(_portalDirection.forward, _portalDirection.position);
    public bool IsActive { get; private set; }

    public event Action<WorldPortal> OnActiveChange;

    private void OnValidate()
    {
        switch (_switchingMethod)
        {
            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse:

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
        _portalCenterTriggerBehaviour.StayEvent += OnStay;
        _portalCenterTriggerBehaviour.ExitEvent += OnExit;
    }

    private float GetTriggerablePortalDotResult(ITriggerable triggerable)
    {
        Vector3 triggerableToPortalDirection = (_portalMask.transform.position.XZ() - triggerable.Transform.position.XZ()).normalized;
        Vector3 portDirection = _portalDirection.forward.XZ().normalized;

        return Vector3.Dot(triggerableToPortalDirection, portDirection);
    }

    private WorldLayerID CalculateNextLayer(ITriggerable triggerable, float dotResult, int currentLayerID, bool withWrite = true)
    {
        int? nextLayer = null;

        switch (_switchingMethod)
        {
            case WorldPortalSwitchingMethod.Switch_To_Next_Layer_With_Reverse:

                nextLayer = dotResult >= 0 
                    ? WorldLayerExtensions.CalculateNextLayer(currentLayerID).LayerID
                    : WorldLayerExtensions.CalculatePreviousLayer(currentLayerID).LayerID;
                break;

            case WorldPortalSwitchingMethod.Switch_To_Previous_Layer_With_Reverse:

                nextLayer = dotResult >= 0
                    ? WorldLayerExtensions.CalculatePreviousLayer(currentLayerID).LayerID
                    : WorldLayerExtensions.CalculateNextLayer(currentLayerID).LayerID;
                break;

            case WorldPortalSwitchingMethod.Switch_To_Specific_Layer_With_Reverse:

                WorldPortalSpecificSwitchInfo info = _worldPortalSwitchInfo.Cast<WorldPortalSpecificSwitchInfo>();

                if (dotResult >= 0)
                {
                    nextLayer = info.NextWorldLayerID;

                    if (withWrite)
                        info.SetPreviousWorldLayerID(currentLayerID);
                }
                else
                {
                    if (info.PreviousWorldLayerID.HasValue)
                    {
                        nextLayer = info.PreviousWorldLayerID;
                        if (withWrite)
                            info.SetPreviousWorldLayerID(null);
                    }
                    else
                        nextLayer = currentLayerID;
                }
                break;
        }

        return new WorldLayerID(nextLayer.Value);
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
    }

    private void Update()
    {
        if (_containsEntity != null)
            RefreshLayers();
    }

    private void OnExit(ITriggerable triggerable)
    {
        _containsEntity = null;
        _lastPlayerDotResult = null;
        _portalMask.transform.position = _portalDirection.position;
    }

    private void RefreshLayers()
    {
        if (_containsEntity is null)
            return;

        float dotResult = GetTriggerablePortalDotResult(_containsEntity);
        float minCameraPortalDistance = Mathf.Min(_containsEntity.Camera.nearClipPlane * 10f, 0.3f);
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

        //WorldCore.Instance.SetNextLayer(
        //    CalculateNextLayer(_containsEntity, dotResult, WorldCore.Instance.ActiveWorldLayerID.Value, false).LayerID);

        if (_lastPlayerDotResult.HasValue)
        {
            if (dotResult.SignsDiferrent(_lastPlayerDotResult.Value))
            {
                WorldLayerID nextLayerID =
                    CalculateNextLayer(_containsEntity, _lastPlayerDotResult.Value, WorldCore.Instance.ActiveWorldLayerID.Value);

                WorldCore.Instance.SetActiveLayer(nextLayerID.LayerID, 0);

                //WorldCore.Instance.SetActiveLayer(nextLayerID.LayerID,
                //    CalculateNextLayer(_containsEntity, dotResult, nextLayerID.LayerID, false).LayerID);

                return;
            }
        }

        _lastPlayerDotResult = dotResult;
    }

    public void OnLayerActivate()
    {
        SetPortalActive(true);
    }

    public void OnLayerDeactivate()
    {
        SetPortalActive(false);
    }

    public void SetPortalActive(bool value)
    {
        _portalCenterTriggerBehaviour.gameObject.SetActive(value);
        _portalMask.gameObject.SetActive(value);
        IsActive = value;
        OnActiveChange?.Invoke(this);
    }
}