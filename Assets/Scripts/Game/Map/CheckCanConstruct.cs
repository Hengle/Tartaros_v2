﻿namespace Tartaros.Map
{
	using Tartaros.Entities;
	
	using UnityEngine;

	public class CheckCanConstruct : MonoBehaviour, ICheckCanConstruct
    {
        bool ICheckCanConstruct.IsInBoundsMap(Bounds2D bounds, Vector3 position)
        {
            return bounds.CountainsPoint(position, bounds);
        }

        bool ICheckCanConstruct.IsNotOnABuilding(Vector3 BuildingPosition, Vector3 mousePosition)
        {
            int multiplicateur = 5;
            Vector3 positionStart = mousePosition + Vector3.up * multiplicateur;
            Ray ray = new Ray(positionStart, Vector3.down);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.TryGetComponentInParent(out Entity entity))
                {
                    if(entity.EntityType == EntityType.Building)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return true;
            }
            Debug.LogError("Raycast hit nothing");
            return false;
        }
    }
}
