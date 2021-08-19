using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TailPlayersFood : MonoBehaviour, IInteractable
{
    FoodSpawner _foodSpawner;

    public static UnityAction OnFoodTaken;

    public void SetFoodSpawner(FoodSpawner pFoodSpawner) => _foodSpawner = pFoodSpawner;

    public void Interact()
    {
        _foodSpawner.AddAvailablePosition(transform.position);
        OnFoodTaken?.Invoke();
    }
}
