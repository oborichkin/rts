using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private Building _placedBuilding = null;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    void Update()
    {
        if (_placedBuilding != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }

            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                10000000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                _placedBuilding.SetPosition(_raycastHit.point);
                if (_lastPlacementPosition != _raycastHit.point)
                {
                    _placedBuilding.CheckValidPlacement();
                }
                _lastPlacementPosition = _raycastHit.point;
            }

            if (
                _placedBuilding.HasValidPlacement &&
                Input.GetMouseButtonDown(0) &&
                !EventSystem.current.IsPointerOverGameObject()
            )
            {
                _PlaceBuilding();
            }
        }
    }

    void _PreparePlacedBuilding(int buildingDataIndex)
    {
        Building building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex]
        );
        // link the data into the manager
        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
    }

    void _PlaceBuilding()
    {
        _placedBuilding.Place();
        _placedBuilding = null;
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
    }

    void _CancelPlacedBuilding()
    {
        // destroy the "phantom" building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
    }

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(buildingDataIndex);
    }
}