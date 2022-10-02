using System;
using UnityEngine;

namespace RPGEngine.Core
{
    [CreateAssetMenu(fileName = "GameObjectFloatGameEvent",
        menuName = "RPG Project/Game Events/GameObject Float GameEvent")]
    public class GameObjectFloatGameEvent : GameEvent<GameObject, float> { }
}